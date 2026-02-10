using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Special companion abilities and team combo attacks
    /// NEW FEATURE: Enhanced companion combat system for base game and DLCs
    /// </summary>
    public enum CompanionSpecialAbility
    {
        // Rhysand Abilities
        MindShatter,            // Daemati attack that stuns enemies
        DarknessShroud,         // Protects entire party with darkness
        
        // Cassian Abilities
        SiphonBlast,            // Devastating physical attack
        WarCry,                 // Buffs entire party's strength
        
        // Azriel Abilities
        ShadowStrike,           // High damage stealth attack
        TruthTeller,            // His legendary dagger attack
        
        // Mor Abilities
        TruthBurst,             // Area damage with truth magic
        CourtOfDreams,          // Party-wide buff
        
        // Amren Abilities
        AncientWrath,           // Powerful magic attack
        TimeStop,               // Stuns all enemies briefly
        
        // Lucien Abilities
        SpellCleaver,           // Breaks enemy magic
        FireBurst,              // Area fire damage
        
        // Tamlin Abilities
        BeastForm,              // Transform and rampage
        NaturalHealing,         // Self-heal ability
        
        // Nesta Abilities
        SilverFlames,           // Death magic attack
        Valkyrie Strike,        // Powerful sword attack (Book 3+)
        
        // Elain Abilities
        PropheticVision,        // Reveals enemy weaknesses
        GardenBlessing          // Heals party over time
    }

    /// <summary>
    /// Team combo attacks that require specific companion combinations
    /// </summary>
    [System.Serializable]
    public class TeamCombo
    {
        public string comboName;
        public string description;
        public List<string> requiredCompanions;  // Names of companions needed
        public int powerMultiplier;  // Damage multiplier (2 = 200%, 3 = 300%, etc.)
        public string effectDescription;
        
        public TeamCombo(string name, string desc, List<string> companions, int multiplier, string effect)
        {
            comboName = name;
            description = desc;
            requiredCompanions = companions;
            powerMultiplier = multiplier;
            effectDescription = effect;
        }
    }

    /// <summary>
    /// Enhanced companion abilities system
    /// Provides special moves and team combos for companions
    /// </summary>
    public class CompanionAbilities
    {
        private static Dictionary<string, List<CompanionSpecialAbility>> companionSpecials;
        private static List<TeamCombo> teamCombos;

        /// <summary>
        /// Initialize companion special abilities
        /// </summary>
        public static void Initialize()
        {
            InitializeSpecialAbilities();
            InitializeTeamCombos();
        }

        /// <summary>
        /// Set up which companions have which special abilities
        /// </summary>
        private static void InitializeSpecialAbilities()
        {
            companionSpecials = new Dictionary<string, List<CompanionSpecialAbility>>();

            // Rhysand - Most Powerful High Lord
            companionSpecials["Rhysand"] = new List<CompanionSpecialAbility>
            {
                CompanionSpecialAbility.MindShatter,
                CompanionSpecialAbility.DarknessShroud
            };

            // Cassian - General of Illyrian Legions
            companionSpecials["Cassian"] = new List<CompanionSpecialAbility>
            {
                CompanionSpecialAbility.SiphonBlast,
                CompanionSpecialAbility.WarCry
            };

            // Azriel - Spymaster and Shadowsinger
            companionSpecials["Azriel"] = new List<CompanionSpecialAbility>
            {
                CompanionSpecialAbility.ShadowStrike,
                CompanionSpecialAbility.TruthTeller
            };

            // Mor - The Morrigan
            companionSpecials["Morrigan"] = new List<CompanionSpecialAbility>
            {
                CompanionSpecialAbility.TruthBurst,
                CompanionSpecialAbility.CourtOfDreams
            };

            // Amren - Ancient One
            companionSpecials["Amren"] = new List<CompanionSpecialAbility>
            {
                CompanionSpecialAbility.AncientWrath,
                CompanionSpecialAbility.TimeStop
            };

            // Lucien - Spell-Cleaver
            companionSpecials["Lucien"] = new List<CompanionSpecialAbility>
            {
                CompanionSpecialAbility.SpellCleaver,
                CompanionSpecialAbility.FireBurst
            };

            // Tamlin - Shapeshifter
            companionSpecials["Tamlin"] = new List<CompanionSpecialAbility>
            {
                CompanionSpecialAbility.BeastForm,
                CompanionSpecialAbility.NaturalHealing
            };

            // Nesta - Death Incarnate
            companionSpecials["Nesta Archeron"] = new List<CompanionSpecialAbility>
            {
                CompanionSpecialAbility.SilverFlames,
                CompanionSpecialAbility.Valkyrie Strike  // DLC 2 only
            };

            // Elain - Seer
            companionSpecials["Elain Archeron"] = new List<CompanionSpecialAbility>
            {
                CompanionSpecialAbility.PropheticVision,
                CompanionSpecialAbility.GardenBlessing
            };
        }

        /// <summary>
        /// Initialize team combo attacks
        /// </summary>
        private static void InitializeTeamCombos()
        {
            teamCombos = new List<TeamCombo>();

            // Inner Circle Combos (Night Court)
            teamCombos.Add(new TeamCombo(
                "Court of Dreams Assault",
                "The Inner Circle unleashes their combined power",
                new List<string> { "Rhysand", "Cassian", "Azriel" },
                3,
                "Massive damage to all enemies with Night Court magic"
            ));

            teamCombos.Add(new TeamCombo(
                "Illyrian Legion Strike",
                "The three Illyrian warriors coordinate a devastating attack",
                new List<string> { "Cassian", "Azriel", "Rhysand" },
                3,
                "Triple physical attack with siphon boost"
            ));

            // Female Power Combos
            teamCombos.Add(new TeamCombo(
                "Queens of the Night",
                "The powerful females of the Night Court combine their magic",
                new List<string> { "Morrigan", "Amren", "Nesta Archeron" },
                3,
                "Devastating magical assault combining Truth, Ancient Power, and Death"
            ));

            // Archeron Sisters
            teamCombos.Add(new TeamCombo(
                "Archeron Fury",
                "The three Archeron sisters fight together",
                new List<string> { "Nesta Archeron", "Elain Archeron" },  // + Feyre (player)
                2,
                "Sister bond amplifies all abilities"
            ));

            // Mate Bond Combos
            teamCombos.Add(new TeamCombo(
                "Mating Bond Surge",
                "The power of the mating bond amplifies combat abilities",
                new List<string> { "Rhysand" },  // + Feyre (player)
                2,
                "Mating bond creates powerful magical synergy"
            ));

            // Spring Court Alliance (Book 1)
            teamCombos.Add(new TeamCombo(
                "Spring's Wrath",
                "Tamlin and Lucien fight together for the Spring Court",
                new List<string> { "Tamlin", "Lucien" },
                2,
                "Combined nature and fire magic"
            ));

            // Shadow and Truth (DLC 1+)
            teamCombos.Add(new TeamCombo(
                "Shadow and Truth",
                "Azriel's shadows and Mor's truth combine",
                new List<string> { "Azriel", "Morrigan" },
                2,
                "Enemies cannot hide from shadows or lies"
            ));

            // Ancient Wisdom (DLC 1+)
            teamCombos.Add(new TeamCombo(
                "Ancient Wisdom",
                "Amren and Rhysand combine their vast power and knowledge",
                new List<string> { "Amren", "Rhysand" },
                3,
                "Ancient magic overwhelms enemies"
            ));

            // Valkyries (DLC 2 only - requires Nesta training)
            teamCombos.Add(new TeamCombo(
                "Valkyrie Shield Wall",
                "The Valkyries form an impenetrable defense",
                new List<string> { "Nesta Archeron" },  // In Book 3, add Gwyn and Emerie
                2,
                "Party defense dramatically increased"
            ));
        }

        /// <summary>
        /// Get special abilities for a companion
        /// </summary>
        public static List<CompanionSpecialAbility> GetCompanionAbilities(string companionName)
        {
            if (companionSpecials == null) Initialize();
            
            if (companionSpecials.ContainsKey(companionName))
            {
                return companionSpecials[companionName];
            }
            
            return new List<CompanionSpecialAbility>();
        }

        /// <summary>
        /// Check if a team combo is available with current party
        /// </summary>
        public static List<TeamCombo> GetAvailableCombos(List<Companion> activeParty, Character player)
        {
            if (teamCombos == null) Initialize();
            
            List<TeamCombo> available = new List<TeamCombo>();
            List<string> partyNames = new List<string>();
            
            // Add active party members
            foreach (var companion in activeParty)
            {
                partyNames.Add(companion.name);
            }
            
            // Player (Feyre) is always in party
            bool playerIsFeyre = player.name.ToLower().Contains("feyre");
            
            // Check each combo
            foreach (var combo in teamCombos)
            {
                bool hasAllMembers = true;
                
                foreach (var requiredName in combo.requiredCompanions)
                {
                    if (!partyNames.Contains(requiredName))
                    {
                        hasAllMembers = false;
                        break;
                    }
                }
                
                if (hasAllMembers)
                {
                    available.Add(combo);
                }
            }
            
            return available;
        }

        /// <summary>
        /// Execute a special ability with effects
        /// </summary>
        public static void ExecuteSpecialAbility(CompanionSpecialAbility ability, Companion user, List<Enemy> targets)
        {
            Debug.Log($"{user.name} uses {ability}!");
            
            switch (ability)
            {
                case CompanionSpecialAbility.MindShatter:
                    // Daemati attack - high damage + stun
                    foreach (var target in targets)
                    {
                        int damage = user.magicPower * 2;
                        Debug.Log($"Mind Shatter deals {damage} psychic damage and stuns!");
                        // Apply stun effect
                    }
                    break;
                    
                case CompanionSpecialAbility.SiphonBlast:
                    // Massive physical damage to single target
                    if (targets.Count > 0)
                    {
                        int damage = user.strength * 3;
                        Debug.Log($"Siphon Blast deals {damage} devastating damage!");
                    }
                    break;
                    
                case CompanionSpecialAbility.ShadowStrike:
                    // High critical hit chance attack
                    if (targets.Count > 0)
                    {
                        int damage = user.strength * 2;
                        Debug.Log($"Shadow Strike from the darkness deals {damage} damage with guaranteed critical!");
                    }
                    break;
                    
                case CompanionSpecialAbility.TruthBurst:
                    // Area attack with truth magic
                    foreach (var target in targets)
                    {
                        int damage = user.magicPower * 2;
                        Debug.Log($"Truth magic reveals and damages for {damage}!");
                    }
                    break;
                    
                case CompanionSpecialAbility.SilverFlames:
                    // Nesta's death magic - extremely powerful
                    foreach (var target in targets)
                    {
                        int damage = user.magicPower * 3;
                        Debug.Log($"Silver Flames of Death deal {damage} catastrophic damage!");
                    }
                    break;
                    
                case CompanionSpecialAbility.PropheticVision:
                    // Reveals enemy weaknesses
                    Debug.Log($"Elain's vision reveals enemy weaknesses! Critical chance increased!");
                    break;
                    
                // Add more ability implementations as needed
                default:
                    Debug.Log($"{ability} executed!");
                    break;
            }
        }

        /// <summary>
        /// Execute a team combo attack
        /// </summary>
        public static void ExecuteTeamCombo(TeamCombo combo, List<Companion> party, List<Enemy> targets)
        {
            Debug.Log($"TEAM COMBO: {combo.comboName}!");
            Debug.Log(combo.effectDescription);
            
            // Calculate base damage from all party members
            int totalPower = 0;
            foreach (var companion in party)
            {
                totalPower += companion.strength + companion.magicPower;
            }
            
            // Apply combo multiplier
            int comboDamage = totalPower * combo.powerMultiplier;
            
            // Distribute damage across all enemies
            foreach (var target in targets)
            {
                int damage = comboDamage / targets.Count;
                Debug.Log($"{combo.comboName} deals {damage} damage to {target.name}!");
            }
        }

        /// <summary>
        /// Get description of a special ability
        /// </summary>
        public static string GetAbilityDescription(CompanionSpecialAbility ability)
        {
            switch (ability)
            {
                case CompanionSpecialAbility.MindShatter:
                    return "Rhysand's Daemati power shatters enemy minds, dealing massive psychic damage and stunning them.";
                case CompanionSpecialAbility.DarknessShroud:
                    return "Rhysand cloaks the party in protective darkness, reducing incoming damage.";
                case CompanionSpecialAbility.SiphonBlast:
                    return "Cassian channels power through his siphons for a devastating physical attack.";
                case CompanionSpecialAbility.WarCry:
                    return "Cassian's battle cry inspires the party, increasing everyone's strength.";
                case CompanionSpecialAbility.ShadowStrike:
                    return "Azriel strikes from the shadows with guaranteed critical damage.";
                case CompanionSpecialAbility.TruthTeller:
                    return "Azriel's legendary dagger strikes true, ignoring armor.";
                case CompanionSpecialAbility.TruthBurst:
                    return "Mor unleashes her Truth magic in a devastating area attack.";
                case CompanionSpecialAbility.SilverFlames:
                    return "Nesta channels the death magic stolen from the Cauldron.";
                case CompanionSpecialAbility.PropheticVision:
                    return "Elain's Seer abilities reveal enemy weaknesses and future attacks.";
                default:
                    return "A powerful special ability.";
            }
        }
    }
}
