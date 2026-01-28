using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Manages character abilities and magic system
    /// Modularized for better organization and extensibility
    /// </summary>
    public class AbilitySystem
    {
        private List<MagicType> abilities;
        private CharacterClass characterClass;

        public AbilitySystem(CharacterClass charClass)
        {
            abilities = new List<MagicType>();
            characterClass = charClass;
        }

        /// <summary>
        /// Learn a new ability if not already known
        /// Returns true if ability was learned
        /// </summary>
        public bool LearnAbility(MagicType ability)
        {
            if (abilities.Contains(ability))
            {
                return false;
            }

            // Validate ability against character class
            if (!CanLearnAbility(ability))
            {
                Debug.LogWarning($"Character class {characterClass} cannot learn {ability}");
                return false;
            }

            abilities.Add(ability);
            return true;
        }

        /// <summary>
        /// Check if character has a specific ability
        /// </summary>
        public bool HasAbility(MagicType ability)
        {
            return abilities.Contains(ability);
        }

        /// <summary>
        /// Get all learned abilities
        /// </summary>
        public List<MagicType> GetAbilities()
        {
            return new List<MagicType>(abilities);
        }

        /// <summary>
        /// Validate if a character class can learn an ability
        /// Optimized with early returns
        /// </summary>
        private bool CanLearnAbility(MagicType ability)
        {
            // Humans cannot learn magic (unless Made)
            if (characterClass == CharacterClass.Human)
            {
                return false;
            }

            // Suriel can only learn Seer ability
            if (characterClass == CharacterClass.Suriel)
            {
                return ability == MagicType.Seer;
            }

            // All other Fae classes can learn most abilities
            return true;
        }

        /// <summary>
        /// Remove an ability (for game mechanics like curse effects)
        /// </summary>
        public bool ForgetAbility(MagicType ability)
        {
            return abilities.Remove(ability);
        }

        /// <summary>
        /// Clear all abilities (for transformation effects)
        /// </summary>
        public void ClearAbilities()
        {
            abilities.Clear();
        }

        /// <summary>
        /// Get count of learned abilities
        /// </summary>
        public int GetAbilityCount()
        {
            return abilities.Count;
        }
    }
}
