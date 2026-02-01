using System;
using UnityEngine;
using UnityEngine.Events;

namespace KalponicStudio.Health
{
    /// <summary>
    /// Reusable health system component for 2D and 3D games.
    /// Handles health management, damage, healing, and death events.
    /// Can be attached to any GameObject that needs health functionality.
    /// Now uses event channels for better decoupling and modularity.
    /// </summary>
    public class HealthSystem : MonoBehaviour, IHealthComponent
    {
        [Header("Event Channels")]
        [Tooltip("Event channel for broadcasting health-related events. Improves decoupling between systems.")]
        [SerializeField] private HealthEventChannelSO healthEvents;

        [Header("Health Settings")]
        [Tooltip("Maximum health value this entity can have.")]
        [SerializeField] private int maxHealth = 100;

        [Tooltip("Current health value. Will be clamped between 0 and maxHealth.")]
        [SerializeField] private int currentHealth = 100;

        [Tooltip("Whether health should automatically regenerate over time.")]
        [SerializeField] private bool regenerateHealth = false;

        [Tooltip("How much health to regenerate per second when regeneration is enabled.")]
        [SerializeField] private float regenerationRate = 1f;

        [Tooltip("Delay in seconds before regeneration starts after taking damage.")]
        [SerializeField] private float regenerationDelay = 2f;

        [Tooltip("Flat amount of damage to reduce from all incoming damage.")]
        [SerializeField] private float flatDamageReduction = 0f;

        [Tooltip("Percentage of damage to reduce (0-1). Applied after flat reduction.")]
        [SerializeField, Range(0f, 1f)] private float percentDamageReduction = 0f;

        [Tooltip("Specific damage type resistances. Each resistance reduces damage of that type.")]
        [SerializeField] private DamageResistance[] damageResistances = new DamageResistance[0];

        [Header("Downed State")]
        [Tooltip("Whether this entity can enter a downed state instead of dying immediately.")]
        [SerializeField] private bool enableDownedState = false;

        [Tooltip("How long the entity stays downed before dying (if not revived).")]
        [SerializeField] private float downedDuration = 5f;

        [Tooltip("Whether this entity can be revived while downed.")]
        [SerializeField] private bool allowRevive = true;

        [Header("Invulnerability")]
        [Tooltip("Whether this entity can become temporarily invulnerable after taking damage.")]
        [SerializeField] private bool enableInvulnerability = true;

        [Tooltip("How long invulnerability lasts after taking damage.")]
        [SerializeField] private float invulnerabilityDuration = 1f;

        [Tooltip("Whether to flash the entity's renderer during invulnerability.")]
        [SerializeField] private bool flashDuringInvulnerability = true;

        [Tooltip("How fast to flash during invulnerability (seconds between flashes).")]
        [SerializeField] private float flashInterval = 0.1f;

        [Header("Events")]
        [Tooltip("UnityEvent triggered when health changes. Parameters: (currentHealth, maxHealth)")]
        [SerializeField] private UnityEvent<int, int> onHealthChanged = new UnityEvent<int, int>();

        [Tooltip("UnityEvent triggered when damage is taken. Parameter: damage amount")]
        [SerializeField] private UnityEvent<int> onDamageTaken = new UnityEvent<int>();

        [Tooltip("UnityEvent triggered when healing occurs. Parameter: heal amount")]
        [SerializeField] private UnityEvent<int> onHealed = new UnityEvent<int>();

        [Tooltip("UnityEvent triggered when the entity dies.")]
        [SerializeField] private UnityEvent onDeath = new UnityEvent();

        [Tooltip("UnityEvent triggered when the entity enters downed state.")]
        [SerializeField] private UnityEvent onDowned = new UnityEvent();

        [Tooltip("UnityEvent triggered when the entity is revived.")]
        [SerializeField] private UnityEvent onRevived = new UnityEvent();

        [Tooltip("UnityEvent triggered when downed state expires.")]
        [SerializeField] private UnityEvent onDownedExpired = new UnityEvent();

        [Tooltip("UnityEvent triggered when invulnerability starts.")]
        [SerializeField] private UnityEvent onInvulnerabilityStart = new UnityEvent();

