using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Centralized configuration for game constants and balance values
    /// Optimizes performance by caching frequently accessed values
    /// Enhanced with combat, currency, and progression settings
    /// </summary>
    public static class GameConfig
    {
        // =====================================================
        // EXPERIENCE AND PROGRESSION
        // =====================================================
        public const int BASE_XP_PER_LEVEL = 100;
        public const int STAT_INCREASE_PER_LEVEL_HEALTH = 10;
        public const int STAT_INCREASE_PER_LEVEL_MAGIC = 5;
        public const int STAT_INCREASE_PER_LEVEL_STRENGTH = 3;
        public const int STAT_INCREASE_PER_LEVEL_AGILITY = 3;
        public const int MAX_LEVEL = 50;

        // =====================================================
        // CHARACTER CLASS BASE STATS
        // =====================================================
        public static class ClassStats
        {
            // High Fae - Balanced magic users
            public const int HIGHFAE_HEALTH = 150;
            public const int HIGHFAE_MAGIC = 100;
            public const int HIGHFAE_STRENGTH = 80;
            public const int HIGHFAE_AGILITY = 70;

            // Illyrian - Powerful warriors with flight
            public const int ILLYRIAN_HEALTH = 180;
            public const int ILLYRIAN_MAGIC = 60;
            public const int ILLYRIAN_STRENGTH = 120;
            public const int ILLYRIAN_AGILITY = 90;

            // Lesser Fae - Quick and versatile
            public const int LESSERFAE_HEALTH = 100;
            public const int LESSERFAE_MAGIC = 60;
            public const int LESSERFAE_STRENGTH = 60;
            public const int LESSERFAE_AGILITY = 80;

            // Human - Starts weak, can be Made
            public const int HUMAN_HEALTH = 80;
            public const int HUMAN_MAGIC = 0;
            public const int HUMAN_STRENGTH = 50;
            public const int HUMAN_AGILITY = 60;

            // Attor - Flying predators
            public const int ATTOR_HEALTH = 120;
            public const int ATTOR_MAGIC = 40;
            public const int ATTOR_STRENGTH = 90;
            public const int ATTOR_AGILITY = 100;

            // Suriel - Prophetic creatures
            public const int SURIEL_HEALTH = 70;
            public const int SURIEL_MAGIC = 150;
            public const int SURIEL_STRENGTH = 30;
            public const int SURIEL_AGILITY = 40;
        }

        // =====================================================
        // COMBAT SETTINGS
        // =====================================================
        public static class CombatSettings
        {
            // Critical hits
            public const float CRITICAL_HIT_CHANCE = 0.15f;
            public const float CRITICAL_HIT_MULTIPLIER = 2.0f;
            
            // Dodge and defense
            public const float BASE_DODGE_CHANCE = 0.05f;
            public const float AGILITY_DODGE_FACTOR = 0.01f;
            public const float DEFEND_DAMAGE_REDUCTION = 0.5f;
            
            // Flee mechanics
            public const float BASE_FLEE_CHANCE = 0.5f;
            public const float MIN_FLEE_CHANCE = 0.1f;
            public const float MAX_FLEE_CHANCE = 0.9f;
            
            // Combo system (v2.6.7: Enhanced with cascade bonuses)
            public const int MAX_COMBO_COUNT = 5;
            public const float COMBO_DAMAGE_BONUS_PER_HIT = 0.1f;  // 10% per combo hit
            public const int COMBO_CASCADE_THRESHOLD = 5;  // NEW: Trigger cascade every 5 hits
            public const float COMBO_CASCADE_BONUS = 0.5f;  // NEW: 50% bonus on cascade
            public const float COMBO_CASCADE_XP_BONUS = 1.25f;  // NEW: 25% bonus XP on cascade
            
            // Status effects
            public const int DEFAULT_EFFECT_DURATION = 3;
            public const float EFFECT_RESIST_CHANCE_PER_LEVEL = 0.02f;
            
            // Experience from combat
            public const int BASE_COMBAT_XP = 25;
        }

        // =====================================================
        // CURRENCY SETTINGS
        // =====================================================
        public static class CurrencySettings
        {
            public const int STARTING_GOLD = 100;
            public const int MAX_GOLD = 9999999;
            
            // Enemy gold drops (base values, scaled by level and difficulty)
            public const int TRIVIAL_ENEMY_GOLD = 5;
            public const int EASY_ENEMY_GOLD = 10;
            public const int NORMAL_ENEMY_GOLD = 25;
            public const int HARD_ENEMY_GOLD = 50;
            public const int ELITE_ENEMY_GOLD = 100;
            public const int BOSS_ENEMY_GOLD = 500;
            
            // Shop pricing
            public const float SELL_PRICE_RATIO = 0.5f;  // Items sell for 50% of buy price
        }

        // =====================================================
        // INVENTORY SETTINGS
        // =====================================================
        public static class InventorySettings
        {
            public const int MAX_INVENTORY_SIZE = 50;
            public const int DEFAULT_STACK_SIZE = 99;
            public const int EQUIPMENT_SLOTS = 6;  // Weapon, Armor, Accessory x2, Ring x2
        }

        // =====================================================
        // COMPANION SETTINGS
        // =====================================================
        public static class CompanionSettings
        {
            public const int MAX_PARTY_SIZE = 3;
            public const int DEFAULT_LOYALTY = 50;
            public const int MAX_LOYALTY = 100;
            public const int MIN_LOYALTY = 0;
            
            // Loyalty thresholds
            public const int LOYALTY_VERY_LOW = 20;
            public const int LOYALTY_LOW = 40;
            public const int LOYALTY_NEUTRAL = 60;
            public const int LOYALTY_HIGH = 80;
            
            // Loyalty bonuses
            public const float LOYALTY_HIGH_BONUS = 1.2f;
            public const float LOYALTY_LOW_PENALTY = 0.8f;
        }

        // =====================================================
        // TIME SETTINGS
        // =====================================================
        public static class TimeSettings
        {
            public const int HOURS_PER_DAY = 24;
            public const int DAYS_PER_MONTH = 30;
            public const int MONTHS_PER_YEAR = 12;
            public const int MOON_CYCLE_DAYS = 30;
            
            // Time of day ranges (hours)
            public const int DAWN_START = 5;
            public const int MORNING_START = 8;
            public const int MIDDAY_START = 12;
            public const int AFTERNOON_START = 15;
            public const int DUSK_START = 18;
            public const int EVENING_START = 20;
            public const int NIGHT_START = 23;
            public const int LATE_NIGHT_START = 2;
        }

        // =====================================================
        // SAVE SYSTEM SETTINGS
        // =====================================================
        public static class SaveSettings
        {
            public const string SAVE_FILE_PREFIX = "acotar_save_";
            public const string SAVE_FILE_EXTENSION = ".json";
            public const int MAX_SAVE_SLOTS = 5;
            public const bool AUTO_SAVE_ENABLED = true;
            public const int AUTO_SAVE_INTERVAL_MINUTES = 10;
        }

        // =====================================================
        // GAME STATE DEFAULTS
        // =====================================================
        public const string DEFAULT_STARTING_LOCATION = "Human Lands";
        public const string DEFAULT_PLAYER_NAME = "Feyre Archeron";
        public const CharacterClass DEFAULT_PLAYER_CLASS = CharacterClass.Human;
        public const Court DEFAULT_PLAYER_COURT = Court.Spring;

        // =====================================================
        // UI SETTINGS
        // =====================================================
        public static class UISettings
        {
            public const float TOOLTIP_DELAY = 0.5f;
            public const float NOTIFICATION_DURATION = 3.0f;
            public const float FADE_DURATION = 0.3f;
            public const int MAX_COMBAT_LOG_ENTRIES = 50;
            public const float BUTTON_TRANSITION_DURATION = 0.1f;
            public const float PANEL_FADE_DURATION = 0.25f;
            public const float UI_ANIMATION_SPEED = 1.0f;
        }

        // =====================================================
        // GRAPHICS SETTINGS
        // =====================================================
        public static class GraphicsSettings
        {
            // Default quality
            public const int DEFAULT_QUALITY_LEVEL = 2; // High
            public const bool DEFAULT_VSYNC = true;
            public const int DEFAULT_TARGET_FPS = 60;
            
            // Screen effects
            public const float DEFAULT_SCREEN_SHAKE_INTENSITY = 0.1f;
            public const float DEFAULT_SCREEN_SHAKE_DURATION = 0.3f;
            public const float CRITICAL_HIT_SHAKE_MULTIPLIER = 2.0f;
            public const float SCREEN_FLASH_DURATION = 0.15f;
            
            // Particle effects
            public const int LOW_PARTICLE_DENSITY = 25;
            public const int MEDIUM_PARTICLE_DENSITY = 50;
            public const int HIGH_PARTICLE_DENSITY = 100;
            public const int ULTRA_PARTICLE_DENSITY = 150;
            
            // Shadow settings
            public const int LOW_SHADOW_RESOLUTION = 512;
            public const int MEDIUM_SHADOW_RESOLUTION = 1024;
            public const int HIGH_SHADOW_RESOLUTION = 2048;
            public const int ULTRA_SHADOW_RESOLUTION = 4096;
            
            // Render scale
            public const float LOW_RENDER_SCALE = 0.75f;
            public const float MEDIUM_RENDER_SCALE = 1.0f;
            public const float HIGH_RENDER_SCALE = 1.0f;
            public const float ULTRA_RENDER_SCALE = 1.25f;
        }

        // =====================================================
        // VISUAL EFFECTS SETTINGS
        // =====================================================
        public static class VFXSettings
        {
            // Effect durations
            public const float HIT_EFFECT_DURATION = 0.3f;
            public const float CRITICAL_EFFECT_DURATION = 0.5f;
            public const float MAGIC_CAST_DURATION = 0.5f;
            public const float MAGIC_IMPACT_DURATION = 0.4f;
            public const float HEAL_EFFECT_DURATION = 1.0f;
            public const float LEVEL_UP_EFFECT_DURATION = 2.0f;
            
            // Effect scales
            public const float DEFAULT_EFFECT_SCALE = 1.0f;
            public const float CRITICAL_EFFECT_SCALE = 1.5f;
            public const float LEVEL_UP_EFFECT_SCALE = 2.0f;
            
            // Trail effect
            public const float TRAIL_EFFECT_DURATION = 0.3f;
            public const float PROJECTILE_SPEED = 10f;
        }

        // =====================================================
        // EQUIPMENT AND ITEMS
        // =====================================================
        public static class Equipment
        {
            // v2.6.7: Equipment set bonuses
            // Sets require 2, 3, or 4 pieces to activate bonuses
            public const int SET_BONUS_2_PIECES = 2;
            public const int SET_BONUS_3_PIECES = 3;
            public const int SET_BONUS_4_PIECES = 4;
            
            // Set bonus multipliers
            public const float SET_BONUS_2_PIECE_MULTIPLIER = 1.1f;  // 10% bonus
            public const float SET_BONUS_3_PIECE_MULTIPLIER = 1.25f;  // 25% bonus
            public const float SET_BONUS_4_PIECE_MULTIPLIER = 1.5f;   // 50% bonus
            
            // Mana cost reduction caps
            public const int MAX_FLAT_MANA_REDUCTION = 20;  // Maximum flat reduction
            public const float MAX_PERCENT_MANA_REDUCTION = 0.5f;  // Maximum 50% reduction
        }

        // =====================================================
        // DEBUG/DEVELOPMENT
        // =====================================================
        public static class DebugSettings
        {
            public const bool SHOW_DEBUG_INFO = false;
            public const bool INFINITE_GOLD = false;
            public const bool INFINITE_HEALTH = false;
            public const bool UNLOCK_ALL_ABILITIES = false;
            public const bool SHOW_VFX_DEBUG = false;
            public const bool SHOW_GRAPHICS_DEBUG = false;
        }
    }
}
