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
        /// v2.6.7: Enhanced with equipment-based cost reduction support
        /// </summary>
        /// <param name="magicType">Type of magic ability</param>
        /// <param name="character">Optional character for equipment-based cost reduction</param>
        /// <returns>Mana cost after equipment reductions</returns>
        public static int GetManaCost(MagicType magicType, Character character = null)
        {
            int baseCost;
            switch (magicType)
            {
                // Basic magic - low cost
                case MagicType.Healing:
                    baseCost = 15; break;
                case MagicType.ShieldCreation:
                    baseCost = 20; break;
                case MagicType.LightManipulation:
                    baseCost = 10; break;

                // Elemental magic - medium cost
                case MagicType.FireManipulation:
                    baseCost = 25; break;
                case MagicType.WaterManipulation:
                    baseCost = 25; break;
                case MagicType.WindManipulation:
                    baseCost = 25; break;
                case MagicType.IceManipulation:
                    baseCost = 25; break;
                case MagicType.DarknessManipulation:
                    baseCost = 30; break;

                // Advanced magic - high cost
                case MagicType.Shapeshifting:
                    baseCost = 40; break;
                case MagicType.Winnowing:
                    baseCost = 35; break;
                case MagicType.Daemati:
                    baseCost = 50; break;
                case MagicType.Shadowsinger:
                    baseCost = 45; break;
                case MagicType.TruthTelling:
                    baseCost = 30; break;

                // Legendary magic - very high cost
                case MagicType.DeathManifestation:
                    baseCost = 60; break;
                case MagicType.Seer:
                    baseCost = 40; break;
                case MagicType.MatingBond:
                    baseCost = 50; break;

                default:
                    baseCost = 20; break; // Default cost
            }
            
            // v2.6.7: Apply equipment-based cost reduction
            if (character != null && character.inventory != null)
            {
                var (flatReduction, percentReduction) = character.inventory.GetManaCostReduction();
                
                // Apply percent reduction first
                int costAfterPercent = Mathf.RoundToInt(baseCost * (1.0f - percentReduction));
                
                // Then apply flat reduction
                int finalCost = Mathf.Max(1, costAfterPercent - flatReduction); // Minimum cost of 1
                
                if (finalCost < baseCost)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, "ManaSystem", 
                        $"Mana cost reduced: {baseCost} → {finalCost} (Equipment: -{flatReduction} flat, -{percentReduction*100}% percent)");
                }
                
                return finalCost;
            }
            
            return baseCost;
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
