using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Types of food buffs
    /// </summary>
    public enum FoodBuffType
    {
        None,
        HealthRegen,     // Restore health over time
        ManaRegen,       // Restore mana over time
        StrengthBoost,   // Temporary strength increase
        MagicBoost,      // Temporary magic power increase
        AgilityBoost,    // Temporary agility increase
        DefenseBoost,    // Temporary defense increase
        WellFed          // General stat boost
    }

    /// <summary>
    /// Active food buff on a character
    /// </summary>
    [System.Serializable]
    public class FoodBuff
    {
        public string foodName;
        public FoodBuffType buffType;
        public int buffValue;       // Amount of buff (stat boost or regen per tick)
        public float duration;      // Total duration in seconds
        public float remainingTime; // Time left in seconds

        public FoodBuff(string name, FoodBuffType type, int value, float duration)
        {
            this.foodName = name;
            this.buffType = type;
            this.buffValue = value;
            this.duration = duration;
            this.remainingTime = duration;
        }

        /// <summary>
        /// Update buff timer
        /// </summary>
        /// <returns>True if buff is still active</returns>
        public bool Update(float deltaTime)
        {
            remainingTime -= deltaTime;
            return remainingTime > 0;
        }
    }

    /// <summary>
    /// Manages food consumption and temporary buffs
    /// Integrates with inventory system for food items
    /// </summary>
    public class FoodBuffSystem : MonoBehaviour
    {
        public static FoodBuffSystem Instance { get; private set; }

        private Dictionary<Character, List<FoodBuff>> activeBuffs;
        private float tickInterval = 1f; // Update buffs every second
        private float tickTimer = 0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                activeBuffs = new Dictionary<Character, List<FoodBuff>>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            // Update buff timers
            tickTimer += Time.deltaTime;
            
            if (tickTimer >= tickInterval)
            {
                UpdateAllBuffs(tickInterval);
                tickTimer = 0f;
            }
        }

        /// <summary>
        /// Consume food and apply buff to character
        /// </summary>
        public void ConsumeFood(Character character, string foodItemId)
        {
            // Get food buff parameters based on item
            (FoodBuffType buffType, int buffValue, float duration) = GetFoodBuffData(foodItemId);

            if (buffType == FoodBuffType.None)
            {
                Debug.LogWarning($"Food item {foodItemId} has no buff data");
                return;
            }

            // Create and apply buff
            FoodBuff buff = new FoodBuff(foodItemId, buffType, buffValue, duration);
            AddBuff(character, buff);

            Debug.Log($"{character.name} consumed {foodItemId} - {buffType} +{buffValue} for {duration}s");
        }

        /// <summary>
        /// Get buff data for a food item
        /// </summary>
        private (FoodBuffType, int, float) GetFoodBuffData(string foodItemId)
        {
            switch (foodItemId)
            {
                // Basic Foods - Small Effects
                case "food_fae_bread":
                    return (FoodBuffType.HealthRegen, 2, 30f); // 2 HP per second for 30s

                case "food_roasted_meat":
                    return (FoodBuffType.StrengthBoost, 5, 60f); // +5 Strength for 60s

                case "food_vegetable_stew":
                    return (FoodBuffType.DefenseBoost, 5, 60f); // +5 Defense for 60s

                case "food_honey_cakes":
                    return (FoodBuffType.HealthRegen, 3, 45f); // 3 HP per second for 45s

                case "food_herbal_tea":
                    return (FoodBuffType.ManaRegen, 2, 60f); // 2 MP per second for 60s

                // Premium Foods - Stronger Effects
                case "food_starlight_wine":
                    return (FoodBuffType.MagicBoost, 15, 120f); // +15 Magic Power for 120s

                case "food_mushroom_soup":
                    return (FoodBuffType.WellFed, 5, 90f); // +5 all stats for 90s

                case "food_strength_stew":
                    return (FoodBuffType.StrengthBoost, 10, 120f); // +10 Strength for 120s

                case "food_mages_delight":
                    return (FoodBuffType.MagicBoost, 20, 120f); // +20 Magic Power for 120s

                case "food_travelers_rations":
                    return (FoodBuffType.HealthRegen, 1, 120f); // 1 HP per second for 120s

                default:
                    return (FoodBuffType.None, 0, 0f);
            }
        }

        /// <summary>
        /// Add a buff to a character
        /// </summary>
        private void AddBuff(Character character, FoodBuff buff)
        {
            if (!activeBuffs.ContainsKey(character))
            {
                activeBuffs[character] = new List<FoodBuff>();
            }

            // Check if same buff type already exists
            FoodBuff existingBuff = activeBuffs[character].Find(b => b.buffType == buff.buffType);
            if (existingBuff != null)
            {
                // Refresh duration if same type
                existingBuff.remainingTime = Mathf.Max(existingBuff.remainingTime, buff.duration);
                Debug.Log($"Refreshed {buff.buffType} buff on {character.name}");
            }
            else
            {
                // Add new buff
                activeBuffs[character].Add(buff);
                ApplyBuffToCharacter(character, buff, true);
            }
        }

        /// <summary>
        /// Update all active buffs
        /// </summary>
        private void UpdateAllBuffs(float deltaTime)
        {
            List<Character> charactersToRemove = new List<Character>();

            foreach (var kvp in activeBuffs)
            {
                Character character = kvp.Key;
                List<FoodBuff> buffs = kvp.Value;
                List<FoodBuff> buffsToRemove = new List<FoodBuff>();

                foreach (FoodBuff buff in buffs)
                {
                    // Update buff timer
                    bool stillActive = buff.Update(deltaTime);

                    // Apply regeneration effects
                    if (stillActive)
                    {
                        ApplyBuffTick(character, buff);
                    }
                    else
                    {
                        buffsToRemove.Add(buff);
                    }
                }

                // Remove expired buffs
                foreach (FoodBuff buff in buffsToRemove)
                {
                    buffs.Remove(buff);
                    RemoveBuffFromCharacter(character, buff);
                    Debug.Log($"{buff.foodName} buff expired on {character.name}");
                }

                // Clean up character entry if no buffs left
                if (buffs.Count == 0)
                {
                    charactersToRemove.Add(character);
                }
            }

            // Remove empty character entries
            foreach (Character character in charactersToRemove)
            {
                activeBuffs.Remove(character);
            }
        }

        /// <summary>
        /// Apply buff effect to character (when buff starts)
        /// </summary>
        private void ApplyBuffToCharacter(Character character, FoodBuff buff, bool isAdding)
        {
            int multiplier = isAdding ? 1 : -1;

            switch (buff.buffType)
            {
                case FoodBuffType.StrengthBoost:
                    character.stats.strength += buff.buffValue * multiplier;
                    break;

                case FoodBuffType.MagicBoost:
                    character.stats.magicPower += buff.buffValue * multiplier;
                    break;

                case FoodBuffType.AgilityBoost:
                    character.stats.agility += buff.buffValue * multiplier;
                    break;

                case FoodBuffType.DefenseBoost:
                    // Defense would need to be tracked separately or as damage reduction
                    break;

                case FoodBuffType.WellFed:
                    character.stats.strength += buff.buffValue * multiplier;
                    character.stats.magicPower += buff.buffValue * multiplier;
                    character.stats.agility += buff.buffValue * multiplier;
                    break;

                // Regen types don't need stat changes, handled per tick
                case FoodBuffType.HealthRegen:
                case FoodBuffType.ManaRegen:
                    break;
            }
        }

        /// <summary>
        /// Apply buff tick effect (for regeneration)
        /// </summary>
        private void ApplyBuffTick(Character character, FoodBuff buff)
        {
            switch (buff.buffType)
            {
                case FoodBuffType.HealthRegen:
                    character.Heal(buff.buffValue);
                    break;

                case FoodBuffType.ManaRegen:
                    character.stats.magicPower = Mathf.Min(
                        character.stats.magicPower + buff.buffValue,
                        character.stats.maxMagicPower
                    );
                    break;
            }
        }

        /// <summary>
        /// Remove buff effect from character (when buff expires)
        /// </summary>
        private void RemoveBuffFromCharacter(Character character, FoodBuff buff)
        {
            ApplyBuffToCharacter(character, buff, false);
        }

        /// <summary>
        /// Get all active buffs for a character
        /// </summary>
        public List<FoodBuff> GetActiveBuffs(Character character)
        {
            if (activeBuffs.ContainsKey(character))
            {
                return new List<FoodBuff>(activeBuffs[character]);
            }
            return new List<FoodBuff>();
        }

        /// <summary>
        /// Clear all buffs from a character
        /// </summary>
        public void ClearBuffs(Character character)
        {
            if (activeBuffs.ContainsKey(character))
            {
                List<FoodBuff> buffs = new List<FoodBuff>(activeBuffs[character]);
                foreach (FoodBuff buff in buffs)
                {
                    RemoveBuffFromCharacter(character, buff);
                }
                activeBuffs.Remove(character);
            }
        }
    }
}
