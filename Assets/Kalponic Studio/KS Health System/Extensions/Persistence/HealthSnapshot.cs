using System;
using System.Collections.Generic;
using KalponicStudio.Health;
using UnityEngine;

namespace KalponicStudio.Health.Extensions.Persistence
{
    /// <summary>
    /// Enhanced health snapshot with comprehensive serialization support.
    /// Includes versioning, timestamps, and multiple serialization formats.
    /// </summary>
    [Serializable]
    public struct HealthSnapshot
    {
        // Versioning for forward/backward compatibility
        public int Version;
        public DateTime Timestamp;

        // Core health data
        public int MaxHealth;
        public int CurrentHealth;
        public bool IsDowned;
        public bool IsDead;
        public float DownedTimeRemaining;

        // Shield data
        public int MaxShield;
        public int CurrentShield;

        // Status effects with enhanced data
        public List<StatusEffectSnapshot> StatusEffects;

        // Additional state data
        public bool IsInvulnerable;
        public float InvulnerabilityTimeRemaining;
        public bool RegenerationEnabled;
        public float RegenerationRate;
        public float RegenerationDelay;
        public float FlatDamageReduction;
        public float PercentDamageReduction;
        public DamageResistanceSnapshot[] DamageResistances;

        // Metadata
        public string EntityName;
        public string EntityTag;
        public Vector3 Position;
        public Quaternion Rotation;

        /// <summary>
        /// Creates a snapshot with current timestamp and version
        /// </summary>
        public static HealthSnapshot Create()
        {
            return new HealthSnapshot
            {
                Version = 1,
                Timestamp = DateTime.UtcNow,
                StatusEffects = new List<StatusEffectSnapshot>(),
                DamageResistances = new DamageResistanceSnapshot[0]
            };
        }

        /// <summary>
        /// Validates the snapshot data integrity
        /// </summary>
        public bool IsValid()
        {
            if (Version < 1) return false;
            if (MaxHealth < 0 || CurrentHealth < 0) return false;
            if (CurrentHealth > MaxHealth) return false;
            if (MaxShield < 0 || CurrentShield < 0) return false;
            if (CurrentShield > MaxShield) return false;
            if (DownedTimeRemaining < 0) return false;
            if (InvulnerabilityTimeRemaining < 0) return false;

            return true;
        }

        /// <summary>
        /// Serializes to JSON string
        /// </summary>
        public string ToJson()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }

        /// <summary>
        /// Deserializes from JSON string
        /// </summary>
        public static HealthSnapshot FromJson(string json)
        {
            var snapshot = UnityEngine.JsonUtility.FromJson<HealthSnapshot>(json);

            // Validate and repair if needed
            if (!snapshot.IsValid())
            {
                UnityEngine.Debug.LogWarning("[HealthSnapshot] Invalid snapshot data detected, attempting repair...");
                snapshot.RepairInvalidData();
            }

            return snapshot;
        }

        /// <summary>
        /// Attempts to repair invalid snapshot data
        /// </summary>
        private void RepairInvalidData()
        {
            MaxHealth = Mathf.Max(1, MaxHealth);
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            MaxShield = Mathf.Max(0, MaxShield);
            CurrentShield = Mathf.Clamp(CurrentShield, 0, MaxShield);
            DownedTimeRemaining = Mathf.Max(0, DownedTimeRemaining);
            InvulnerabilityTimeRemaining = Mathf.Max(0, InvulnerabilityTimeRemaining);
            Version = Mathf.Max(1, Version);
        }
    }

    /// <summary>
    /// Serializable version of StatusEffect for snapshots
    /// </summary>
    [Serializable]
    public struct StatusEffectSnapshot
    {
        public string EffectName;
        public StatusEffectType EffectType;
        public float Duration;
        public float RemainingTime;
        public bool IsBuff;
        public int AmountPerTick;
        public float TickInterval;
        public float SpeedMultiplier;
        public StatusStackingMode StackingMode;
        public int MaxStacks;
        public int StackCount;

        public static StatusEffectSnapshot FromStatusEffect(StatusEffect effect)
        {
            return new StatusEffectSnapshot
            {
                EffectName = effect.effectName,
                EffectType = effect.effectType,
                Duration = effect.duration,
                RemainingTime = effect.remainingTime,
                IsBuff = effect.isBuff,
                AmountPerTick = effect.amountPerTick,
                TickInterval = effect.tickInterval,
                SpeedMultiplier = effect.speedMultiplier,
                StackingMode = effect.stackingMode,
                MaxStacks = effect.maxStacks,
                StackCount = effect.stackCount
            };
        }

        public StatusEffect ToStatusEffect()
        {
            var effect = new StatusEffect(EffectName, Duration, IsBuff, AmountPerTick, TickInterval, SpeedMultiplier)
            {
                effectType = EffectType,
                remainingTime = RemainingTime,
                stackingMode = StackingMode,
                maxStacks = MaxStacks,
                stackCount = StackCount
            };
            return effect;
        }
    }

    /// <summary>
    /// Serializable version of DamageResistance for snapshots
    /// </summary>
    [Serializable]
    public struct DamageResistanceSnapshot
    {
        public DamageType Type;
        public float Multiplier;

        public static DamageResistanceSnapshot FromDamageResistance(DamageResistance resistance)
        {
            return new DamageResistanceSnapshot
            {
                Type = resistance.type,
                Multiplier = resistance.multiplier
            };
        }

        public DamageResistance ToDamageResistance()
        {
            return new DamageResistance
            {
                type = Type,
                multiplier = Multiplier
            };
        }
    }
}