        [Tooltip("UnityEvent triggered when invulnerability ends.")]
        [SerializeField] private UnityEvent onInvulnerabilityEnd = new UnityEvent();

        public event Action<int, int> HealthChanged;
        public event Action<int> DamageTaken;
        public event Action<int> Healed;
        public event Action Death;
        public event Action Downed;
        public event Action Revived;
        public event Action DownedExpired;
        public event Action InvulnerabilityStarted;
        public event Action InvulnerabilityEnded;

        // Public properties
        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public float HealthPercent => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        public bool IsAlive => currentHealth > 0 && !isDead;
        public bool IsInvulnerable => invulnerabilityTimer > 0;
        public bool IsDowned => isDowned;
        public bool IsDead => isDead;
        public float DownedTimeRemaining => downedTimer;

        // Private fields
        private float regenerationTimer = 0f;
        private float invulnerabilityTimer = 0f;
        private float lastDamageTime = 0f;
        private SpriteRenderer spriteRenderer;
        private bool originalSpriteEnabled = true;
        private IShieldAbsorber shieldAbsorber;
        private bool isDowned = false;
        private bool isDead = false;
        private float downedTimer = 0f;

        /// <summary>
        /// Validates the health system setup and logs warnings for common misconfigurations
        /// </summary>
        private void ValidateSetup()
        {
            if (maxHealth <= 0)
            {
                Debug.LogError($"[HealthSystem] Max health must be positive. Current value: {maxHealth}. Setting to 100.", this);
                maxHealth = 100;
            }

            if (currentHealth < 0)
            {
                Debug.LogWarning($"[HealthSystem] Current health cannot be negative. Current value: {currentHealth}. Setting to 0.", this);
                currentHealth = 0;
            }

            if (regenerateHealth && regenerationRate <= 0)
            {
                Debug.LogWarning($"[HealthSystem] Regeneration enabled but rate is not positive. Current rate: {regenerationRate}. Disabling regeneration.", this);
                regenerateHealth = false;
            }

            if (enableInvulnerability && invulnerabilityDuration <= 0)
            {
                Debug.LogWarning($"[HealthSystem] Invulnerability enabled but duration is not positive. Current duration: {invulnerabilityDuration}. Setting to 1 second.", this);
                invulnerabilityDuration = 1f;
            }

            if (enableDownedState && downedDuration <= 0)
            {
                Debug.LogWarning($"[HealthSystem] Downed state enabled but duration is not positive. Current duration: {downedDuration}. Setting to 5 seconds.", this);
                downedDuration = 5f;
            }

            // Validate damage resistances
            foreach (var resistance in damageResistances)
            {
                if (resistance.multiplier < -1f || resistance.multiplier > 2f)
                {
                    Debug.LogWarning($"[HealthSystem] Damage resistance multiplier should be between -1 and 2. Current: {resistance.multiplier} for {resistance.type}", this);
                }
            }
        }

        private void Awake()
        {
            ValidateSetup();

            // Cache components
            spriteRenderer = GetComponent<SpriteRenderer>();
            shieldAbsorber = GetComponent<IShieldAbsorber>();

            // Ensure current health doesn't exceed max
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            isDowned = false;
            isDead = false;

            // Trigger initial health changed event
            RaiseHealthChanged();
        }

        private void Update()
        {
            HandleInvulnerability();
            HandleRegeneration();
            HandleDownedState();
        }

        private void HandleInvulnerability()
        {
            if (invulnerabilityTimer > 0)
            {
                invulnerabilityTimer -= Time.deltaTime;

                // Handle flashing effect
                if (flashDuringInvulnerability && spriteRenderer != null)
                {
                    float flashTime = Time.time - (lastDamageTime + invulnerabilityDuration - invulnerabilityTimer);
                    bool shouldShow = Mathf.FloorToInt(flashTime / flashInterval) % 2 == 0;
                    spriteRenderer.enabled = shouldShow;
                }

                // Check if invulnerability ended
                if (invulnerabilityTimer <= 0)
                {
                    invulnerabilityTimer = 0;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.enabled = originalSpriteEnabled;
                    }
                    onInvulnerabilityEnd?.Invoke();
                }
            }
        }

