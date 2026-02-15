using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Centralized balance configuration for game tuning
    /// All combat, progression, and economy values in one place
    /// Version 2.0 - Phase 8 Balance Pass
    /// </summary>
    public static class BalanceConfig
    {
        // Version tracking
        public const string BALANCE_VERSION = "2.3.3";
        public const string LAST_UPDATED = "2026-02-14";

        #region Combat Balance

        /// <summary>
        /// Combat mechanics balance values
        /// </summary>
        public static class Combat
        {
            // Critical Hit System
            public const float CRITICAL_HIT_CHANCE = 0.15f;  // 15% base chance
            public const float CRITICAL_HIT_MULTIPLIER = 2.0f;  // 2x damage
            public const float AGILITY_CRIT_BONUS = 0.001f;  // +0.1% per agility point

            // Dodge System
            public const float BASE_DODGE_CHANCE = 0.05f;  // 5% base dodge
            public const float AGILITY_DODGE_BONUS = 0.01f;  // 1% per agility point
            public const float MAX_DODGE_CHANCE = 0.75f;  // Cap at 75%

            // Defend Action
            public const float DEFEND_DAMAGE_REDUCTION = 0.50f;  // 50% damage reduction
            public const float DEFEND_DURATION_TURNS = 1;  // Lasts 1 turn

            // Flee System
            public const float BASE_FLEE_CHANCE = 0.30f;  // 30% base
            public const float AGILITY_FLEE_BONUS = 0.01f;  // 1% per agility
            public const float LEVEL_FLEE_PENALTY = 0.05f;  // -5% per enemy level advantage

            // Damage Variance
            public const float MIN_DAMAGE_MULTIPLIER = 0.85f;  // 85% of base
            public const float MAX_DAMAGE_MULTIPLIER = 1.15f;  // 115% of base

            // Magic Damage Multipliers
            public const float ELEMENTAL_MAGIC_MULTIPLIER = 1.5f;
            public const float DAEMATI_MAGIC_MULTIPLIER = 2.0f;
            public const float HEALING_MAGIC_MULTIPLIER = 1.2f;

            // Turn-based
            public const int MAX_COMBAT_TURNS = 50;  // Prevent infinite battles
            public const float INITIATIVE_AGILITY_WEIGHT = 1.5f;
            
            // Combo System (v2.3.2)
            public const float COMBO_DAMAGE_BONUS_PER_HIT = 0.10f;  // 10% bonus per consecutive hit
            public const int COMBO_MAX_HITS = 5;  // Maximum combo multiplier (50% bonus at 5 hits)
            public const int COMBO_DODGE_TOLERANCE = 1;  // Combo survives 1 dodge before resetting
        }

        #endregion

        #region Class Balance

        /// <summary>
        /// Character class balance adjustments
        /// </summary>
        public static class Classes
        {
            // High Fae (Balanced)
            public const int HIGHFAE_HEALTH = 150;
            public const int HIGHFAE_MAGIC = 100;
            public const int HIGHFAE_STRENGTH = 80;
            public const int HIGHFAE_AGILITY = 70;

            // Illyrian (Warrior)
            public const int ILLYRIAN_HEALTH = 180;
            public const int ILLYRIAN_MAGIC = 60;
            public const int ILLYRIAN_STRENGTH = 120;
            public const int ILLYRIAN_AGILITY = 90;

            // Lesser Fae (Agile Caster)
            public const int LESSERFAE_HEALTH = 100;
            public const int LESSERFAE_MAGIC = 80;
            public const int LESSERFAE_STRENGTH = 60;
            public const int LESSERFAE_AGILITY = 100;

            // Human (Weak Start, High Growth)
            public const int HUMAN_HEALTH = 80;
            public const int HUMAN_MAGIC = 20;  // v2.3.2: Increased from 0 to enable basic abilities
            public const int HUMAN_STRENGTH = 50;
            public const int HUMAN_AGILITY = 60;
            public const float HUMAN_GROWTH_MULTIPLIER = 1.3f;  // 30% bonus growth

            // Attor (Fast Attacker)
            public const int ATTOR_HEALTH = 120;
            public const int ATTOR_MAGIC = 40;
            public const int ATTOR_STRENGTH = 90;
            public const int ATTOR_AGILITY = 110;

            // Suriel (Pure Caster)
            public const int SURIEL_HEALTH = 100;  // v2.3.2: Increased from 70 for better survivability
            public const int SURIEL_MAGIC = 150;
            public const int SURIEL_STRENGTH = 30;
            public const int SURIEL_AGILITY = 40;
        }

        #endregion

        #region Enemy Balance

        /// <summary>
        /// Enemy difficulty scaling
        /// </summary>
        public static class Enemies
        {
            // Difficulty multipliers (HP, Damage, XP, Gold)
            public static readonly float[] TRIVIAL_MULTIPLIERS = { 0.5f, 0.5f, 1.0f, 0.8f };
            public static readonly float[] EASY_MULTIPLIERS = { 0.75f, 0.75f, 1.2f, 1.0f };
            public static readonly float[] NORMAL_MULTIPLIERS = { 1.0f, 1.0f, 1.5f, 1.2f };
            public static readonly float[] HARD_MULTIPLIERS = { 1.5f, 1.3f, 2.0f, 1.5f };
            public static readonly float[] ELITE_MULTIPLIERS = { 2.0f, 1.5f, 3.0f, 2.0f };
            public static readonly float[] BOSS_MULTIPLIERS = { 3.0f, 2.0f, 5.0f, 3.0f };

            // XP Rewards by difficulty
            public const int TRIVIAL_XP = 25;
            public const int EASY_XP = 50;
            public const int NORMAL_XP = 100;
            public const int HARD_XP = 200;
            public const int ELITE_XP = 400;
            public const int BOSS_XP = 1000;

            // Gold Rewards by difficulty
            public const int TRIVIAL_GOLD = 10;
            public const int EASY_GOLD = 25;
            public const int NORMAL_GOLD = 50;
            public const int HARD_GOLD = 100;
            public const int ELITE_GOLD = 200;
            public const int BOSS_GOLD = 500;

            // Loot drop chances
            public const float TRIVIAL_DROP_CHANCE = 0.20f;  // 20%
            public const float EASY_DROP_CHANCE = 0.30f;
            public const float NORMAL_DROP_CHANCE = 0.50f;
            public const float HARD_DROP_CHANCE = 0.70f;
            public const float ELITE_DROP_CHANCE = 0.85f;
            public const float BOSS_DROP_CHANCE = 1.0f;  // 100%
            
            // Book 1 Boss Scaling (v2.3.2) - Make story bosses progressively harder
            // Standard bosses use BOSS_MULTIPLIERS above
            // Named story bosses get additional scaling based on difficulty mode
            // Format: [HP multiplier, Damage multiplier, XP multiplier, Gold multiplier]
            
            // Middengard Wyrm - First Trial Boss (Level 6-7)
            public static readonly float[] MIDDENGARD_WYRM_MULTIPLIERS = { 3.0f, 2.0f, 5.0f, 3.0f };  // Standard boss
            
            // Naga - Second Trial Boss (Level 7-8)
            public static readonly float[] NAGA_MULTIPLIERS = { 3.2f, 2.1f, 5.0f, 3.0f };  // Slightly harder (+7%)
            
            // Amarantha - Final Boss (Level 9-10)
            // Base values for Normal mode
            public static readonly float[] AMARANTHA_MULTIPLIERS = { 4.0f, 2.5f, 6.0f, 4.0f };  // Significantly harder (+33%)
            
            /// <summary>
            /// Get Amarantha boss multipliers adjusted for difficulty mode (v2.3.2)
            /// Makes final boss appropriately challenging for each difficulty
            /// </summary>
            public static float[] GetAmaranthaBossMultipliers(DifficultyLevel difficulty)
            {
                switch (difficulty)
                {
                    case DifficultyLevel.Story:
                        return new float[] { 2.5f, 1.5f, 6.0f, 4.0f };  // Easier final boss for story mode
                    case DifficultyLevel.Normal:
                        return AMARANTHA_MULTIPLIERS;  // Standard challenge
                    case DifficultyLevel.Hard:
                        return new float[] { 5.0f, 3.0f, 7.0f, 5.0f };  // Tougher for hard mode
                    case DifficultyLevel.Nightmare:
                        return new float[] { 6.0f, 3.5f, 8.0f, 6.0f };  // Ultimate challenge
                    default:
                        return AMARANTHA_MULTIPLIERS;
                }
            }
        }

        #endregion

        #region Progression Balance

        /// <summary>
        /// Character progression balance
        /// </summary>
        public static class Progression
        {
            // Level up requirements
            public const int BASE_XP_PER_LEVEL = 100;
            public const float XP_LEVEL_MULTIPLIER = 1.15f;  // 15% increase per level
            
            // Early game progression smoothing (v2.3.2 - revised)
            // Changed from 1.2f to 0.7f to REDUCE early grind and improve new player experience
            public const float EARLY_GAME_XP_SCALING = 0.7f;  // Levels 1-5 require 30% LESS XP
            public const int EARLY_GAME_LEVEL_THRESHOLD = 5;  // Extended to cover more early content

            // Stat growth per level
            public const int HEALTH_PER_LEVEL = 10;
            public const int MAGIC_PER_LEVEL = 5;
            public const int STRENGTH_PER_LEVEL = 3;
            public const int AGILITY_PER_LEVEL = 3;

            // Max level caps
            public const int BOOK1_MAX_LEVEL = 10;
            public const int BOOK2_MAX_LEVEL = 20;
            public const int BOOK3_MAX_LEVEL = 30;

            // Expected level progression
            public const int EXPECTED_LEVEL_CALANMAI = 3;
            public const int EXPECTED_LEVEL_UNDER_MOUNTAIN = 5;
            public const int EXPECTED_LEVEL_FIRST_TRIAL = 6;
            public const int EXPECTED_LEVEL_FINAL_TRIAL = 9;
            public const int EXPECTED_LEVEL_AMARANTHA = 10;
            
            // Story Milestone XP Bonuses (v2.3.2)
            // Reward players for reaching major story points
            public const int MILESTONE_CALANMAI_XP = 150;
            public const int MILESTONE_UNDER_MOUNTAIN_XP = 200;
            public const int MILESTONE_FIRST_TRIAL_XP = 250;
            public const int MILESTONE_FINAL_TRIAL_XP = 300;
            public const int MILESTONE_BREAK_CURSE_XP = 500;
        }

        #endregion

        #region Quest Balance

        /// <summary>
        /// Quest reward balance
        /// </summary>
        public static class Quests
        {
            // XP rewards by quest type
            public const int MINOR_QUEST_XP = 100;
            public const int STANDARD_QUEST_XP = 250;
            public const int MAJOR_QUEST_XP = 500;
            public const int CLIMAX_QUEST_XP = 1000;
            public const int BOOK_FINALE_XP = 1500;

            // Gold rewards
            public const int MINOR_QUEST_GOLD = 50;
            public const int STANDARD_QUEST_GOLD = 100;
            public const int MAJOR_QUEST_GOLD = 250;
            public const int CLIMAX_QUEST_GOLD = 500;

            // Reputation rewards
            public const int MINOR_REP_REWARD = 5;
            public const int STANDARD_REP_REWARD = 10;
            public const int MAJOR_REP_REWARD = 20;
            public const int CLIMAX_REP_REWARD = 50;
        }

        #endregion

        #region Economy Balance

        /// <summary>
        /// Economy and pricing balance
        /// </summary>
        public static class Economy
        {
            // Shop price modifiers by reputation
            public static readonly float[] REPUTATION_PRICE_MODIFIERS = 
            {
                1.50f,  // Hostile: +50%
                1.25f,  // Unfriendly: +25%
                1.00f,  // Neutral: Normal
                0.90f,  // Friendly: -10%
                0.80f,  // Honored: -20%
                0.70f,  // Revered: -30%
                0.50f   // Exalted: -50%
            };

            // Item value by rarity
            public const int COMMON_BASE_VALUE = 10;
            public const int UNCOMMON_BASE_VALUE = 50;
            public const int RARE_BASE_VALUE = 200;
            public const int EPIC_BASE_VALUE = 1000;
            public const int LEGENDARY_BASE_VALUE = 5000;
            public const int ARTIFACT_BASE_VALUE = 20000;

            // Crafting costs (% of item value)
            public const float CRAFTING_MATERIAL_COST_PERCENT = 0.60f;  // 60% of value
            public const float CRAFTING_TIME_PER_VALUE = 0.01f;  // 1s per 100 value

            // Starting currency
            public const int STARTING_GOLD = 100;
            public const int STARTING_FAE_CRYSTALS = 0;
        }

        #endregion

        #region Difficulty Settings

        /// <summary>
        /// Difficulty mode balance
        /// </summary>
        public static class Difficulty
        {
            // Story Mode (Easy)
            public static readonly float[] STORY_MULTIPLIERS = 
            { 
                0.5f,   // Enemy HP
                0.5f,   // Enemy Damage
                1.5f,   // XP Gain
                1.5f    // Gold Gain
            };

            // Normal Mode (Balanced)
            public static readonly float[] NORMAL_MULTIPLIERS = 
            { 
                1.0f,   // Enemy HP
                1.0f,   // Enemy Damage
                1.0f,   // XP Gain
                1.0f    // Gold Gain
            };

            // Hard Mode (Challenging)
            public static readonly float[] HARD_MULTIPLIERS = 
            { 
                1.5f,   // Enemy HP
                1.3f,   // Enemy Damage
                1.2f,   // XP Gain
                1.2f    // Gold Gain
            };

            // Nightmare Mode (Expert)
            public static readonly float[] NIGHTMARE_MULTIPLIERS = 
            { 
                2.0f,   // Enemy HP
                1.5f,   // Enemy Damage
                1.5f,   // XP Gain
                1.5f    // Gold Gain
            };
        }

        #endregion

        #region Companion Balance

        /// <summary>
        /// Companion system balance
        /// </summary>
        public static class Companions
        {
            // Loyalty effects on combat
            public const float LOYALTY_MIN_EFFECTIVENESS = 0.80f;  // 80% at 0 loyalty
            public const float LOYALTY_MAX_EFFECTIVENESS = 1.20f;  // 120% at 100 loyalty

            // Loyalty gain/loss
            public const int LOYALTY_QUEST_COMPLETE = 10;
            public const int LOYALTY_DIALOGUE_CHOICE = 5;
            public const int LOYALTY_IN_PARTY = 1;  // Per quest with them
            public const int LOYALTY_IGNORED = -5;  // Per major quest without them

            // Party limits
            public const int MAX_ACTIVE_COMPANIONS = 3;
            public const int MAX_RECRUITABLE_COMPANIONS = 9;
        }

        #endregion

        #region Time & Events

        /// <summary>
        /// Time system and moon phase effects
        /// </summary>
        public static class Time
        {
            // Day/Night cycle (in real-time seconds)
            public const float GAME_MINUTES_PER_REAL_SECOND = 1.0f;
            public const int MINUTES_PER_GAME_HOUR = 60;
            public const int HOURS_PER_GAME_DAY = 24;

            // Moon phase magic modifiers
            public const float FULL_MOON_MAGIC_BONUS = 0.30f;  // +30%
            public const float GIBBOUS_MOON_MAGIC_BONUS = 0.15f;  // +15%
            public const float NEW_MOON_MAGIC_PENALTY = -0.15f;  // -15%

            // Special event bonuses
            public const float CALANMAI_MAGIC_BONUS = 0.50f;  // +50% fire magic
            public const float STARFALL_XP_BONUS = 0.25f;  // +25% XP gain
        }

        #endregion

        #region Performance Settings

        /// <summary>
        /// Performance and optimization settings
        /// </summary>
        public static class Performance
        {
            // Object pooling
            public const int COMBAT_EFFECT_POOL_SIZE = 20;
            public const int UI_NOTIFICATION_POOL_SIZE = 10;

            // Update rates (frames between updates)
            public const int UI_UPDATE_FREQUENCY = 5;  // Every 5 frames
            public const int AMBIENT_UPDATE_FREQUENCY = 30;  // Every 30 frames

            // Cache lifetimes (seconds)
            public const float LOCATION_CACHE_LIFETIME = 60.0f;
            public const float QUEST_CACHE_LIFETIME = 30.0f;
        }

        #endregion

        /// <summary>
        /// Get difficulty multipliers for a specific difficulty level
        /// </summary>
        public static float[] GetDifficultyMultipliers(DifficultyLevel difficulty)
        {
            switch (difficulty)
            {
                case DifficultyLevel.Story:
                    return Difficulty.STORY_MULTIPLIERS;
                case DifficultyLevel.Normal:
                    return Difficulty.NORMAL_MULTIPLIERS;
                case DifficultyLevel.Hard:
                    return Difficulty.HARD_MULTIPLIERS;
                case DifficultyLevel.Nightmare:
                    return Difficulty.NIGHTMARE_MULTIPLIERS;
                default:
                    return Difficulty.NORMAL_MULTIPLIERS;
            }
        }

        /// <summary>
        /// Calculate XP required for a specific level
        /// </summary>
        public static int GetXPRequiredForLevel(int level)
        {
            if (level <= 1) return 0;
            
            float xp = Progression.BASE_XP_PER_LEVEL;
            for (int i = 2; i <= level; i++)
            {
                xp *= Progression.XP_LEVEL_MULTIPLIER;
            }
            return Mathf.RoundToInt(xp);
        }

        /// <summary>
        /// Get reputation price modifier
        /// </summary>
        public static float GetReputationPriceModifier(int reputationLevel)
        {
            reputationLevel = Mathf.Clamp(reputationLevel, 0, Economy.REPUTATION_PRICE_MODIFIERS.Length - 1);
            return Economy.REPUTATION_PRICE_MODIFIERS[reputationLevel];
        }

        /// <summary>
        /// Get companion effectiveness based on loyalty
        /// </summary>
        public static float GetCompanionEffectiveness(int loyalty)
        {
            float normalizedLoyalty = Mathf.Clamp01(loyalty / 100f);
            return Mathf.Lerp(
                Companions.LOYALTY_MIN_EFFECTIVENESS,
                Companions.LOYALTY_MAX_EFFECTIVENESS,
                normalizedLoyalty
            );
        }

        /// <summary>
        /// Apply balance patch (for runtime tuning)
        /// </summary>
        public static void ApplyBalancePatch(string patchVersion)
        {
            Debug.Log($"BalanceConfig: Applying balance patch {patchVersion}");
            // Future patches can be applied here
        }
    }
}
