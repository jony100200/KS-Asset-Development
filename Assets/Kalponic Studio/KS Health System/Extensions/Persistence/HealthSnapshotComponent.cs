using UnityEngine;
using KalponicStudio.Health;

namespace KalponicStudio.Health.Extensions.Persistence
{
    /// <summary>
    /// Enhanced health snapshot component with comprehensive state capture and restoration.
    /// Supports JSON serialization and includes validation for data integrity.
    ///
    /// This component allows you to save and load the complete state of health, shield,
    /// and status effect systems, making it perfect for:
    /// - Game saves and checkpoints
    /// - Scene transitions
    /// - Character respawning
    /// - Multiplayer synchronization
    /// - Undo/redo functionality
    ///
    /// Features:
    /// - Complete health system state (HP, shields, status effects)
    /// - Advanced settings (regeneration, damage reduction, resistances)
    /// - Transform data (position/rotation)
    /// - Metadata (entity name/tag)
    /// - JSON serialization for easy storage
    /// - Validation to prevent corrupted states
    /// - Editor context menu for testing
    /// </summary>
    public class HealthSnapshotComponent : MonoBehaviour, IHealthSerializable
    {
        [Header("Component References")]
        [Tooltip("Reference to the HealthSystem component. If not set, will auto-find on this GameObject.")]
        [SerializeField] private HealthSystem healthSystem;

        [Tooltip("Reference to the ShieldSystem component. If not set, will auto-find on this GameObject.")]
        [SerializeField] private ShieldSystem shieldSystem;

        [Tooltip("Reference to the StatusEffectSystem component. If not set, will auto-find on this GameObject.")]
        [SerializeField] private StatusEffectSystem statusEffectSystem;

        [Header("Snapshot Configuration")]
        [Tooltip("Whether to include transform (position/rotation) data in snapshots.")]
        [SerializeField] private bool includeTransform = true;

        [Tooltip("Whether to include metadata (name/tag) in snapshots.")]
        [SerializeField] private bool includeMetadata = true;

        /// <summary>
        /// Initializes component references on awake
        /// </summary>
        protected void Awake()
        {
            if (healthSystem == null)
            {
                healthSystem = GetComponent<HealthSystem>();
            }

            if (shieldSystem == null)
            {
                shieldSystem = GetComponent<ShieldSystem>();
            }

            if (statusEffectSystem == null)
            {
                statusEffectSystem = GetComponent<StatusEffectSystem>();
            }
        }

