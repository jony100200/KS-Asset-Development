using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using KalponicStudio.Health;

namespace KalponicStudio.Health
{
    /// <summary>
    /// Shield system that works with HealthSystem.
    /// Provides damage absorption and regeneration mechanics.
    /// Shields absorb damage before it reaches health.
    /// Now uses event channels for better decoupling.
    /// </summary>
    [RequireComponent(typeof(HealthSystem))]
    public class ShieldSystem : MonoBehaviour, IShieldComponent, IShieldAbsorber
    {
        [Header("Event Channels")]
        [Tooltip("Event channel for broadcasting shield-related events.")]
        [SerializeField] private HealthEventChannelSO healthEvents;

        [Header("Shield Settings")]
        [Tooltip("Maximum shield capacity this entity can have.")]
        [SerializeField] private int maxShield = 50;

        [Tooltip("Current shield value. Will be clamped between 0 and maxShield.")]
        [SerializeField] private int currentShield = 50;

        [Tooltip("How much shield to regenerate per second when regeneration is active.")]
        [SerializeField] private float shieldRegenerationRate = 5f;

        [Tooltip("Delay in seconds before shield regeneration starts after taking damage.")]
        [SerializeField] private float regenerationDelay = 3f;

        [Tooltip("Whether shield should recharge after taking damage.")]
        [SerializeField] private bool shieldRechargesAfterDamage = true;

        [Header("Visual Settings")]
        [Tooltip("SpriteRenderer to use for shield visual effects.")]
        [SerializeField] private SpriteRenderer shieldRenderer;

        [Tooltip("Color of the shield when active and has capacity.")]
        [SerializeField] private Color shieldActiveColor = Color.blue;

        [Tooltip("Color of the shield when depleted.")]
        [SerializeField] private Color shieldDepletedColor = Color.gray;

        [Header("Events")]
        [Tooltip("UnityEvent triggered when shield changes. Parameters: (currentShield, maxShield)")]
        [SerializeField] private UnityEvent<int, int> onShieldChanged = new UnityEvent<int, int>();

        [Tooltip("UnityEvent triggered when shield absorbs damage. Parameter: damage absorbed")]
        [SerializeField] private UnityEvent<int> onShieldAbsorbed = new UnityEvent<int>();

        [Tooltip("UnityEvent triggered when shield is completely depleted.")]
        [SerializeField] private UnityEvent onShieldDepleted = new UnityEvent();

        [Tooltip("UnityEvent triggered when shield is restored or regenerated.")]
        [SerializeField] private UnityEvent onShieldRestored = new UnityEvent();

        public event Action<int, int> ShieldChanged;
        public event Action<int> ShieldAbsorbed;
        public event Action ShieldDepleted;
        public event Action ShieldRestored;

        // Public properties
        public int MaxShield => maxShield;
        public int CurrentShield => currentShield;
        public bool HasShield => currentShield > 0;

        // Private fields
        private float regenerationTimer = 0f;
        private bool shieldActive = true;

        private void Awake()
        {
            currentShield = Mathf.Clamp(currentShield, 0, maxShield);

            // Setup shield visual
            if (shieldRenderer == null)
            {
                shieldRenderer = GetComponent<SpriteRenderer>();
            }

            UpdateShieldVisual();
            RaiseShieldChanged(currentShield, maxShield);
        }

        private void Update()
        {
            HandleShieldRegeneration();
        }

        private void HandleShieldRegeneration()
        {
            if (!shieldRechargesAfterDamage || !shieldActive || currentShield >= maxShield) return;

            // Check if enough time has passed since last damage
            if (regenerationTimer > 0)
            {
                regenerationTimer -= Time.deltaTime;
                return;
            }

            // Regenerate shield
            if (currentShield < maxShield)
            {
                int oldShield = currentShield;
                currentShield = Mathf.Min(maxShield, currentShield + Mathf.RoundToInt(shieldRegenerationRate * Time.deltaTime));

                if (currentShield != oldShield)
                {
                    RaiseShieldChanged(currentShield, maxShield);
                    UpdateShieldVisual();
                }
            }
        }

        /// <summary>
        /// Absorb incoming damage and return remaining damage (if any)
        /// </summary>
        public int AbsorbDamage(int damage)
        {
            return ApplyShieldDamage(damage);
        }

        /// <summary>
        /// Manually damage the shield
        /// </summary>
        public void DamageShield(int damage)
        {
            ApplyShieldDamage(damage);
        }

