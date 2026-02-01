using System;
using UnityEngine;
using KalponicStudio.Health;

namespace KalponicStudio.Health.Extensions.Combat
{
    /// <summary>
    /// Damage router that handles team-based damage rules and damage application.
    /// Checks team relationships before applying damage and routes damage through
    /// the appropriate systems. Useful for multiplayer games with team mechanics.
    /// </summary>
    [RequireComponent(typeof(HealthSystem))]
    public class DamageRouter : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("HealthSystem component to apply damage to. Auto-finds if not set.")]
        [SerializeField] private HealthSystem healthSystem;

        [Tooltip("TeamComponent defining this entity's team. Auto-finds if not set.")]
        [SerializeField] private TeamComponent ownerTeam;

        [Header("Rules")]
        [Tooltip("Whether to check team rules before applying damage.")]
        [SerializeField] private bool useTeamRules = true;

        public event Action<DamageInfo, DamageSourceInfo> DamageApplied;

        public DamageInfo LastDamageInfo { get; private set; }
        public DamageSourceInfo LastDamageSource { get; private set; }
        public float LastDamageTime { get; private set; }

        private void Awake()
        {
            if (healthSystem == null)
            {
                healthSystem = GetComponent<HealthSystem>();
            }

            if (ownerTeam == null)
            {
                ownerTeam = GetComponent<TeamComponent>();
            }
        }

        public bool ApplyDamage(DamageInfo damageInfo, DamageSourceInfo sourceInfo)
        {
            if (healthSystem == null) return false;

            if (useTeamRules && ownerTeam != null && sourceInfo.IsValid)
            {
                DamageSourceInfo owner = ownerTeam.ToDamageSource();
                if (!DamageRules.CanDamage(sourceInfo, owner))
                {
                    return false;
                }
            }

            sourceInfo.ApplyTo(ref damageInfo);
            healthSystem.TakeDamage(damageInfo);
            TrackDamage(damageInfo, sourceInfo);
            return true;
        }

        private void TrackDamage(DamageInfo damageInfo, DamageSourceInfo sourceInfo)
        {
            LastDamageInfo = damageInfo;
            LastDamageSource = sourceInfo;
            LastDamageTime = Time.time;
            DamageApplied?.Invoke(damageInfo, sourceInfo);
        }
    }
}
