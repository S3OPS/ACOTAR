using UnityEngine;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Character titles earned through gameplay
    /// NEW FEATURE: Achievement-based title system
    /// </summary>
    public enum CharacterTitle
    {
        None,
        
        // Base Game Titles (Book 1)
        MortalHuntress,         // Start as human
        BeastSlayer,            // Kill a powerful Fae creature
        Survivor,               // Complete first trial
        RiddleSolver,           // Complete second trial
        Heartbreaker,           // Complete third trial
        CurseBreaker,           // Defeat Amarantha
        HighFae,                // Transform into High Fae
        
        // DLC 1 Titles (Book 2)
        NightCourtResident,     // Join Night Court
        InnerCircleMember,      // Become part of Inner Circle
        HighLadyOfNight,        // First High Lady
        Valkyrie,               // Begin Valkyrie training
        CourtOfDreams,          // Master role in Court
        
        // DLC 2 Titles (Book 3)
        Spy,                    // Infiltrate Spring Court
        Diplomat,               // Unite the courts
        Warlord,                // Lead armies
        CauldronBorn,           // Master Cauldron-given powers
        SaviorOfPrythian,       // Win the war
        
        // Special Achievement Titles
        Loremaster,             // Complete all side quests
        CompanionOfLegends,     // Max loyalty with all companions
        MasterCrafter,          // Master all crafting recipes
        ExplorerOfCourts,       // Visit all seven courts
        UltimateWarrior         // Complete on Nightmare difficulty
    }

    /// <summary>
    /// Mastery levels for different skill categories
    /// NEW FEATURE: Skill mastery tracking
    /// </summary>
    public enum SkillMastery
    {
        Novice,         // 0-24
        Apprentice,     // 25-49
        Adept,          // 50-74
        Expert,         // 75-99
        Master,         // 100-149
        GrandMaster     // 150+
    }

    /// <summary>
    /// Different skill categories players can develop
    /// </summary>
    public enum SkillCategory
    {
        Combat,         // Physical fighting
        Magic,          // Magical abilities
        Stealth,        // Sneaking and subterfuge
        Diplomacy,      // Social interactions
        Crafting,       // Creating items
        Exploration     // Finding secrets
    }

    /// <summary>
    /// Enhanced character progression system
    /// Tracks titles, skill mastery, and provides milestone rewards
    /// </summary>
    [System.Serializable]
    public class CharacterProgression
    {
        // Current titles earned
        public List<CharacterTitle> earnedTitles;
        public CharacterTitle activeTitle;
        
        // Skill experience tracking
        public Dictionary<SkillCategory, int> skillExperience;
        
        // Progression milestones
        public int questsCompleted;
        public int enemiesDefeated;
        public int itemsCrafted;
        public int locationsDiscovered;
        public int secretsFound;
        
        // Statistics
        public float playtime;  // In hours
        public int deaths;
        public int companionsRecruited;
        public int dialoguesCompleted;

        public CharacterProgression()
        {
            earnedTitles = new List<CharacterTitle>();
            activeTitle = CharacterTitle.None;
            skillExperience = new Dictionary<SkillCategory, int>
            {
                { SkillCategory.Combat, 0 },
                { SkillCategory.Magic, 0 },
                { SkillCategory.Stealth, 0 },
                { SkillCategory.Diplomacy, 0 },
                { SkillCategory.Crafting, 0 },
                { SkillCategory.Exploration, 0 }
            };
            
            questsCompleted = 0;
            enemiesDefeated = 0;
            itemsCrafted = 0;
            locationsDiscovered = 0;
            secretsFound = 0;
            playtime = 0f;
            deaths = 0;
            companionsRecruited = 0;
            dialoguesCompleted = 0;
        }

        /// <summary>
        /// Earn a new title
        /// </summary>
        public void EarnTitle(CharacterTitle title)
        {
            if (!earnedTitles.Contains(title))
            {
                earnedTitles.Add(title);
                Debug.Log($"<color=gold>TITLE EARNED: {GetTitleName(title)}</color>");
                Debug.Log($"<color=yellow>{GetTitleDescription(title)}</color>");
                
                // Automatically set as active title if first one
                if (earnedTitles.Count == 1)
                {
                    activeTitle = title;
                }
                
                // Grant title bonuses
                ApplyTitleBonus(title);
            }
        }

        /// <summary>
        /// Gain experience in a skill category
        /// </summary>
        public void GainSkillExperience(SkillCategory skill, int amount)
        {
            int oldExperience = skillExperience[skill];
            skillExperience[skill] += amount;
            
            SkillMastery oldMastery = GetSkillMastery(oldExperience);
            SkillMastery newMastery = GetSkillMastery(skillExperience[skill]);
            
            if (newMastery > oldMastery)
            {
                Debug.Log($"<color=cyan>SKILL MASTERY INCREASED!</color>");
                Debug.Log($"{skill} is now {newMastery} level!");
                
                // Grant mastery bonus
                ApplyMasteryBonus(skill, newMastery);
            }
        }

        /// <summary>
        /// Calculate skill mastery level from experience
        /// </summary>
        private SkillMastery GetSkillMastery(int experience)
        {
            if (experience >= 150) return SkillMastery.GrandMaster;
            if (experience >= 100) return SkillMastery.Master;
            if (experience >= 75) return SkillMastery.Expert;
            if (experience >= 50) return SkillMastery.Adept;
            if (experience >= 25) return SkillMastery.Apprentice;
            return SkillMastery.Novice;
        }

        /// <summary>
        /// Get display name for a title
        /// </summary>
        public string GetTitleName(CharacterTitle title)
        {
            switch (title)
            {
                case CharacterTitle.MortalHuntress: return "Mortal Huntress";
                case CharacterTitle.BeastSlayer: return "Beast Slayer";
                case CharacterTitle.Survivor: return "Survivor of Trials";
                case CharacterTitle.RiddleSolver: return "Riddle Solver";
                case CharacterTitle.Heartbreaker: return "Heartbreaker";
                case CharacterTitle.CurseBreaker: return "Curse Breaker";
                case CharacterTitle.HighFae: return "High Fae";
                case CharacterTitle.NightCourtResident: return "Resident of the Night Court";
                case CharacterTitle.InnerCircleMember: return "Inner Circle Member";
                case CharacterTitle.HighLadyOfNight: return "High Lady of the Night Court";
                case CharacterTitle.Valkyrie: return "Valkyrie";
                case CharacterTitle.CourtOfDreams: return "Court of Dreams";
                case CharacterTitle.Spy: return "The Spy";
                case CharacterTitle.Diplomat: return "Uniter of Courts";
                case CharacterTitle.Warlord: return "Warlord of Prythian";
                case CharacterTitle.CauldronBorn: return "Cauldron-Born";
                case CharacterTitle.SaviorOfPrythian: return "Savior of Prythian";
                case CharacterTitle.Loremaster: return "Loremaster";
                case CharacterTitle.CompanionOfLegends: return "Companion of Legends";
                case CharacterTitle.MasterCrafter: return "Master Crafter";
                case CharacterTitle.ExplorerOfCourts: return "Explorer of the Seven Courts";
                case CharacterTitle.UltimateWarrior: return "Ultimate Warrior";
                default: return "No Title";
            }
        }

        /// <summary>
        /// Get description for a title
        /// </summary>
        public string GetTitleDescription(CharacterTitle title)
        {
            switch (title)
            {
                case CharacterTitle.MortalHuntress:
                    return "You began as a mortal huntress in the human lands.";
                case CharacterTitle.CurseBreaker:
                    return "You broke Amarantha's curse and freed Prythian!";
                case CharacterTitle.HighLadyOfNight:
                    return "You became the first High Lady in Prythian's history!";
                case CharacterTitle.SaviorOfPrythian:
                    return "You led the forces that defeated Hybern and saved all of Prythian!";
                case CharacterTitle.Loremaster:
                    return "You have discovered all the lore and secrets of Prythian.";
                default:
                    return "A title of achievement.";
            }
        }

        /// <summary>
        /// Apply bonuses for earning a title
        /// </summary>
        private void ApplyTitleBonus(CharacterTitle title)
        {
            // Titles could grant stat bonuses, gold, items, etc.
            switch (title)
            {
                case CharacterTitle.CurseBreaker:
                    Debug.Log("+100 bonus to all stats!");
                    Debug.Log("+1000 Gold reward!");
                    break;
                case CharacterTitle.HighLadyOfNight:
                    Debug.Log("+50 Magic Power!");
                    Debug.Log("Unlocked: High Lady abilities!");
                    break;
                case CharacterTitle.MasterCrafter:
                    Debug.Log("Crafting costs reduced by 25%!");
                    Debug.Log("Crafting speed increased by 50%!");
                    break;
            }
        }

        /// <summary>
        /// Apply bonuses for skill mastery level up
        /// </summary>
        private void ApplyMasteryBonus(SkillCategory skill, SkillMastery mastery)
        {
            int bonus = GetMasteryBonus(mastery);
            
            switch (skill)
            {
                case SkillCategory.Combat:
                    Debug.Log($"+{bonus} Strength and Agility!");
                    break;
                case SkillCategory.Magic:
                    Debug.Log($"+{bonus} Magic Power!");
                    break;
                case SkillCategory.Diplomacy:
                    Debug.Log($"Dialogue options improved!");
                    Debug.Log($"Shop prices reduced by {bonus * 2}%!");
                    break;
                case SkillCategory.Crafting:
                    Debug.Log($"New recipes unlocked!");
                    Debug.Log($"Crafting success rate +{bonus * 5}%!");
                    break;
            }
        }

        /// <summary>
        /// Get stat bonus for mastery level
        /// </summary>
        private int GetMasteryBonus(SkillMastery mastery)
        {
            switch (mastery)
            {
                case SkillMastery.Novice: return 0;
                case SkillMastery.Apprentice: return 5;
                case SkillMastery.Adept: return 10;
                case SkillMastery.Expert: return 20;
                case SkillMastery.Master: return 35;
                case SkillMastery.GrandMaster: return 50;
                default: return 0;
            }
        }

        /// <summary>
        /// Update a statistic
        /// </summary>
        public void UpdateStatistic(string statName, int value = 1)
        {
            switch (statName.ToLower())
            {
                case "quest":
                    questsCompleted += value;
                    GainSkillExperience(SkillCategory.Exploration, 2);
                    CheckTitleProgress();
                    break;
                case "enemy":
                    enemiesDefeated += value;
                    GainSkillExperience(SkillCategory.Combat, 1);
                    break;
                case "craft":
                    itemsCrafted += value;
                    GainSkillExperience(SkillCategory.Crafting, 1);
                    break;
                case "location":
                    locationsDiscovered += value;
                    GainSkillExperience(SkillCategory.Exploration, 3);
                    break;
                case "secret":
                    secretsFound += value;
                    GainSkillExperience(SkillCategory.Exploration, 5);
                    break;
                case "companion":
                    companionsRecruited += value;
                    GainSkillExperience(SkillCategory.Diplomacy, 5);
                    break;
                case "dialogue":
                    dialoguesCompleted += value;
                    GainSkillExperience(SkillCategory.Diplomacy, 1);
                    break;
                case "death":
                    deaths += value;
                    break;
            }
        }

        /// <summary>
        /// Check if player qualifies for any new titles
        /// </summary>
        private void CheckTitleProgress()
        {
            // Loremaster: Complete 50+ quests
            if (questsCompleted >= 50 && !earnedTitles.Contains(CharacterTitle.Loremaster))
            {
                EarnTitle(CharacterTitle.Loremaster);
            }
            
            // Explorer: Discover all 20+ locations
            if (locationsDiscovered >= 20 && !earnedTitles.Contains(CharacterTitle.ExplorerOfCourts))
            {
                EarnTitle(CharacterTitle.ExplorerOfCourts);
            }
            
            // Master Crafter: Craft 100+ items
            if (itemsCrafted >= 100 && !earnedTitles.Contains(CharacterTitle.MasterCrafter))
            {
                EarnTitle(CharacterTitle.MasterCrafter);
            }
        }

        /// <summary>
        /// Get progress summary for player
        /// </summary>
        public string GetProgressSummary()
        {
            return $@"
=== CHARACTER PROGRESSION ===
Active Title: {GetTitleName(activeTitle)}
Titles Earned: {earnedTitles.Count}

STATISTICS:
Quests Completed: {questsCompleted}
Enemies Defeated: {enemiesDefeated}
Items Crafted: {itemsCrafted}
Locations Discovered: {locationsDiscovered}
Secrets Found: {secretsFound}
Companions Recruited: {companionsRecruited}
Deaths: {deaths}
Playtime: {playtime:F1} hours

SKILL MASTERY:
Combat: {GetSkillMastery(skillExperience[SkillCategory.Combat])} ({skillExperience[SkillCategory.Combat]} XP)
Magic: {GetSkillMastery(skillExperience[SkillCategory.Magic])} ({skillExperience[SkillCategory.Magic]} XP)
Stealth: {GetSkillMastery(skillExperience[SkillCategory.Stealth])} ({skillExperience[SkillCategory.Stealth]} XP)
Diplomacy: {GetSkillMastery(skillExperience[SkillCategory.Diplomacy])} ({skillExperience[SkillCategory.Diplomacy]} XP)
Crafting: {GetSkillMastery(skillExperience[SkillCategory.Crafting])} ({skillExperience[SkillCategory.Crafting]} XP)
Exploration: {GetSkillMastery(skillExperience[SkillCategory.Exploration])} ({skillExperience[SkillCategory.Exploration]} XP)
";
        }
    }
}
