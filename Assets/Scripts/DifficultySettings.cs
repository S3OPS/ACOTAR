using System;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Game difficulty levels
    /// </summary>
    public enum DifficultyLevel
    {
        Story,      // Easy mode - focus on narrative
        Normal,     // Balanced gameplay
        Hard,       // Challenging combat
        Nightmare   // Extreme difficulty for veterans
    }

    /// <summary>
    /// Manages game difficulty settings and their effects on gameplay
    /// </summary>
    public static class DifficultySettings
    {
        private static DifficultyLevel currentDifficulty = DifficultyLevel.Normal;

        /// <summary>
        /// Get current difficulty level
        /// </summary>
        public static DifficultyLevel CurrentDifficulty
        {
            get { return currentDifficulty; }
            set 
            { 
                currentDifficulty = value;
                Debug.Log($"Difficulty set to: {value}");
                OnDifficultyChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Event fired when difficulty changes
        /// </summary>
        public static event Action<DifficultyLevel> OnDifficultyChanged;

        /// <summary>
        /// Get enemy damage multiplier based on difficulty
        /// </summary>
        public static float GetEnemyDamageMultiplier()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return 0.5f;    // Enemies deal 50% damage
                case DifficultyLevel.Normal:
                    return 1.0f;    // Normal damage
                case DifficultyLevel.Hard:
                    return 1.5f;    // Enemies deal 150% damage
                case DifficultyLevel.Nightmare:
                    return 2.0f;    // Enemies deal 200% damage
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get enemy health multiplier based on difficulty
        /// </summary>
        public static float GetEnemyHealthMultiplier()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return 0.6f;    // Enemies have 60% health
                case DifficultyLevel.Normal:
                    return 1.0f;    // Normal health
                case DifficultyLevel.Hard:
                    return 1.4f;    // Enemies have 140% health
                case DifficultyLevel.Nightmare:
                    return 2.0f;    // Enemies have 200% health
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get player damage multiplier based on difficulty
        /// </summary>
        public static float GetPlayerDamageMultiplier()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return 1.5f;    // Player deals 150% damage
                case DifficultyLevel.Normal:
                    return 1.0f;    // Normal damage
                case DifficultyLevel.Hard:
                    return 0.9f;    // Player deals 90% damage
                case DifficultyLevel.Nightmare:
                    return 0.75f;   // Player deals 75% damage
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get XP gain multiplier based on difficulty
        /// </summary>
        public static float GetXPMultiplier()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return 1.5f;    // 150% XP gain
                case DifficultyLevel.Normal:
                    return 1.0f;    // Normal XP
                case DifficultyLevel.Hard:
                    return 1.25f;   // 125% XP gain (reward for difficulty)
                case DifficultyLevel.Nightmare:
                    return 1.5f;    // 150% XP gain (reward for difficulty)
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get gold drop multiplier based on difficulty
        /// </summary>
        public static float GetGoldMultiplier()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return 1.5f;    // 150% gold
                case DifficultyLevel.Normal:
                    return 1.0f;    // Normal gold
                case DifficultyLevel.Hard:
                    return 1.2f;    // 120% gold
                case DifficultyLevel.Nightmare:
                    return 1.5f;    // 150% gold
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get loot drop chance multiplier based on difficulty
        /// </summary>
        public static float GetLootChanceMultiplier()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return 1.3f;    // 130% drop chance
                case DifficultyLevel.Normal:
                    return 1.0f;    // Normal drop chance
                case DifficultyLevel.Hard:
                    return 1.1f;    // 110% drop chance
                case DifficultyLevel.Nightmare:
                    return 1.25f;   // 125% drop chance
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get flee success chance modifier
        /// </summary>
        public static float GetFleeChanceModifier()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return 0.3f;    // +30% flee chance
                case DifficultyLevel.Normal:
                    return 0.0f;    // Normal flee chance
                case DifficultyLevel.Hard:
                    return -0.15f;  // -15% flee chance
                case DifficultyLevel.Nightmare:
                    return -0.3f;   // -30% flee chance
                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Get critical hit chance modifier for enemies
        /// </summary>
        public static float GetEnemyCritChanceModifier()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return -0.1f;   // -10% enemy crit chance
                case DifficultyLevel.Normal:
                    return 0.0f;    // Normal crit chance
                case DifficultyLevel.Hard:
                    return 0.05f;   // +5% enemy crit chance
                case DifficultyLevel.Nightmare:
                    return 0.1f;    // +10% enemy crit chance
                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Check if tutorial hints should be shown
        /// </summary>
        public static bool ShowTutorialHints()
        {
            return currentDifficulty == DifficultyLevel.Story || 
                   currentDifficulty == DifficultyLevel.Normal;
        }

        /// <summary>
        /// Check if auto-heal between combats is enabled
        /// </summary>
        public static bool AutoHealBetweenCombats()
        {
            return currentDifficulty == DifficultyLevel.Story;
        }

        /// <summary>
        /// Get companion effectiveness multiplier
        /// </summary>
        public static float GetCompanionEffectiveness()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return 1.3f;    // Companions 30% more effective
                case DifficultyLevel.Normal:
                    return 1.0f;    // Normal effectiveness
                case DifficultyLevel.Hard:
                    return 0.9f;    // Companions 10% less effective
                case DifficultyLevel.Nightmare:
                    return 0.8f;    // Companions 20% less effective
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get a description of the current difficulty
        /// </summary>
        public static string GetDifficultyDescription()
        {
            switch (currentDifficulty)
            {
                case DifficultyLevel.Story:
                    return "Story Mode: Focus on the narrative. Enemies are weaker, you deal more damage, and gain more XP. Perfect for experiencing the ACOTAR story.";
                case DifficultyLevel.Normal:
                    return "Normal: Balanced gameplay experience. Recommended for most players.";
                case DifficultyLevel.Hard:
                    return "Hard: A challenging experience. Enemies are tougher and deal more damage. Better rewards for skilled players.";
                case DifficultyLevel.Nightmare:
                    return "Nightmare: Extreme difficulty. Only for veterans seeking the ultimate challenge. Highest rewards but punishing combat.";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Display current difficulty settings
        /// </summary>
        public static void DisplaySettings()
        {
            Debug.Log("\n=== Difficulty Settings ===");
            Debug.Log($"Current Difficulty: {currentDifficulty}");
            Debug.Log(GetDifficultyDescription());
            Debug.Log("\nMultipliers:");
            Debug.Log($"  Enemy Damage: {GetEnemyDamageMultiplier():P0}");
            Debug.Log($"  Enemy Health: {GetEnemyHealthMultiplier():P0}");
            Debug.Log($"  Player Damage: {GetPlayerDamageMultiplier():P0}");
            Debug.Log($"  XP Gain: {GetXPMultiplier():P0}");
            Debug.Log($"  Gold Drop: {GetGoldMultiplier():P0}");
            Debug.Log($"  Loot Chance: {GetLootChanceMultiplier():P0}");
            Debug.Log($"  Flee Modifier: {GetFleeChanceModifier():+0.0%;-0.0%;0%}");
            Debug.Log("===========================\n");
        }
    }
}
