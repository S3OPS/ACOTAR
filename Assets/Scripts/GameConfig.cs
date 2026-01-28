using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Centralized configuration for game constants and balance values
    /// Optimizes performance by caching frequently accessed values
    /// </summary>
    public static class GameConfig
    {
        // Experience and Progression
        public const int BASE_XP_PER_LEVEL = 100;
        public const int STAT_INCREASE_PER_LEVEL_HEALTH = 10;
        public const int STAT_INCREASE_PER_LEVEL_MAGIC = 5;
        public const int STAT_INCREASE_PER_LEVEL_STRENGTH = 3;
        public const int STAT_INCREASE_PER_LEVEL_AGILITY = 3;

        // Character Class Base Stats
        public static class ClassStats
        {
            // High Fae
            public const int HIGHFAE_HEALTH = 150;
            public const int HIGHFAE_MAGIC = 100;
            public const int HIGHFAE_STRENGTH = 80;
            public const int HIGHFAE_AGILITY = 70;

            // Illyrian
            public const int ILLYRIAN_HEALTH = 180;
            public const int ILLYRIAN_MAGIC = 60;
            public const int ILLYRIAN_STRENGTH = 120;
            public const int ILLYRIAN_AGILITY = 90;

            // Lesser Fae
            public const int LESSERFAE_HEALTH = 100;
            public const int LESSERFAE_MAGIC = 60;
            public const int LESSERFAE_STRENGTH = 60;
            public const int LESSERFAE_AGILITY = 80;

            // Human
            public const int HUMAN_HEALTH = 80;
            public const int HUMAN_MAGIC = 0;
            public const int HUMAN_STRENGTH = 50;
            public const int HUMAN_AGILITY = 60;

            // Attor
            public const int ATTOR_HEALTH = 120;
            public const int ATTOR_MAGIC = 40;
            public const int ATTOR_STRENGTH = 90;
            public const int ATTOR_AGILITY = 100;

            // Suriel
            public const int SURIEL_HEALTH = 70;
            public const int SURIEL_MAGIC = 150;
            public const int SURIEL_STRENGTH = 30;
            public const int SURIEL_AGILITY = 40;
        }

        // Game State
        public const string DEFAULT_STARTING_LOCATION = "Human Lands";
        public const string DEFAULT_PLAYER_NAME = "Feyre Archeron";
        public const CharacterClass DEFAULT_PLAYER_CLASS = CharacterClass.Human;
        public const Court DEFAULT_PLAYER_COURT = Court.Spring;
    }
}