        /// <summary>
        /// Captures a comprehensive snapshot of the current health state.
        ///
        /// This method creates a complete snapshot including:
        /// - Health values (current/max health, downed/dead states)
        /// - Shield values (if shield system is present)
        /// - Status effects (if status effect system is present)
        /// - Advanced health settings (regeneration, damage reduction, resistances)
        /// - Transform data (position/rotation if enabled)
        /// - Metadata (name/tag if enabled)
        ///
        /// The snapshot can be serialized to JSON for saving to disk or PlayerPrefs.
        /// </summary>
        /// <returns>A complete HealthSnapshot containing all captured state data</returns>
        public HealthSnapshot CaptureSnapshot()
        {
            HealthSnapshot snapshot = HealthSnapshot.Create();

            // Capture entity metadata for identification
            if (includeMetadata)
            {
                snapshot.EntityName = gameObject.name;
                snapshot.EntityTag = gameObject.tag;
            }

            // Capture transform data for spatial restoration
            if (includeTransform)
            {
                snapshot.Position = transform.position;
                snapshot.Rotation = transform.rotation;
            }

            // Capture core health system data
            if (healthSystem != null)
            {
                snapshot.MaxHealth = healthSystem.MaxHealth;
                snapshot.CurrentHealth = healthSystem.CurrentHealth;
                snapshot.IsDowned = healthSystem.IsDowned;
                snapshot.IsDead = healthSystem.IsDead;
                snapshot.IsInvulnerable = healthSystem.IsInvulnerable;
                snapshot.DownedTimeRemaining = healthSystem.DownedTimeRemaining;

                // Get invulnerability timer via reflection (private field access)
                var invulnerabilityTimerField = healthSystem.GetType().GetField("invulnerabilityTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (invulnerabilityTimerField != null)
                {
                    snapshot.InvulnerabilityTimeRemaining = (float)invulnerabilityTimerField.GetValue(healthSystem);
                }

                // Capture advanced health settings via reflection
                // These are private fields that control regeneration and damage mitigation
                var regenRateField = healthSystem.GetType().GetField("regenerationRate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var regenDelayField = healthSystem.GetType().GetField("regenerationDelay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var regenerateHealthField = healthSystem.GetType().GetField("regenerateHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var flatReductionField = healthSystem.GetType().GetField("flatDamageReduction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var percentReductionField = healthSystem.GetType().GetField("percentDamageReduction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var resistancesField = healthSystem.GetType().GetField("damageResistances", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (regenerateHealthField != null) snapshot.RegenerationEnabled = (bool)regenerateHealthField.GetValue(healthSystem);
                if (regenRateField != null) snapshot.RegenerationRate = (float)regenRateField.GetValue(healthSystem);
                if (regenDelayField != null) snapshot.RegenerationDelay = (float)regenDelayField.GetValue(healthSystem);
                if (flatReductionField != null) snapshot.FlatDamageReduction = (float)flatReductionField.GetValue(healthSystem);
                if (percentReductionField != null) snapshot.PercentDamageReduction = (float)percentReductionField.GetValue(healthSystem);

                // Capture damage resistance array
                if (resistancesField != null)
                {
                    var resistances = (DamageResistance[])resistancesField.GetValue(healthSystem);
                    snapshot.DamageResistances = new DamageResistanceSnapshot[resistances.Length];
                    for (int i = 0; i < resistances.Length; i++)
                    {
                        snapshot.DamageResistances[i] = DamageResistanceSnapshot.FromDamageResistance(resistances[i]);
                    }
                }
            }

            // Capture shield system data
            if (shieldSystem != null)
            {
                snapshot.MaxShield = shieldSystem.MaxShield;
                snapshot.CurrentShield = shieldSystem.CurrentShield;
            }

            // Capture active status effects
            if (statusEffectSystem != null)
            {
                var activeEffects = statusEffectSystem.GetActiveEffects();
                snapshot.StatusEffects = new System.Collections.Generic.List<StatusEffectSnapshot>(activeEffects.Count);
                foreach (var effect in activeEffects)
                {
                    snapshot.StatusEffects.Add(StatusEffectSnapshot.FromStatusEffect(effect));
                }
            }

            return snapshot;
        }

        /// <summary>
        /// Restores health state from a snapshot with validation.
        ///
        /// This method safely restores the complete state of all health-related systems:
        /// - Health values and states (current health, downed/dead status)
        /// - Shield values (if shield system is present)
        /// - Status effects (if status effect system is present)
        /// - Advanced health settings (regeneration, damage reduction, resistances)
        /// - Transform data (position/rotation if enabled)
        ///
        /// The method includes validation to ensure data integrity and will log errors
        /// for invalid snapshots rather than corrupting the game state.
        /// </summary>
        /// <param name="snapshot">The HealthSnapshot containing the state to restore</param>
        public void RestoreSnapshot(HealthSnapshot snapshot)
        {
            if (!snapshot.IsValid())
            {
                Debug.LogError("[HealthSnapshotComponent] Cannot restore invalid snapshot");
                return;
            }

            // Restore transform if included
            if (includeTransform)
            {
                transform.SetPositionAndRotation(snapshot.Position, snapshot.Rotation);
            }

            // Restore health system
            if (healthSystem != null)
            {
                healthSystem.SetMaxHealth(snapshot.MaxHealth > 0 ? snapshot.MaxHealth : healthSystem.MaxHealth);
                healthSystem.SetHealth(snapshot.CurrentHealth);

                // Restore advanced health settings via reflection
                var regenRateField = healthSystem.GetType().GetField("regenerationRate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var regenDelayField = healthSystem.GetType().GetField("regenerationDelay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var regenerateHealthField = healthSystem.GetType().GetField("regenerateHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var flatReductionField = healthSystem.GetType().GetField("flatDamageReduction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var percentReductionField = healthSystem.GetType().GetField("percentDamageReduction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var resistancesField = healthSystem.GetType().GetField("damageResistances", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (regenerateHealthField != null) regenerateHealthField.SetValue(healthSystem, snapshot.RegenerationEnabled);
                if (regenRateField != null) regenRateField.SetValue(healthSystem, snapshot.RegenerationRate);
                if (regenDelayField != null) regenDelayField.SetValue(healthSystem, snapshot.RegenerationDelay);
                if (flatReductionField != null) flatReductionField.SetValue(healthSystem, snapshot.FlatDamageReduction);
                if (percentReductionField != null) percentReductionField.SetValue(healthSystem, snapshot.PercentDamageReduction);


                if (resistancesField != null && snapshot.DamageResistances != null)
                {
                    var resistances = new DamageResistance[snapshot.DamageResistances.Length];
                    for (int i = 0; i < snapshot.DamageResistances.Length; i++)
                    {
                        resistances[i] = snapshot.DamageResistances[i].ToDamageResistance();
                    }
                    resistancesField.SetValue(healthSystem, resistances);
                }

                if (snapshot.IsDead)
                {
                    healthSystem.ForceDead();
                }
                else if (snapshot.IsDowned)
                {
                    healthSystem.ForceDowned(snapshot.DownedTimeRemaining);
                }
            }

            // Restore shield system
            if (shieldSystem != null)
            {
                if (snapshot.MaxShield > 0)
                {
                    shieldSystem.SetMaxShield(snapshot.MaxShield);
                }
                shieldSystem.SetShield(snapshot.CurrentShield);
            }

            // Restore status effects
            if (statusEffectSystem != null && snapshot.StatusEffects != null)
            {
                var effects = new System.Collections.Generic.List<StatusEffect>(snapshot.StatusEffects.Count);
                foreach (var effectSnapshot in snapshot.StatusEffects)
                {
                    effects.Add(effectSnapshot.ToStatusEffect());
                }
                statusEffectSystem.RestoreEffects(effects);
            }
        }

        /// <summary>
        /// Saves the current snapshot to PlayerPrefs as JSON.
        ///
        /// This method is accessible via the component's context menu in the Unity Editor.
        /// The snapshot is stored using the GameObject's name as a key, allowing multiple
        /// entities to save their states independently.
        ///
        /// Use this for quick testing and debugging of the snapshot functionality.
        /// </summary>
        [ContextMenu("Save Snapshot to PlayerPrefs")]
        private void SaveSnapshotToPrefs()
        {
            var snapshot = CaptureSnapshot();
            string json = snapshot.ToJson();
            PlayerPrefs.SetString($"HealthSnapshot_{gameObject.name}", json);
            PlayerPrefs.Save();
            Debug.Log($"[HealthSnapshotComponent] Saved snapshot ({json.Length} chars)");
        }

        /// <summary>
        /// Loads a previously saved snapshot from PlayerPrefs.
        ///
        /// This method is accessible via the component's context menu in the Unity Editor.
        /// It attempts to load a snapshot using the GameObject's name as a key.
        ///
        /// Use this for quick testing and debugging of the snapshot restoration.
        /// </summary>
        [ContextMenu("Load Snapshot from PlayerPrefs")]
        private void LoadSnapshotFromPrefs()
        {
            string json = PlayerPrefs.GetString($"HealthSnapshot_{gameObject.name}", "");
            if (!string.IsNullOrEmpty(json))
            {
                var snapshot = HealthSnapshot.FromJson(json);
                RestoreSnapshot(snapshot);
                Debug.Log("[HealthSnapshotComponent] Loaded snapshot from PlayerPrefs");
            }
            else
            {
                Debug.LogWarning("[HealthSnapshotComponent] No snapshot found in PlayerPrefs");
            }
        }
    }
}
