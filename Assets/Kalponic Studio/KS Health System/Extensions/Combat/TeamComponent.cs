using UnityEngine;

namespace KalponicStudio.Health.Extensions.Combat
{
    /// <summary>
    /// Team component that defines an entity's team and faction membership.
    /// Used by DamageRouter and DamageRules for team-based combat mechanics.
    /// Supports both numeric team IDs and string-based faction IDs.
    /// </summary>
    public class TeamComponent : MonoBehaviour
    {
        [Header("Team")]
        [Tooltip("Numeric team identifier. Entities with same team ID are considered allies.")]
        [SerializeField] private int teamId = 0;

        [Tooltip("String-based faction identifier. Entities with same faction ID are considered allies.")]
        [SerializeField] private string factionId = "";

        [Tooltip("Whether this entity allows friendly fire from its own team/faction.")]
        [SerializeField] private bool allowFriendlyFire = false;

        public int TeamId => teamId;
        public string FactionId => factionId;
        public bool AllowFriendlyFire => allowFriendlyFire;

        public DamageSourceInfo ToDamageSource(string sourceTag = null)
        {
            return new DamageSourceInfo
            {
                Source = gameObject,
                TeamId = teamId,
                FactionId = factionId,
                AllowFriendlyFire = allowFriendlyFire,
                SourceTag = string.IsNullOrWhiteSpace(sourceTag) ? gameObject.tag : sourceTag
            };
        }
    }
}
