using System;

namespace ACOTAR
{
    /// <summary>
    /// Encapsulates character statistics and stat-related operations
    /// Separated for better modularity and testability
    /// </summary>
    [Serializable]
    public class CharacterStats
    {
        public int health;
        public int maxHealth;
        public int magicPower;
        public int strength;
        public int agility;
        public int experience;
        public int level;

        // Property accessors for UI compatibility
        public int CurrentHealth { get { return health; } set { health = value; } }
        public int MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
        public int MaxMagicPower { get { return magicPower; } }
        public int Level { get { return level; } set { level = value; } }
        public int Experience { get { return experience; } set { experience = value; } }

        public CharacterStats()
        {
            level = 1;
            experience = 0;
        }

        /// <summary>
        /// Initialize stats based on character class
        /// Optimized with centralized configuration
        /// </summary>
        public void InitializeForClass(CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.HighFae:
                    maxHealth = GameConfig.ClassStats.HIGHFAE_HEALTH;
                    magicPower = GameConfig.ClassStats.HIGHFAE_MAGIC;
                    strength = GameConfig.ClassStats.HIGHFAE_STRENGTH;
                    agility = GameConfig.ClassStats.HIGHFAE_AGILITY;
                    break;
                case CharacterClass.Illyrian:
                    maxHealth = GameConfig.ClassStats.ILLYRIAN_HEALTH;
                    magicPower = GameConfig.ClassStats.ILLYRIAN_MAGIC;
                    strength = GameConfig.ClassStats.ILLYRIAN_STRENGTH;
                    agility = GameConfig.ClassStats.ILLYRIAN_AGILITY;
                    break;
                case CharacterClass.LesserFae:
                    maxHealth = GameConfig.ClassStats.LESSERFAE_HEALTH;
                    magicPower = GameConfig.ClassStats.LESSERFAE_MAGIC;
                    strength = GameConfig.ClassStats.LESSERFAE_STRENGTH;
                    agility = GameConfig.ClassStats.LESSERFAE_AGILITY;
                    break;
                case CharacterClass.Human:
                    maxHealth = GameConfig.ClassStats.HUMAN_HEALTH;
                    magicPower = GameConfig.ClassStats.HUMAN_MAGIC;
                    strength = GameConfig.ClassStats.HUMAN_STRENGTH;
                    agility = GameConfig.ClassStats.HUMAN_AGILITY;
                    break;
                case CharacterClass.Attor:
                    maxHealth = GameConfig.ClassStats.ATTOR_HEALTH;
                    magicPower = GameConfig.ClassStats.ATTOR_MAGIC;
                    strength = GameConfig.ClassStats.ATTOR_STRENGTH;
                    agility = GameConfig.ClassStats.ATTOR_AGILITY;
                    break;
                case CharacterClass.Suriel:
                    maxHealth = GameConfig.ClassStats.SURIEL_HEALTH;
                    magicPower = GameConfig.ClassStats.SURIEL_MAGIC;
                    strength = GameConfig.ClassStats.SURIEL_STRENGTH;
                    agility = GameConfig.ClassStats.SURIEL_AGILITY;
                    break;
            }
            health = maxHealth;
        }

        /// <summary>
        /// Apply damage with validation
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (damage < 0) damage = 0;
            health -= damage;
            if (health < 0) health = 0;
        }

        /// <summary>
        /// Apply healing with validation
        /// </summary>
        public void Heal(int amount)
        {
            if (amount < 0) amount = 0;
            health += amount;
            if (health > maxHealth) health = maxHealth;
        }

        /// <summary>
        /// Check if character is alive
        /// </summary>
        public bool IsAlive()
        {
            return health > 0;
        }

        /// <summary>
        /// Add experience and handle level ups
        /// Returns true if leveled up
        /// Improved with early game pacing (v2.3.1)
        /// </summary>
        public bool GainExperience(int xp)
        {
            if (xp < 0) return false;
            
            experience += xp;
            bool didLevelUp = false;
            
            int requiredXP = GetXPRequiredForNextLevel();
            while (experience >= requiredXP)
            {
                experience -= requiredXP;
                LevelUp();
                didLevelUp = true;
                requiredXP = GetXPRequiredForNextLevel();
            }
            
            return didLevelUp;
        }

        /// <summary>
        /// Increase stats on level up
        /// </summary>
        private void LevelUp()
        {
            level++;
            maxHealth += GameConfig.STAT_INCREASE_PER_LEVEL_HEALTH;
            health = maxHealth;
            magicPower += GameConfig.STAT_INCREASE_PER_LEVEL_MAGIC;
            strength += GameConfig.STAT_INCREASE_PER_LEVEL_STRENGTH;
            agility += GameConfig.STAT_INCREASE_PER_LEVEL_AGILITY;
        }

        /// <summary>
        /// Get XP required for next level with improved progression curve
        /// Early levels (1-3) require more XP for better pacing (v2.3.1)
        /// </summary>
        public int GetXPRequiredForNextLevel()
        {
            // Ensure level is at least 1 to avoid Math.Pow edge cases
            // (negative exponents would produce fractional results)
            int effectiveLevel = Math.Max(1, level);
            
            int baseXP = (int)(BalanceConfig.Progression.BASE_XP_PER_LEVEL * 
                               Math.Pow(BalanceConfig.Progression.XP_LEVEL_MULTIPLIER, effectiveLevel - 1));
            
            // Apply early game scaling for better pacing
            if (effectiveLevel <= BalanceConfig.Progression.EARLY_GAME_LEVEL_THRESHOLD)
            {
                baseXP = (int)(baseXP * BalanceConfig.Progression.EARLY_GAME_XP_SCALING);
            }
            
            return baseXP;
        }
    }
}
