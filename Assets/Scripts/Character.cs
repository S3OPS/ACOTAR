using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Represents the seven High Fae Courts of Prythian
    /// Based on ACOTAR lore
    /// </summary>
    public enum Court
    {
        Spring,
        Summer,
        Autumn,
        Winter,
        Night,
        Dawn,
        Day
    }

    /// <summary>
    /// Character classes based on ACOTAR lore
    /// </summary>
    public enum CharacterClass
    {
        HighFae,
        LesserFae,
        Human,
        Illyrian,
        Attor,
        Suriel
    }

    /// <summary>
    /// Magic abilities from ACOTAR
    /// </summary>
    public enum MagicType
    {
        Shapeshifting,
        Winnowing,
        DarknessManipulation,
        LightManipulation,
        FireManipulation,
        WaterManipulation,
        WindManipulation,
        IceManipulation,
        Healing,
        ShieldCreation,
        Daemati,      // Mind reading/manipulation
        Seer          // Prophetic visions
    }

    /// <summary>
    /// Game character with ACOTAR attributes
    /// Refactored to use modular systems for better organization
    /// </summary>
    [System.Serializable]
    public class Character
    {
        public string name;
        public CharacterClass characterClass;
        public Court allegiance;
        public bool isFae;
        public bool isMadeByTheCauldron;

        // Modular systems
        private CharacterStats stats;
        private AbilitySystem abilitySystem;

        // Property accessors for stats (maintains compatibility)
        public int health { get { return stats.health; } set { stats.health = value; } }
        public int maxHealth { get { return stats.maxHealth; } set { stats.maxHealth = value; } }
        public int magicPower { get { return stats.magicPower; } set { stats.magicPower = value; } }
        public int strength { get { return stats.strength; } set { stats.strength = value; } }
        public int agility { get { return stats.agility; } set { stats.agility = value; } }
        public int experience { get { return stats.experience; } set { stats.experience = value; } }
        public int level { get { return stats.level; } set { stats.level = value; } }
        public List<MagicType> abilities { get { return abilitySystem.GetAbilities(); } }

        public Character(string name, CharacterClass charClass, Court court)
        {
            this.name = name;
            this.characterClass = charClass;
            this.allegiance = court;
            this.isFae = charClass != CharacterClass.Human;
            this.isMadeByTheCauldron = false;
            
            // Initialize modular systems
            stats = new CharacterStats();
            stats.InitializeForClass(charClass);
            abilitySystem = new AbilitySystem(charClass);
            
            GameEvents.TriggerCharacterCreated(this);
        }

        /// <summary>
        /// Learn a new magic ability
        /// </summary>
        public void LearnAbility(MagicType ability)
        {
            if (abilitySystem.LearnAbility(ability))
            {
                GameEvents.TriggerAbilityLearned(this, ability);
            }
        }

        /// <summary>
        /// Check if character has a specific ability
        /// </summary>
        public bool HasAbility(MagicType ability)
        {
            return abilitySystem.HasAbility(ability);
        }

        /// <summary>
        /// Take damage with event notification
        /// </summary>
        public void TakeDamage(int damage)
        {
            stats.TakeDamage(damage);
            GameEvents.TriggerCharacterTakeDamage(this, damage);
        }

        /// <summary>
        /// Heal with event notification
        /// </summary>
        public void Heal(int amount)
        {
            stats.Heal(amount);
            GameEvents.TriggerCharacterHealed(this, amount);
        }

        /// <summary>
        /// Check if character is alive
        /// </summary>
        public bool IsAlive()
        {
            return stats.IsAlive();
        }

        /// <summary>
        /// Gain experience with level up notification
        /// </summary>
        public void GainExperience(int xp)
        {
            int oldLevel = stats.level;
            if (stats.GainExperience(xp))
            {
                GameEvents.TriggerCharacterLevelUp(this, stats.level);
            }
        }

        /// <summary>
        /// Get XP required for next level
        /// </summary>
        public int GetXPRequiredForNextLevel()
        {
            return stats.GetXPRequiredForNextLevel();
        }
    }
}