        /// <summary>
        /// Restore shield points
        /// </summary>
        public void RestoreShield(int amount)
        {
            if (amount <= 0) return;

            int oldShield = currentShield;
            currentShield = Mathf.Min(maxShield, currentShield + amount);

            if (currentShield != oldShield)
            {
                RaiseShieldChanged(currentShield, maxShield);
                UpdateShieldVisual();

                if (oldShield <= 0 && currentShield > 0)
                {
                    RaiseShieldRestored();
                }
            }
        }

        /// <summary>
        /// Set shield to specific value
        /// </summary>
        public void SetShield(int newShield)
        {
            int oldShield = currentShield;
            currentShield = Mathf.Clamp(newShield, 0, maxShield);

            if (currentShield != oldShield)
            {
                RaiseShieldChanged(currentShield, maxShield);
                UpdateShieldVisual();

                if (oldShield <= 0 && currentShield > 0)
                {
                    RaiseShieldRestored();
                }
                else if (currentShield <= 0 && oldShield > 0)
                {
                    RaiseShieldDepleted();
                    if (healthEvents != null)
                    {
                        healthEvents.RaiseShieldDepleted();
                    }
                }
            }
        }

        /// <summary>
        /// Fully restore shield
        /// </summary>
        public void RestoreFullShield()
        {
            SetShield(maxShield);
        }

        /// <summary>
        /// Deplete shield completely
        /// </summary>
        public void DepleteShield()
        {
            SetShield(0);
        }

        /// <summary>
        /// Enable/disable shield
        /// </summary>
        public void SetShieldActive(bool active)
        {
            shieldActive = active;
            UpdateShieldVisual();
        }

        public void SetMaxShield(int newMaxShield)
        {
            maxShield = Mathf.Max(1, newMaxShield);
            currentShield = Mathf.Min(currentShield, maxShield);
            RaiseShieldChanged(currentShield, maxShield);
            UpdateShieldVisual();
        }

        public void ConfigureRegeneration(float rate, float delay, bool rechargeAfterDamage)
        {
            shieldRegenerationRate = Mathf.Max(0f, rate);
            regenerationDelay = Mathf.Max(0f, delay);
            shieldRechargesAfterDamage = rechargeAfterDamage;
        }

        private void UpdateShieldVisual()
        {
            if (shieldRenderer == null) return;

            shieldRenderer.enabled = shieldActive && currentShield > 0;

            if (shieldRenderer.enabled)
            {
                // Color based on shield strength
                float healthRatio = maxShield > 0 ? (float)currentShield / maxShield : 0f;
                shieldRenderer.color = Color.Lerp(shieldDepletedColor, shieldActiveColor, healthRatio);
            }
        }

        // Unity event helpers for easy inspector setup
        public void DamageShieldFromInt(int damage) => DamageShield(damage);
        public void RestoreShieldFromInt(int amount) => RestoreShield(amount);
        public void SetShieldFromInt(int shield) => SetShield(shield);
        public void SetMaxShieldFromInt(int maxShield) => SetMaxShield(maxShield);

        private int ApplyShieldDamage(int damage)
        {
            if (!shieldActive || damage <= 0 || currentShield <= 0) return damage;

            int damageAbsorbed = Mathf.Min(damage, currentShield);
            int remainingDamage = damage - damageAbsorbed;
            int oldShield = currentShield;
            currentShield = Mathf.Max(0, currentShield - damageAbsorbed);

            RaiseShieldChanged(currentShield, maxShield);
            RaiseShieldAbsorbed(damageAbsorbed);
            if (healthEvents != null)
            {
                healthEvents.RaiseShieldAbsorbed(damageAbsorbed);
            }

            regenerationTimer = regenerationDelay;

            if (currentShield <= 0 && oldShield > 0)
            {
                RaiseShieldDepleted();
                if (healthEvents != null)
                {
                    healthEvents.RaiseShieldDepleted();
                }
            }

            UpdateShieldVisual();
            return remainingDamage;
        }

        private void RaiseShieldChanged(int current, int max)
        {
            ShieldChanged?.Invoke(current, max);
            onShieldChanged?.Invoke(current, max);
        }

        private void RaiseShieldAbsorbed(int damage)
        {
            ShieldAbsorbed?.Invoke(damage);
            onShieldAbsorbed?.Invoke(damage);
        }

        private void RaiseShieldDepleted()
        {
            ShieldDepleted?.Invoke();
            onShieldDepleted?.Invoke();
        }

        private void RaiseShieldRestored()
        {
            ShieldRestored?.Invoke();
            onShieldRestored?.Invoke();
        }
    }
}
