using UnityEngine;

namespace KalponicStudio.Health.Extensions.Combat
{
    /// <summary>
    /// Static utility class for team-based damage rules and relationships.
    /// Determines whether entities can damage each other based on team and faction membership.
    /// Supports friendly fire settings and complex team dynamics.
    /// </summary>
    public static class DamageRules
    {
        /// <summary>
        /// Checks if two damage sources are considered friendly (same team or faction).
        /// </summary>
        /// <param name="attacker">The attacking entity's damage source info</param>
        /// <param name="target">The target entity's damage source info</param>
        /// <returns>True if entities are friendly, false otherwise</returns>
        public static bool IsFriendly(DamageSourceInfo attacker, DamageSourceInfo target)
        {
            if (!attacker.IsValid || !target.IsValid) return false;

            if (attacker.TeamId != 0 && target.TeamId != 0 && attacker.TeamId == target.TeamId)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(attacker.FactionId) &&
                !string.IsNullOrWhiteSpace(target.FactionId) &&
                attacker.FactionId == target.FactionId)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if damage can be applied from attacker to target.
        /// Respects team relationships and friendly fire settings.
        /// </summary>
        /// <param name="attacker">The attacking entity's damage source info</param>
        /// <param name="target">The target entity's damage source info</param>
        /// <returns>True if damage is allowed, false if blocked by team rules</returns>
        public static bool CanDamage(DamageSourceInfo attacker, DamageSourceInfo target)
        {
            if (!attacker.IsValid || !target.IsValid) return true;

            if (IsFriendly(attacker, target))
            {
                return attacker.AllowFriendlyFire || target.AllowFriendlyFire;
            }

            return true;
        }

        /// <summary>
        /// Creates damage source info from a GameObject with TeamComponent.
        /// </summary>
        /// <param name="obj">GameObject to extract team information from</param>
        /// <returns>DamageSourceInfo for the GameObject, or default if no TeamComponent found</returns>
        public static DamageSourceInfo FromGameObject(GameObject obj)
        {
            if (obj == null) return default;

            TeamComponent team = obj.GetComponent<TeamComponent>();
            if (team == null) return default;

            return team.ToDamageSource();
        }
    }
}
