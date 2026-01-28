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
    /// </summary>
    [System.Serializable]
    public class Character
    {
        public string name;
        public CharacterClass characterClass;
        public Court allegiance;
        public List<MagicType> abilities;
        public int health;
        public int maxHealth;
        public int magicPower;
        public int strength;
        public int agility;
        public bool isFae;
        public bool isMadeByTheCauldron;

        public Character(string name, CharacterClass charClass, Court court)
        {
            this.name = name;
            this.characterClass = charClass;
            this.allegiance = court;
            this.abilities = new List<MagicType>();
            this.isFae = charClass != CharacterClass.Human;
            this.isMadeByTheCauldron = false;
            
            // Set base stats based on class
            SetBaseStats();
        }

        private void SetBaseStats()
        {
            switch (characterClass)
            {
                case CharacterClass.HighFae:
                    maxHealth = 150;
                    magicPower = 100;
                    strength = 80;
                    agility = 70;
                    break;
                case CharacterClass.Illyrian:
                    maxHealth = 180;
                    magicPower = 60;
                    strength = 120;
                    agility = 90;
                    break;
                case CharacterClass.LesserFae:
                    maxHealth = 100;
                    magicPower = 60;
                    strength = 60;
                    agility = 80;
                    break;
                case CharacterClass.Human:
                    maxHealth = 80;
                    magicPower = 0;
                    strength = 50;
                    agility = 60;
                    break;
                case CharacterClass.Attor:
                    maxHealth = 120;
                    magicPower = 40;
                    strength = 90;
                    agility = 100;
                    break;
                case CharacterClass.Suriel:
                    maxHealth = 70;
                    magicPower = 150;
                    strength = 30;
                    agility = 40;
                    break;
            }
            health = maxHealth;
        }

        public void LearnAbility(MagicType ability)
        {
            if (!abilities.Contains(ability))
            {
                abilities.Add(ability);
            }
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health < 0) health = 0;
        }

        public void Heal(int amount)
        {
            health += amount;
            if (health > maxHealth) health = maxHealth;
        }

        public bool IsAlive()
        {
            return health > 0;
        }
    }
}
