using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Manages mana (magical energy) for characters
    /// v2.3.3: NEW - Adds resource management for magic abilities
    /// </summary>
    [System.Serializable]
    public class ManaSystem
    {
        private int currentMana;
        private int maxMana;
        private int manaRegen; // Mana restored per turn

        // Constants
        private const int BASE_MANA_PER_MAGIC_POWER = 2; // 2 mana per magic power point
        private const int MIN_MANA_REGEN = 5;
        private const int MANA_REGEN_PER_LEVEL = 2;

        /// <summary>
        /// Properties for external access
        /// </summary>
        public int CurrentMana { get { return currentMana; } }
        public int MaxMana { get { return maxMana; } }
        public int ManaRegen { get { return manaRegen; } }
        public float ManaPercentage { get { return maxMana > 0 ? (float)currentMana / maxMana : 0f; } }

        /// <summary>
        /// Initialize mana system based on character stats
        /// </summary>
        public void Initialize(int magicPower, int level)
        {
            maxMana = magicPower * BASE_MANA_PER_MAGIC_POWER;
            currentMana = maxMana;
            manaRegen = MIN_MANA_REGEN + (level * MANA_REGEN_PER_LEVEL);
        }

        /// <summary>
        /// Update max mana when magic power changes
        /// </summary>
        public void UpdateMaxMana(int magicPower)
        {
            int oldMax = maxMana;
            maxMana = magicPower * BASE_MANA_PER_MAGIC_POWER;
            
            // Adjust current mana proportionally
            if (oldMax > 0)
            {
                float percentage = (float)currentMana / oldMax;
                currentMana = Mathf.RoundToInt(maxMana * percentage);
            }
            else
            {
                currentMana = maxMana;
            }
        }

        /// <summary>
        /// Update mana regeneration when level changes
        /// </summary>
        public void UpdateManaRegen(int level)
        {
            manaRegen = MIN_MANA_REGEN + (level * MANA_REGEN_PER_LEVEL);
        }

        /// <summary>
        /// Try to consume mana for an ability
        /// Returns true if mana was available and consumed
        /// </summary>
        public bool TryConsumeMana(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Cannot consume negative mana");
                return false;
            }

            if (currentMana >= amount)
            {
                currentMana -= amount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Restore mana (from items, abilities, or turn regeneration)
        /// </summary>
        public void RestoreMana(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Cannot restore negative mana");
                return;
            }

            currentMana = Mathf.Min(currentMana + amount, maxMana);
        }

        /// <summary>
        /// Regenerate mana at turn start
        /// </summary>
        public int RegenerateMana()
        {
            int restored = Mathf.Min(manaRegen, maxMana - currentMana);
            currentMana += restored;
            return restored;
        }

        /// <summary>
        /// Set mana to maximum (for rest, level up, etc.)
        /// </summary>
        public void RestoreToMax()
        {
            currentMana = maxMana;
        }

        /// <summary>
        /// Check if character has enough mana for an ability
        /// </summary>
        public bool HasEnoughMana(int cost)
        {
            return currentMana >= cost;
        }

        /// <summary>
        /// Get mana cost for a magic type
        /// </summary>
        public static int GetManaCost(MagicType magicType)
        {
            switch (magicType)
            {
                // Basic magic - low cost
                case MagicType.Healing:
                    return 15;
                case MagicType.ShieldCreation:
                    return 20;
                case MagicType.LightManipulation:
                    return 10;

                // Elemental magic - medium cost
                case MagicType.FireManipulation:
                    return 25;
                case MagicType.WaterManipulation:
                    return 25;
                case MagicType.WindManipulation:
                    return 25;
                case MagicType.IceManipulation:
                    return 25;
                case MagicType.DarknessManipulation:
                    return 30;

                // Advanced magic - high cost
                case MagicType.Shapeshifting:
                    return 40;
                case MagicType.Winnowing:
                    return 35;
                case MagicType.Daemati:
                    return 50;
                case MagicType.Shadowsinger:
                    return 45;
                case MagicType.TruthTelling:
                    return 30;

                // Legendary magic - very high cost
                case MagicType.DeathManifestation:
                    return 60;
                case MagicType.Seer:
                    return 40;
                case MagicType.MatingBond:
                    return 50;

                default:
                    return 20; // Default cost
            }
        }

        /// <summary>
        /// Get current mana as a formatted string
        /// </summary>
        public string GetManaString()
        {
            return $"{currentMana}/{maxMana}";
        }
    }
}
