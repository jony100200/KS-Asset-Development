using UnityEngine;
using UnityEngine.UI;
using KalponicStudio.Health;
using KalponicStudio.Health.UI;
using KalponicStudio.Health.Extensions.Persistence;

namespace KalponicStudio.Health.Examples
{
    /// <summary>
    /// Example setup for a basic enemy with health system, UI, and save/load functionality.
    /// Attach this to any enemy prefab for instant health functionality.
    /// </summary>
    public class EnemySetup : MonoBehaviour
    {
        [Header("Health Configuration")]
        [SerializeField] private int maxHealth = 50;
        [SerializeField] private bool enableRegeneration = false;

        [Header("Optional Systems")]
        [SerializeField] private bool addShield = false;
        [SerializeField] private bool addVisualEffects = true;
        [SerializeField] private bool addUI = false;
        [SerializeField] private bool addSaveLoad = false;

        [Header("UI References (if addUI is true)")]
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private HealthUIController uiController;

        private HealthSystem healthSystem;
        private ShieldSystem shieldSystem;
        private HealthSnapshotComponent snapshotComponent;

        protected void Awake()
        {
            SetupHealthSystem();
        }

        private void SetupHealthSystem()
        {
            // Add core health system
            healthSystem = gameObject.AddComponent<HealthSystem>();
            healthSystem.SetMaxHealth(maxHealth);

            if (enableRegeneration)
            {
                healthSystem.ConfigureRegeneration(true, 2f, 3f); // 2 HP/sec after 3 second delay
            }

            // Add shield if requested
            if (addShield)
            {
                shieldSystem = gameObject.AddComponent<ShieldSystem>();
                shieldSystem.SetMaxShield(25);
                shieldSystem.SetShield(25);
            }

            // Add visual effects
            if (addVisualEffects)
            {
                gameObject.AddComponent<HealthVisualSystem>();
            }

            // Add UI if requested
            if (addUI)
            {
                SetupUI();
            }

            // Add save/load functionality if requested
            if (addSaveLoad)
            {
                SetupSaveLoad();
            }
        }

        private void SetupUI()
        {
            // Create UI controller if not assigned
            if (uiController == null)
            {
                GameObject uiObject = new GameObject("HealthUI");
                uiObject.transform.SetParent(transform);
                uiController = uiObject.AddComponent<HealthUIController>();
            }

            // Connect health system to UI controller
            uiController.SetHealthSystem(healthSystem);

            // Connect health bar if assigned
            if (healthBar != null)
            {
                // Set up event listener to update health bar
                healthSystem.HealthChanged += (current, max) =>
                {
                    healthBar.UpdateHealth(current, max);
                };

                // Initial update
                healthBar.UpdateHealth(healthSystem.CurrentHealth, healthSystem.MaxHealth);
            }
        }

        private void SetupSaveLoad()
        {
            // Add snapshot component for save/load functionality
            snapshotComponent = gameObject.AddComponent<HealthSnapshotComponent>();

            // The snapshot component will automatically find and work with
            // HealthSystem, ShieldSystem, and StatusEffectSystem components
        }

        protected void OnDestroy()
        {
            // Clean up event listeners
            if (healthSystem != null && healthBar != null)
            {
                healthSystem.HealthChanged -= (current, max) =>
                {
                    healthBar.UpdateHealth(current, max);
                };
            }
        }
    }
}