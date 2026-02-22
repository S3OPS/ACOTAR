using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Types of synergy bonuses between companions
    /// </summary>
    public enum SynergyType
    {
        Damage,         // Increased damage output
        Defense,        // Increased damage reduction
        Healing,        // Enhanced healing effects
        CriticalRate,   // Increased critical hit chance
        MagicPower,     // Increased magical effectiveness
        Experience,     // Bonus experience gain
        Combo           // Unlocks special combo ability
    }

    /// <summary>
    /// Defines a synergy relationship between two companions
    /// </summary>
    [System.Serializable]
    public class CompanionSynergy
    {
        public string companion1Name;
        public string companion2Name;
        public SynergyType synergyType;
        public float bonusValue;           // Percentage bonus (e.g., 0.15 = 15%)
        public string synergyName;
        public string description;
        public string comboAbilityName;    // Optional: Name of unlocked combo ability

        public CompanionSynergy(string comp1, string comp2, SynergyType type, float bonus, string name, string desc, string comboAbility = null)
        {
            companion1Name = comp1;
            companion2Name = comp2;
            synergyType = type;
            bonusValue = bonus;
            synergyName = name;
            description = desc;
            comboAbilityName = comboAbility;
        }

        /// <summary>
        /// Check if this synergy applies to the given pair of companions
        /// </summary>
        public bool AppliesTo(string name1, string name2)
        {
            return (companion1Name == name1 && companion2Name == name2) ||
                   (companion1Name == name2 && companion2Name == name1);
        }
    }

    /// <summary>
    /// Tracks active synergies in the current party
    /// </summary>
    [System.Serializable]
    public class ActiveSynergy
    {
        public CompanionSynergy synergy;
        public bool isActive;
        public int timesTriggered;

        public ActiveSynergy(CompanionSynergy syn)
        {
            synergy = syn;
            isActive = false;
            timesTriggered = 0;
        }
    }

    /// <summary>
    /// Party Synergy System - Manages companion relationships and combo bonuses
    /// Version 2.6.0 - New Feature
    /// 
    /// This system provides strategic depth through companion pairings that unlock
    /// special bonuses and combo abilities when specific companions are in the party together.
    /// </summary>
    public class PartySynergySystem
    {
        private List<CompanionSynergy> allSynergies;
        private List<ActiveSynergy> activeSynergies;
        private Dictionary<string, int> synergyAchievementProgress;

        // Property accessors (v2.6.0: Following v2.5.x patterns)
        public int ActiveSynergyCount => activeSynergies?.Count(s => s.isActive) ?? 0;
        public bool IsInitialized => allSynergies != null && activeSynergies != null;
        public List<ActiveSynergy> ActiveSynergies => new List<ActiveSynergy>(activeSynergies);

        /// <summary>
        /// Initialize the synergy system with predefined synergies
        /// </summary>
        public PartySynergySystem()
        {
            allSynergies = new List<CompanionSynergy>();
            activeSynergies = new List<ActiveSynergy>();
            synergyAchievementProgress = new Dictionary<string, int>();
            
            InitializeSynergies();
            Debug.Log($"PartySynergySystem initialized with {allSynergies.Count} synergies");
        }

        /// <summary>
        /// Define all companion synergy relationships
        /// Based on ACOTAR lore and character relationships
        /// </summary>
        private void InitializeSynergies()
        {
            // Inner Circle - Night Court Core Team
            allSynergies.Add(new CompanionSynergy(
                "Rhysand", "Feyre",
                SynergyType.MagicPower, 0.20f,
                "High Lord & Lady Bond",
                "The mating bond amplifies magical power for both partners.",
                "Starfall Strike"
            ));

            allSynergies.Add(new CompanionSynergy(
                "Cassian", "Azriel",
                SynergyType.Combo, 0.15f,
                "Brothers in Arms",
                "Centuries of fighting together create perfect tactical coordination.",
                "Twin Strike"
            ));

            allSynergies.Add(new CompanionSynergy(
                "Rhysand", "Cassian",
                SynergyType.Damage, 0.15f,
                "High Lord's Commander",
                "Cassian's loyalty to Rhysand increases their combined offensive power."
            ));

            allSynergies.Add(new CompanionSynergy(
                "Rhysand", "Azriel",
                SynergyType.CriticalRate, 0.10f,
                "High Lord's Spymaster",
                "Azriel's intel combined with Rhysand's power creates devastating precision strikes."
            ));

            allSynergies.Add(new CompanionSynergy(
                "Azriel", "Morrigan",
                SynergyType.CriticalRate, 0.12f,
                "Truth in Shadow",
                "Truth-telling combined with shadow intel reveals enemy weaknesses."
            ));

            allSynergies.Add(new CompanionSynergy(
                "Cassian", "Nesta",
                SynergyType.Damage, 0.25f,
                "Death and War",
                "The mating bond between the Lady of Death and the Lord of Bloodshed is terrifying.",
                "Death Dancers"
            ));

            // Spring Court Allies
            allSynergies.Add(new CompanionSynergy(
                "Tamlin", "Lucien",
                SynergyType.Defense, 0.15f,
                "Spring Court Bond",
                "Centuries of friendship provide mutual protection."
            ));

            // Cross-Court Synergies
            allSynergies.Add(new CompanionSynergy(
                "Lucien", "Feyre",
                SynergyType.Healing, 0.15f,
                "Friends from the Start",
                "Their early friendship provides comfort and enhanced recovery."
            ));

            allSynergies.Add(new CompanionSynergy(
                "Feyre", "Nesta",
                SynergyType.MagicPower, 0.18f,
                "Archeron Sisters",
                "Sisterly bond enhances their Cauldron-made powers."
            ));

            allSynergies.Add(new CompanionSynergy(
                "Morrigan", "Amren",
                SynergyType.MagicPower, 0.20f,
                "Ancient Powers",
                "Combined ancient and raw magical power creates devastating effects."
            ));

            // Valkyrie Synergies
            allSynergies.Add(new CompanionSynergy(
                "Nesta", "Gwyn",
                SynergyType.Combo, 0.15f,
                "Valkyrie Sisters",
                "Trained together, fight together. The Valkyries are unstoppable.",
                "Valkyrie Assault"
            ));

            allSynergies.Add(new CompanionSynergy(
                "Nesta", "Emerie",
                SynergyType.Defense, 0.15f,
                "Shield Maidens",
                "Valkyrie training provides exceptional defensive capabilities."
            ));

            // Support Synergies
            allSynergies.Add(new CompanionSynergy(
                "Amren", "Rhysand",
                SynergyType.MagicPower, 0.15f,
                "Ancient & High Lord",
                "Millennia of trust and understanding amplifies magical prowess."
            ));

            allSynergies.Add(new CompanionSynergy(
                "Feyre", "Mor",
                SynergyType.Experience, 0.10f,
                "Learning from the Best",
                "Mor's guidance helps Feyre grow faster as High Lady."
            ));
        }

        /// <summary>
        /// Update active synergies based on current party composition
        /// </summary>
        public void UpdateActiveSynergies(List<Companion> partyMembers)
        {
            // Defensive check (v2.6.0: Following v2.5.x patterns)
            if (partyMembers == null || partyMembers.Count < 2)
            {
                activeSynergies.Clear();
                return;
            }

            activeSynergies.Clear();

            // Check all possible pairs in the party
            for (int i = 0; i < partyMembers.Count; i++)
            {
                for (int j = i + 1; j < partyMembers.Count; j++)
                {
                    string name1 = partyMembers[i].name;
                    string name2 = partyMembers[j].name;

                    // Find matching synergies
                    foreach (var synergy in allSynergies)
                    {
                        if (synergy.AppliesTo(name1, name2))
                        {
                            var activeSyn = new ActiveSynergy(synergy);
                            activeSyn.isActive = true;
                            activeSynergies.Add(activeSyn);

                            Debug.Log($"✨ Synergy Activated: {synergy.synergyName} ({name1} + {name2})");
                            
                            // Track for achievements
                            string synergyKey = $"{synergy.companion1Name}_{synergy.companion2Name}";
                            if (!synergyAchievementProgress.ContainsKey(synergyKey))
                            {
                                synergyAchievementProgress[synergyKey] = 0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get total bonus for a specific synergy type
        /// </summary>
        public float GetSynergyBonus(SynergyType type)
        {
            // Defensive check (v2.6.0)
            if (!IsInitialized)
            {
                Debug.LogWarning("PartySynergySystem: Cannot get synergy bonus - system not initialized");
                return 0f;
            }

            float totalBonus = 0f;
            foreach (var activeSyn in activeSynergies)
            {
                if (activeSyn.isActive && activeSyn.synergy.synergyType == type)
                {
                    totalBonus += activeSyn.synergy.bonusValue;
                }
            }
            return totalBonus;
        }

        /// <summary>
        /// Get all available combo abilities from active synergies
        /// </summary>
        public List<string> GetAvailableComboAbilities()
        {
            // Defensive check (v2.6.0)
            if (!IsInitialized)
            {
                Debug.LogWarning("PartySynergySystem: Cannot get combo abilities - system not initialized");
                return new List<string>();
            }

            var comboAbilities = new List<string>();
            foreach (var activeSyn in activeSynergies)
            {
                if (activeSyn.isActive && !string.IsNullOrEmpty(activeSyn.synergy.comboAbilityName))
                {
                    comboAbilities.Add(activeSyn.synergy.comboAbilityName);
                }
            }
            return comboAbilities;
        }

        /// <summary>
        /// Trigger a synergy effect (called when synergy bonus is used)
        /// </summary>
        public void TriggerSynergy(string companion1Name, string companion2Name)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(companion1Name) || string.IsNullOrEmpty(companion2Name))
            {
                Debug.LogWarning("PartySynergySystem: Cannot trigger synergy with null or empty companion names");
                return;
            }

            foreach (var activeSyn in activeSynergies)
            {
                if (activeSyn.isActive && activeSyn.synergy.AppliesTo(companion1Name, companion2Name))
                {
                    activeSyn.timesTriggered++;

                    // v2.6.10: Multi-sense feedback for synergy combo — matching cascade style
                    ScreenEffectsManager.Instance?.AlertPulse();
                    AudioManager.Instance?.PlayUISFXByName("synergy_trigger");

                    // v2.6.11: Show synergy name as on-screen notification
                    NotificationSystem.ShowCombat($"⚡ {activeSyn.synergy.synergyName}!");

                    // Track for achievements
                    string synergyKey = $"{activeSyn.synergy.companion1Name}_{activeSyn.synergy.companion2Name}";
                    if (synergyAchievementProgress.ContainsKey(synergyKey))
                    {
                        synergyAchievementProgress[synergyKey]++;
                        
                        // Achievement milestone checks
                        if (synergyAchievementProgress[synergyKey] == 10)
                        {
                            // v2.6.12: Milestone synergy notification via achievement panel
                            NotificationSystem.ShowAchievement(
                                $"⚡ {activeSyn.synergy.synergyName} Veteran",
                                $"Triggered '{activeSyn.synergy.synergyName}' 10 times!");
                        }
                        else if (synergyAchievementProgress[synergyKey] == 50)
                        {
                            // v2.6.12: Master milestone synergy notification via achievement panel
                            NotificationSystem.ShowAchievement(
                                $"⚡ {activeSyn.synergy.synergyName} Master",
                                $"You have mastered the '{activeSyn.synergy.synergyName}' synergy — triggered 50 times!");
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Get synergy statistics for achievements/UI display
        /// </summary>
        public Dictionary<string, int> GetSynergyStatistics()
        {
            // Return read-only copy (v2.6.0: Defensive pattern)
            return new Dictionary<string, int>(synergyAchievementProgress);
        }

        /// <summary>
        /// Check if a specific synergy exists between two companions
        /// </summary>
        public bool HasSynergy(string companion1Name, string companion2Name)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(companion1Name) || string.IsNullOrEmpty(companion2Name))
            {
                return false;
            }

            foreach (var synergy in allSynergies)
            {
                if (synergy.AppliesTo(companion1Name, companion2Name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get all synergies for a specific companion (for UI display)
        /// </summary>
        public List<CompanionSynergy> GetCompanionSynergies(string companionName)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(companionName))
            {
                Debug.LogWarning("PartySynergySystem: Cannot get companion synergies with null or empty name");
                return new List<CompanionSynergy>();
            }

            var companionSynergies = new List<CompanionSynergy>();
            foreach (var synergy in allSynergies)
            {
                if (synergy.companion1Name == companionName || synergy.companion2Name == companionName)
                {
                    companionSynergies.Add(synergy);
                }
            }
            return companionSynergies;
        }

        /// <summary>
        /// Get description of all currently active synergies
        /// </summary>
        public string GetActiveSynergiesDescription()
        {
            if (ActiveSynergyCount == 0)
            {
                return "No active synergies. Add more companions to your party!";
            }

            var description = $"Active Synergies ({ActiveSynergyCount}):\n\n";
            foreach (var activeSyn in activeSynergies)
            {
                if (activeSyn.isActive)
                {
                    var syn = activeSyn.synergy;
                    description += $"✨ {syn.synergyName}\n";
                    description += $"   {syn.companion1Name} + {syn.companion2Name}\n";
                    description += $"   {syn.description}\n";
                    description += $"   Bonus: +{(syn.bonusValue * 100):F0}% {syn.synergyType}\n";
                    if (!string.IsNullOrEmpty(syn.comboAbilityName))
                    {
                        description += $"   🎯 Combo: {syn.comboAbilityName}\n";
                    }
                    description += "\n";
                }
            }
            return description;
        }
    }
}