        private void HandleRegeneration()
        {
            if (!regenerateHealth || !IsAlive || IsInvulnerable || isDowned) return;

            // Check if enough time has passed since last damage
            if (Time.time - lastDamageTime < regenerationDelay) return;

            // Regenerate health
            if (currentHealth < maxHealth)
            {
                int oldHealth = currentHealth;
                currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.RoundToInt(regenerationRate * Time.deltaTime));

                if (currentHealth != oldHealth)
                {
                    RaiseHealthChanged();
                }
            }
        }

        private void HandleDownedState()
        {
            if (!isDowned) return;

            if (downedTimer > 0f)
            {
                downedTimer -= Time.deltaTime;
                if (downedTimer <= 0f)
                {
                    downedTimer = 0f;
                    RaiseDownedExpired();
                    Die();
                }
            }
        }

        /// <summary>
        /// Take damage from any source
        /// </summary>
        /// <param name="damage">The amount of damage to apply</param>
        public void TakeDamage(int damage)
        {
            TakeDamage(DamageInfo.FromAmount(damage));
        }

        /// <summary>
        /// Applies damage with full control over damage properties including type, mitigation, and shield interaction.
        /// </summary>
        /// <param name="damageInfo">Complete damage information including amount, type, source, and flags</param>
        /// <remarks>
        /// Damage processing order:
        /// 1. Check if dead or downed (may trigger death)
        /// 2. Check invulnerability (unless ignored)
        /// 3. Shield absorption (if available and not bypassed)
        /// 4. Damage mitigation (flat/percent + type resistances, unless ignored)
        /// 5. Apply remaining damage to health
        /// 6. Trigger invulnerability if enabled
        /// 7. Fire damage/heath change/death events
        /// </remarks>
        public void TakeDamage(DamageInfo damageInfo)
        {
            if (isDead) return;
            if (damageInfo.Amount <= 0) return;
            if (IsInvulnerable && !damageInfo.IgnoreMitigation) return;
            if (isDowned)
            {
                Die();
                return;
            }

            int remainingDamage = damageInfo.Amount;
            if (shieldAbsorber == null)
            {
                shieldAbsorber = GetComponent<IShieldAbsorber>();
            }

            if (shieldAbsorber != null && !damageInfo.BypassShield && damageInfo.Type != DamageType.True)
            {
                remainingDamage = shieldAbsorber.AbsorbDamage(remainingDamage);
            }

            if (remainingDamage <= 0) return;

            if (!damageInfo.IgnoreMitigation && damageInfo.Type != DamageType.True)
            {
                remainingDamage = ApplyMitigation(remainingDamage, damageInfo.Type);
            }

            if (remainingDamage <= 0) return;

            int oldHealth = currentHealth;
            currentHealth = Mathf.Max(0, currentHealth - remainingDamage);
            lastDamageTime = Time.time;

            // Trigger events
            RaiseHealthChanged();
            RaiseDamageTaken(remainingDamage);

            if (enableInvulnerability)
            {
                StartInvulnerability();
            }

            // Check for death
            if (currentHealth <= 0 && oldHealth > 0)
            {
                if (enableDownedState && allowRevive)
                {
                    EnterDownedState();
                }
                else
                {
                    Die();
                }
            }
        }

        /// <summary>
        /// Heal the entity by restoring health points
        /// </summary>
        /// <param name="healAmount">The amount of health to restore</param>
        /// <remarks>
        /// Healing behavior:
        /// - Cannot heal dead entities
        /// - Cannot heal negative amounts
        /// - Health is capped at maxHealth
        /// - If entity is downed, healing triggers revive
        /// - Fires health changed and healed events
        /// </remarks>
        public void Heal(int healAmount)
        {
            if (isDead || healAmount <= 0) return;

            int oldHealth = currentHealth;
            currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);

            if (currentHealth != oldHealth)
            {
                if (isDowned)
                {
                    Revive(currentHealth);
                    return;
                }

                RaiseHealthChanged();
                RaiseHealed(healAmount);
            }
        }

        /// <summary>
        /// Set health to specific value
        /// </summary>
        /// <param name="newHealth">The new health value to set</param>
        /// <remarks>
        /// Health is automatically clamped between 0 and maxHealth.
        /// Setting health to 0 or below may trigger death or downed state.
        /// If entity is downed and new health > 0, triggers revive.
        /// Fires health changed events.
        /// </remarks>
        public void SetHealth(int newHealth)
        {
            int oldHealth = currentHealth;
            currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);

            if (currentHealth != oldHealth)
            {
                if (isDowned && currentHealth > 0)
                {
                    Revive(currentHealth);
                    return;
                }

                RaiseHealthChanged();

                // Check for death
                if (currentHealth <= 0 && oldHealth > 0)
                {
                    if (enableDownedState && allowRevive)
                    {
                        EnterDownedState();
                    }
                    else
                    {
                        Die();
                    }
                }
            }
        }

        /// <summary>
        /// Set the maximum health value
        /// </summary>
        /// <param name="newMaxHealth">The new maximum health value</param>
        /// <remarks>
        /// Max health is clamped to minimum of 1.
        /// Current health is automatically adjusted if it exceeds new max.
        /// Fires health changed events.
        /// </remarks>
        public void SetMaxHealth(int newMaxHealth)
        {
            maxHealth = Mathf.Max(1, newMaxHealth);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            RaiseHealthChanged();
        }

        public void ConfigureRegeneration(bool enabled, float rate, float delay)
        {
            regenerateHealth = enabled;
            regenerationRate = Mathf.Max(0f, rate);
            regenerationDelay = Mathf.Max(0f, delay);
        }

        public void ConfigureMitigation(float flatReduction, float percentReduction, DamageResistance[] resistances)
        {
            flatDamageReduction = Mathf.Max(0f, flatReduction);
            percentDamageReduction = Mathf.Clamp01(percentReduction);
            damageResistances = resistances ?? new DamageResistance[0];
        }

        public void ConfigureDownedState(bool enabled, float duration, bool canRevive)
        {
            enableDownedState = enabled;
            downedDuration = Mathf.Max(0.1f, duration);
            allowRevive = canRevive;
        }

        public void ConfigureInvulnerability(bool enabled, float duration, bool flashDuring, float interval)
        {
            enableInvulnerability = enabled;
            invulnerabilityDuration = Mathf.Max(0f, duration);
            flashDuringInvulnerability = flashDuring;
            flashInterval = Mathf.Max(0.01f, interval);
        }

        /// <summary>
        /// Fully restore health
        /// </summary>
        public void RestoreFullHealth()
        {
            SetHealth(maxHealth);
        }

        /// <summary>
        /// Kill the entity instantly
        /// </summary>
        public void Kill()
        {
            if (isDead) return;
            currentHealth = 0;
            RaiseHealthChanged();
            Die();
        }

        public void Revive(int healthAmount)
        {
            if (!isDowned || !allowRevive) return;

            isDowned = false;
            downedTimer = 0f;
            currentHealth = Mathf.Clamp(healthAmount, 1, maxHealth);
            RaiseHealthChanged();
            RaiseRevived();
        }

        public void ForceDowned(float timeRemaining)
        {
            if (isDead) return;

            isDowned = true;
            downedTimer = Mathf.Max(0.1f, timeRemaining);
            currentHealth = 0;
            RaiseHealthChanged();
            RaiseDowned();
        }

        public void ForceDead()
        {
            if (isDead) return;

            currentHealth = 0;
            isDowned = false;
            downedTimer = 0f;
            Die();
        }

        public void StartInvulnerability()
        {
            if (!enableInvulnerability) return;

            invulnerabilityTimer = invulnerabilityDuration;
            onInvulnerabilityStart?.Invoke();
            InvulnerabilityStarted?.Invoke();

            // Store original sprite state
            if (spriteRenderer != null)
            {
                originalSpriteEnabled = spriteRenderer.enabled;
            }
        }

        public void EndInvulnerability()
        {
            invulnerabilityTimer = 0;
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = originalSpriteEnabled;
            }
            onInvulnerabilityEnd?.Invoke();
            InvulnerabilityEnded?.Invoke();
        }

        /// <summary>
        /// Check if entity can take damage
        /// </summary>
        public bool CanTakeDamage()
        {
            return IsAlive && !IsInvulnerable && !isDowned;
        }

        // Unity event helpers for easy inspector setup
        public void TakeDamageFromInt(int damage) => TakeDamage(damage);
        public void HealFromInt(int healAmount) => Heal(healAmount);
        public void SetHealthFromInt(int health) => SetHealth(health);
        public void SetMaxHealthFromInt(int maxHealth) => SetMaxHealth(maxHealth);

        private void RaiseHealthChanged()
        {
            HealthChanged?.Invoke(currentHealth, maxHealth);
            onHealthChanged?.Invoke(currentHealth, maxHealth);
            if (healthEvents != null)
            {
                healthEvents.RaiseHealthChanged(currentHealth, maxHealth);
            }
        }

        private void RaiseDamageTaken(int damage)
        {
            DamageTaken?.Invoke(damage);
            onDamageTaken?.Invoke(damage);
            if (healthEvents != null)
            {
                healthEvents.RaiseDamageTaken(damage);
            }
        }

        private void RaiseHealed(int amount)
        {
            Healed?.Invoke(amount);
            onHealed?.Invoke(amount);
            if (healthEvents != null)
            {
                healthEvents.RaiseHealed(amount);
            }
        }

        private void RaiseDeath()
        {
            Death?.Invoke();
            onDeath?.Invoke();
            if (healthEvents != null)
            {
                healthEvents.RaiseDeath();
            }
        }

        private void RaiseDowned()
        {
            Downed?.Invoke();
            onDowned?.Invoke();
            if (healthEvents != null)
            {
                healthEvents.RaiseDowned();
            }
        }

        private void RaiseRevived()
        {
            Revived?.Invoke();
            onRevived?.Invoke();
            if (healthEvents != null)
            {
                healthEvents.RaiseRevived();
            }
        }

        private void RaiseDownedExpired()
        {
            DownedExpired?.Invoke();
            onDownedExpired?.Invoke();
            if (healthEvents != null)
            {
                healthEvents.RaiseDownedExpired();
            }
        }

        private int ApplyMitigation(int damage, DamageType damageType)
        {
            float multiplier = GetResistanceMultiplier(damageType);
            float mitigated = damage * multiplier;

            mitigated -= flatDamageReduction;
            mitigated *= 1f - percentDamageReduction;

            return Mathf.Max(0, Mathf.RoundToInt(mitigated));
        }

        private float GetResistanceMultiplier(DamageType damageType)
        {
            if (damageResistances == null || damageResistances.Length == 0) return 1f;

            for (int i = 0; i < damageResistances.Length; i++)
            {
                if (damageResistances[i].type == damageType)
                {
                    return Mathf.Clamp(damageResistances[i].multiplier, 0f, 2f);
                }
            }

            return 1f;
        }

        private void EnterDownedState()
        {
            if (isDowned || isDead) return;
            isDowned = true;
            downedTimer = Mathf.Max(0.1f, downedDuration);
            RaiseDowned();
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            isDowned = false;
            downedTimer = 0f;
            RaiseDeath();
        }

        /// <summary>
        /// Runs performance benchmarks for common health operations
        /// </summary>
        [ContextMenu("Run Performance Benchmarks")]
        private void RunPerformanceBenchmarks()
        {
            const int iterations = 10000;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            Debug.Log("[HealthSystem] Starting performance benchmarks...");

            // Benchmark TakeDamage
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                TakeDamage(10);
                SetHealth(100); // Reset
            }
            stopwatch.Stop();
            Debug.Log($"[HealthSystem] TakeDamage (10k iterations): {stopwatch.ElapsedMilliseconds}ms");

            // Benchmark Heal
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                SetHealth(50);
                Heal(10);
            }
            stopwatch.Stop();
            Debug.Log($"[HealthSystem] Heal (10k iterations): {stopwatch.ElapsedMilliseconds}ms");

            // Benchmark event firing
            int eventCount = 0;
            HealthChanged += (c, m) => eventCount++;
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                SetHealth(i % 100);
            }
            stopwatch.Stop();
            Debug.Log($"[HealthSystem] HealthChanged events (10k iterations): {stopwatch.ElapsedMilliseconds}ms, Events fired: {eventCount}");

            Debug.Log("[HealthSystem] Performance benchmarks complete.");
        }
    }
}
