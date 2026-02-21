using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Manages ability cooldowns and mana costs for magic abilities
    /// Prevents ability spam and adds strategic depth to combat
    /// </summary>
    public class AbilityCooldownManager : MonoBehaviour
    {
        public static AbilityCooldownManager Instance { get; private set; }

        // Cooldown tracking (abilityId -> remaining cooldown time in seconds)
        private Dictionary<string, float> abilityCooldowns = new Dictionary<string, float>();
        
        // Last use time tracking (abilityId -> game time when last used)
        private Dictionary<string, float> lastUseTime = new Dictionary<string, float>();

        // Default cooldown durations by ability type (in seconds)
        private Dictionary<MagicType, float> defaultCooldowns = new Dictionary<MagicType, float>();
        
        // Mana costs by ability type
        private Dictionary<MagicType, int> manaCosts = new Dictionary<MagicType, int>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDefaults();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            // Update cooldowns every frame
            UpdateCooldowns();
        }

        /// <summary>
        /// Initialize default cooldowns and mana costs for each magic type
        /// </summary>
        private void InitializeDefaults()
        {
            // Cooldowns in seconds
            defaultCooldowns[MagicType.Shapeshifting] = 30f;
            defaultCooldowns[MagicType.Winnowing] = 10f;
            defaultCooldowns[MagicType.Daemati] = 20f;
            defaultCooldowns[MagicType.DarknessManipulation] = 15f;
            defaultCooldowns[MagicType.LightManipulation] = 15f;
            defaultCooldowns[MagicType.FireManipulation] = 8f;
            defaultCooldowns[MagicType.WaterManipulation] = 8f;
            defaultCooldowns[MagicType.IceManipulation] = 10f;
            defaultCooldowns[MagicType.WindManipulation] = 8f;
            defaultCooldowns[MagicType.EarthManipulation] = 12f;
            defaultCooldowns[MagicType.Healing] = 15f;
            defaultCooldowns[MagicType.ShieldCreation] = 20f;
            defaultCooldowns[MagicType.Shadowsinger] = 25f;
            defaultCooldowns[MagicType.Seer] = 60f;
            defaultCooldowns[MagicType.SpellCleaving] = 18f;
            defaultCooldowns[MagicType.TruthTelling] = 30f;

            // Mana costs
            manaCosts[MagicType.Shapeshifting] = 40;
            manaCosts[MagicType.Winnowing] = 25;
            manaCosts[MagicType.Daemati] = 35;
            manaCosts[MagicType.DarknessManipulation] = 30;
            manaCosts[MagicType.LightManipulation] = 30;
            manaCosts[MagicType.FireManipulation] = 20;
            manaCosts[MagicType.WaterManipulation] = 20;
            manaCosts[MagicType.IceManipulation] = 25;
            manaCosts[MagicType.WindManipulation] = 20;
            manaCosts[MagicType.EarthManipulation] = 25;
            manaCosts[MagicType.Healing] = 30;
            manaCosts[MagicType.ShieldCreation] = 35;
            manaCosts[MagicType.Shadowsinger] = 40;
            manaCosts[MagicType.Seer] = 50;
            manaCosts[MagicType.SpellCleaving] = 30;
            manaCosts[MagicType.TruthTelling] = 45;
        }

        /// <summary>
        /// Update all active cooldowns
        /// </summary>
        private void UpdateCooldowns()
        {
            List<string> expiredCooldowns = new List<string>();
            
            foreach (var kvp in abilityCooldowns)
            {
                string abilityId = kvp.Key;
                float remainingTime = kvp.Value;
                
                if (remainingTime > 0)
                {
                    abilityCooldowns[abilityId] = remainingTime - Time.deltaTime;
                }
                else
                {
                    expiredCooldowns.Add(abilityId);
                }
            }
            
            // Remove expired cooldowns
            foreach (string abilityId in expiredCooldowns)
            {
                abilityCooldowns.Remove(abilityId);
            }
        }

        /// <summary>
        /// Check if an ability is ready to use (not on cooldown)
        /// </summary>
        public bool IsAbilityReady(string abilityId)
        {
            if (!abilityCooldowns.ContainsKey(abilityId))
                return true;
                
            return abilityCooldowns[abilityId] <= 0;
        }

        /// <summary>
        /// Check if character has enough mana to cast ability
        /// </summary>
        public bool HasEnoughMana(Character character, MagicType abilityType)
        {
            if (!manaCosts.ContainsKey(abilityType))
                return true; // No mana cost defined, allow it
                
            int cost = manaCosts[abilityType];
            return character.stats.magicPower >= cost;
        }

        /// <summary>
        /// Use an ability, starting its cooldown and consuming mana
        /// </summary>
        public bool UseAbility(Character character, MagicType abilityType, string abilityId)
        {
            // Check if ability is ready
            if (!IsAbilityReady(abilityId))
            {
                Debug.Log($"Ability {abilityId} is on cooldown!");
                return false;
            }
            
            // Check mana cost
            if (!HasEnoughMana(character, abilityType))
            {
                Debug.Log($"Not enough mana to cast {abilityType}!");
                return false;
            }
            
            // Consume mana
            if (manaCosts.ContainsKey(abilityType))
            {
                int cost = manaCosts[abilityType];
                character.stats.magicPower -= cost;
                Debug.Log($"Consumed {cost} mana. Remaining: {character.stats.magicPower}");
            }
            
            // Start cooldown
            if (defaultCooldowns.ContainsKey(abilityType))
            {
                float cooldownTime = defaultCooldowns[abilityType];
                abilityCooldowns[abilityId] = cooldownTime;
                lastUseTime[abilityId] = Time.time;
                Debug.Log($"Started cooldown for {abilityId}: {cooldownTime}s");
            }
            
            return true;
        }

        /// <summary>
        /// Get remaining cooldown time for an ability
        /// </summary>
        public float GetRemainingCooldown(string abilityId)
        {
            if (!abilityCooldowns.ContainsKey(abilityId))
                return 0f;
                
            return Mathf.Max(0f, abilityCooldowns[abilityId]);
        }

        /// <summary>
        /// Get the default cooldown duration for an ability type
        /// </summary>
        public float GetCooldownDuration(MagicType abilityType)
        {
            if (defaultCooldowns.ContainsKey(abilityType))
                return defaultCooldowns[abilityType];
            return 0f;
        }

        /// <summary>
        /// Get mana cost for an ability type
        /// </summary>
        public int GetManaCost(MagicType abilityType)
        {
            if (manaCosts.ContainsKey(abilityType))
                return manaCosts[abilityType];
            return 0;
        }

        /// <summary>
        /// Reset all cooldowns (useful for testing or special events)
        /// </summary>
        public void ResetAllCooldowns()
        {
            abilityCooldowns.Clear();
            lastUseTime.Clear();
            Debug.Log("All ability cooldowns reset");
        }

        /// <summary>
        /// Reset cooldown for a specific ability
        /// </summary>
        public void ResetCooldown(string abilityId)
        {
            if (abilityCooldowns.ContainsKey(abilityId))
            {
                abilityCooldowns.Remove(abilityId);
                Debug.Log($"Reset cooldown for {abilityId}");
            }
        }

        /// <summary>
        /// Get all abilities currently on cooldown
        /// </summary>
        public Dictionary<string, float> GetActiveCooldowns()
        {
            return new Dictionary<string, float>(abilityCooldowns);
        }

        /// <summary>
        /// Reduce all cooldowns by a percentage (for special effects)
        /// </summary>
        public void ReduceCooldowns(float percentage)
        {
            List<string> abilities = new List<string>(abilityCooldowns.Keys);
            foreach (string abilityId in abilities)
            {
                float current = abilityCooldowns[abilityId];
                abilityCooldowns[abilityId] = current * (1f - percentage);
            }
            Debug.Log($"Reduced all cooldowns by {percentage * 100}%");
        }
    }
}
