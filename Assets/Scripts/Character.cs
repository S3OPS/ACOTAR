using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Represents the seven High Fae Courts of Prythian plus neutral option
    /// Based on ACOTAR lore
    /// </summary>
    public enum Court
    {
        None,       // No court allegiance (mortals, outcasts)
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
        Daemati,            // Mind reading/manipulation
        Seer,               // Prophetic visions
        Shadowsinger,       // Azriel's unique shadow ability
        TruthTelling,       // Mor's ability to detect and compel truth
        DeathManifestation, // Nesta's power taken from the Cauldron
        MatingBond          // Ability to sense/strengthen mate connection
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
        private CharacterStats _stats;
        private AbilitySystem _abilitySystem;

        // Property accessors for stats (maintains compatibility)
        public int health { get { return _stats.health; } set { _stats.health = value; } }
        public int maxHealth { get { return _stats.maxHealth; } set { _stats.maxHealth = value; } }
        public int magicPower { get { return _stats.magicPower; } set { _stats.magicPower = value; } }
        public int strength { get { return _stats.strength; } set { _stats.strength = value; } }
        public int agility { get { return _stats.agility; } set { _stats.agility = value; } }
        public int experience { get { return _stats.experience; } set { _stats.experience = value; } }
        public int level { get { return _stats.level; } set { _stats.level = value; } }
        public List<MagicType> abilities { get { return _abilitySystem.GetAbilities(); } }

        // Alias properties for UI compatibility
        public string characterName { get { return name; } set { name = value; } }
        public Court courtAllegiance { get { return allegiance; } set { allegiance = value; } }
        public CharacterStats stats { get { return _stats; } }
        public AbilitySystem abilitySystem { get { return _abilitySystem; } }

        public Character(string name, CharacterClass charClass, Court court)
        {
            this.name = name;
            this.characterClass = charClass;
            this.allegiance = court;
            this.isFae = charClass != CharacterClass.Human;
            this.isMadeByTheCauldron = false;
            
            // Initialize modular systems
            _stats = new CharacterStats();
            _stats.InitializeForClass(charClass);
            _abilitySystem = new AbilitySystem(charClass);
            
            GameEvents.TriggerCharacterCreated(this);
        }

        /// <summary>
        /// Initialize stats for a character - called after character creation UI
        /// </summary>
        public void InitializeStats()
        {
            _stats = new CharacterStats();
            _stats.InitializeForClass(characterClass);
            _abilitySystem = new AbilitySystem(characterClass);
        }

        /// <summary>
        /// Learn a new magic ability
        /// </summary>
        public void LearnAbility(MagicType ability)
        {
            if (_abilitySystem.LearnAbility(ability))
            {
                GameEvents.TriggerAbilityLearned(this, ability);
            }
        }

        /// <summary>
        /// Check if character has a specific ability
        /// </summary>
        public bool HasAbility(MagicType ability)
        {
            return _abilitySystem.HasAbility(ability);
        }

        /// <summary>
        /// Take damage with event notification
        /// </summary>
        public void TakeDamage(int damage)
        {
            _stats.TakeDamage(damage);
            GameEvents.TriggerCharacterTakeDamage(this, damage);
        }

        /// <summary>
        /// Heal with event notification
        /// </summary>
        public void Heal(int amount)
        {
            _stats.Heal(amount);
            GameEvents.TriggerCharacterHealed(this, amount);
        }

        /// <summary>
        /// Check if character is alive
        /// </summary>
        public bool IsAlive()
        {
            return _stats.IsAlive();
        }

        /// <summary>
        /// Gain experience with level up notification
        /// </summary>
        public void GainExperience(int xp)
        {
            int oldLevel = _stats.level;
            if (_stats.GainExperience(xp))
            {
                GameEvents.TriggerCharacterLevelUp(this, _stats.level);
                
                // Trigger level up visual effect
                if (ScreenEffectsManager.Instance != null)
                {
                    ScreenEffectsManager.Instance.LevelUpFeedback();
                }
            }
        }

        /// <summary>
        /// Get XP required for next level
        /// </summary>
        public int GetXPRequiredForNextLevel()
        {
            return _stats.GetXPRequiredForNextLevel();
        }
    }
}
