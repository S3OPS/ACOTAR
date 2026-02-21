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
    /// v2.3.3: Now actually applies stat bonuses to character
    /// </summary>
    [System.Serializable]
    public class CharacterProgression
    {
        // Reference to the character (v2.3.3: NEW)
        private Character _character;

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
        /// Set character reference (v2.3.3: NEW)
        /// Must be called after character creation
        /// </summary>
        /// <summary>
        /// Set the character reference for this progression system
        /// Enhanced in v2.6.5: Added validation and logging
        /// </summary>
        /// <param name="character">The character to track progression for</param>
        /// <remarks>
        /// This method must be called before any bonuses can be applied.
        /// Bonuses from titles and mastery require a valid character reference.
        /// Setting a null character will be logged as a warning but not throw an error.
        /// </remarks>
        public void SetCharacter(Character character)
        {
            try
            {
                if (character == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "CharacterProgression", "Setting character reference to null - bonuses will not be applied");
                }
                else
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                        "CharacterProgression", $"Character reference set: {character.name}");
                }
                
                _character = character;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "CharacterProgression", $"Exception in SetCharacter: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Earn a new title
        /// Enhanced in v2.6.5: Added comprehensive error handling and logging
        /// </summary>
        /// <param name="title">The title to earn</param>
        /// <remarks>
        /// Titles are permanent achievements that grant stat bonuses.
        /// The first earned title is automatically set as the active title.
        /// Title bonuses are applied immediately through ApplyTitleBonus.
        /// Duplicate titles are safely ignored without error.
        /// If the title list is null, it will be initialized to prevent crashes.
        /// </remarks>
        public void EarnTitle(CharacterTitle title)
        {
            try
            {
                // Ensure earned titles list is initialized
                if (earnedTitles == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "CharacterProgression", "Earned titles list was null, initializing");
                    earnedTitles = new List<CharacterTitle>();
                }
                
                if (!earnedTitles.Contains(title))
                {
                    earnedTitles.Add(title);
                    
                    string titleName = GetTitleName(title);
                    string titleDesc = GetTitleDescription(title);
                    
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "CharacterProgression", $"TITLE EARNED: {titleName}\n{titleDesc}");
                    
                    // Automatically set as active title if first one
                    if (earnedTitles.Count == 1)
                    {
                        activeTitle = title;
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                            "CharacterProgression", $"Set {titleName} as active title (first title earned)");
                    }
                    
                    // Grant title bonuses with protection
                    try
                    {
                        ApplyTitleBonus(title);
                    }
                    catch (System.Exception bonusEx)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "CharacterProgression", $"Exception applying title bonus for {title}: {bonusEx.Message}");
                    }
                }
                else
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                        "CharacterProgression", $"Title {title} already earned, skipping");
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "CharacterProgression", $"Exception in EarnTitle: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Gain experience in a skill category
        /// Enhanced in v2.6.5: Added comprehensive error handling and logging
        /// </summary>
        /// <param name="skill">The skill category to gain experience in</param>
        /// <param name="amount">Amount of experience to gain</param>
        /// <remarks>
        /// Skill experience accumulates to increase mastery levels.
        /// Mastery level ups grant stat bonuses appropriate to the skill category.
        /// Experience amounts are validated to prevent negative values.
        /// If the skill experience dictionary is null, it will be initialized.
        /// Mastery bonuses are applied through ApplyMasteryBonus with error protection.
        /// </remarks>
        public void GainSkillExperience(SkillCategory skill, int amount)
        {
            try
            {
                // Ensure skill experience dictionary is initialized
                if (skillExperience == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "CharacterProgression", "Skill experience dictionary was null, initializing");
                    skillExperience = new Dictionary<SkillCategory, int>();
                    // Initialize all skill categories
                    foreach (SkillCategory category in System.Enum.GetValues(typeof(SkillCategory)))
                    {
                        skillExperience[category] = 0;
                    }
                }
                
                // Ensure this skill category is in the dictionary
                if (!skillExperience.ContainsKey(skill))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "CharacterProgression", $"Skill category {skill} not in dictionary, adding");
                    skillExperience[skill] = 0;
                }
                
                // Validate amount
                if (amount < 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "CharacterProgression", $"Cannot gain negative skill experience: {amount}");
                    return;
                }
                
                int oldExperience = skillExperience[skill];
                skillExperience[skill] += amount;
                
                SkillMastery oldMastery = GetSkillMastery(oldExperience);
                SkillMastery newMastery = GetSkillMastery(skillExperience[skill]);
                
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                    "CharacterProgression", $"Gained {amount} {skill} experience (total: {skillExperience[skill]})");
                
                if (newMastery > oldMastery)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "CharacterProgression", $"SKILL MASTERY INCREASED! {skill} is now {newMastery} level!");
                    
                    // Grant mastery bonus with protection
                    try
                    {
                        ApplyMasteryBonus(skill, newMastery);
                    }
                    catch (System.Exception bonusEx)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "CharacterProgression", $"Exception applying mastery bonus for {skill} {newMastery}: {bonusEx.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "CharacterProgression", $"Exception in GainSkillExperience: {ex.Message}\nStack: {ex.StackTrace}");
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
        /// Apply bonuses for earning a title (v2.3.3: Now actually applies bonuses!)
        /// </summary>
        private void ApplyTitleBonus(CharacterTitle title)
        {
            if (_character == null)
            {
                Debug.LogWarning("Cannot apply title bonus: Character reference not set");
                return;
            }

            // Apply actual stat bonuses and rewards
            switch (title)
            {
                case CharacterTitle.CurseBreaker:
                    // Major achievement - significant bonuses
                    _character.stats.strength += 100;
                    _character.stats.agility += 100;
                    _character.stats.magicPower += 100;
                    _character.stats.maxHealth += 100;
                    _character.health = _character.maxHealth; // Heal to full
                    
                    CurrencySystem currencySystem = GameManager.Instance?.GetCurrencySystem();
                    if (currencySystem != null)
                    {
                        currencySystem.AddGold(1000);
                    }
                    
                    Debug.Log("+100 to all stats!");
                    Debug.Log("+1000 Gold reward!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification("Title Earned: Curse Breaker!\n+100 All Stats, +1000 Gold", Color.yellow);
                    }
                    break;

                case CharacterTitle.HighLadyOfNight:
                    _character.stats.magicPower += 50;
                    _character.manaSystem.UpdateMaxMana(_character.stats.magicPower);
                    
                    Debug.Log("+50 Magic Power!");
                    Debug.Log("Unlocked: High Lady abilities!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification("Title Earned: High Lady of Night!\n+50 Magic Power", Color.cyan);
                    }
                    break;

                case CharacterTitle.MasterCrafter:
                    // Crafting bonuses are passive, tracked by this title
                    Debug.Log("Crafting costs reduced by 25%!");
                    Debug.Log("Crafting speed increased by 50%!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification("Title Earned: Master Crafter!\nCrafting bonuses unlocked", Color.green);
                    }
                    break;

                case CharacterTitle.HighFae:
                    _character.stats.maxHealth += 50;
                    _character.stats.magicPower += 30;
                    _character.health = _character.maxHealth;
                    
                    Debug.Log("+50 Max Health, +30 Magic Power!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification("Transformed into High Fae!\n+50 HP, +30 Magic", Color.magenta);
                    }
                    break;

                case CharacterTitle.SaviorOfPrythian:
                    // Ultimate achievement
                    _character.stats.strength += 150;
                    _character.stats.agility += 150;
                    _character.stats.magicPower += 150;
                    _character.stats.maxHealth += 150;
                    
                    CurrencySystem currencySystem = GameManager.Instance?.GetCurrencySystem();
                    if (currencySystem != null)
                    {
                        currencySystem.AddGold(5000);
                    }
                    
                    Debug.Log("+150 to all stats!");
                    Debug.Log("+5000 Gold reward!");
                    break;

                case CharacterTitle.CompanionOfLegends:
                    _character.stats.strength += 25;
                    _character.stats.agility += 25;
                    
                    Debug.Log("+25 Strength and Agility!");
                    break;

                default:
                    // Minor titles grant smaller bonuses
                    _character.stats.strength += 10;
                    _character.stats.agility += 10;
                    Debug.Log("+10 Strength and Agility!");
                    break;
            }
        }

        /// <summary>
        /// Apply bonuses for skill mastery level up (v2.3.3: Now actually applies bonuses!)
        /// </summary>
        private void ApplyMasteryBonus(SkillCategory skill, SkillMastery mastery)
        {
            if (_character == null)
            {
                Debug.LogWarning("Cannot apply mastery bonus: Character reference not set");
                return;
            }

            int bonus = GetMasteryBonus(mastery);
            
            switch (skill)
            {
                case SkillCategory.Combat:
                    _character.stats.strength += bonus;
                    _character.stats.agility += bonus;
                    Debug.Log($"+{bonus} Strength and Agility!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"Combat Mastery Up!\n+{bonus} Strength & Agility", Color.red);
                    }
                    break;

                case SkillCategory.Magic:
                    _character.stats.magicPower += bonus;
                    _character.manaSystem.UpdateMaxMana(_character.stats.magicPower);
                    Debug.Log($"+{bonus} Magic Power!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"Magic Mastery Up!\n+{bonus} Magic Power", Color.cyan);
                    }
                    break;

                case SkillCategory.Stealth:
                    _character.stats.agility += bonus * 2; // Stealth heavily benefits agility
                    Debug.Log($"+{bonus * 2} Agility!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"Stealth Mastery Up!\n+{bonus * 2} Agility", Color.gray);
                    }
                    break;

                case SkillCategory.Diplomacy:
                    // Diplomacy provides passive bonuses (shop prices, dialogue options)
                    Debug.Log($"Dialogue options improved!");
                    Debug.Log($"Shop prices reduced by {bonus * 2}%!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"Diplomacy Mastery Up!\n-{bonus * 2}% Shop Prices", Color.yellow);
                    }
                    break;

                case SkillCategory.Crafting:
                    // Crafting provides passive bonuses
                    Debug.Log($"New recipes unlocked!");
                    Debug.Log($"Crafting success rate +{bonus * 5}%!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"Crafting Mastery Up!\n+{bonus * 5}% Success Rate", Color.green);
                    }
                    break;

                case SkillCategory.Exploration:
                    // Exploration grants small all-around bonuses
                    int smallBonus = bonus / 2;
                    _character.stats.strength += smallBonus;
                    _character.stats.agility += smallBonus;
                    _character.stats.magicPower += smallBonus;
                    Debug.Log($"+{smallBonus} to all stats!");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"Exploration Mastery Up!\n+{smallBonus} All Stats", Color.blue);
                    }
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
        /// Enhanced in v2.6.5: Added comprehensive error handling and logging
        /// </summary>
        /// <param name="statName">Name of the statistic to update (quest, enemy, craft, location, secret, companion, dialogue, death)</param>
        /// <param name="value">Value to add to the statistic (default: 1)</param>
        /// <remarks>
        /// This method tracks player achievements and automatically grants skill experience.
        /// All statistic updates are logged for debugging and analytics.
        /// Invalid statistic names are safely ignored with a warning.
        /// Skill experience gains are protected to prevent cascading failures.
        /// CheckTitleProgress is called after quest completion to evaluate title unlocks.
        /// </remarks>
        public void UpdateStatistic(string statName, int value = 1)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(statName))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "CharacterProgression", "Cannot update statistic: statName is null or empty");
                    return;
                }
                
                string normalizedStatName = statName.ToLower();
                
                switch (normalizedStatName)
                {
                    case "quest":
                        questsCompleted += value;
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                            "CharacterProgression", $"Quest completed. Total: {questsCompleted}");
                        
                        try
                        {
                            GainSkillExperience(SkillCategory.Exploration, 2);
                            CheckTitleProgress();
                        }
                        catch (System.Exception ex)
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                                "CharacterProgression", $"Exception in quest stat update: {ex.Message}");
                        }
                        break;
                        
                    case "enemy":
                        enemiesDefeated += value;
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                            "CharacterProgression", $"Enemy defeated. Total: {enemiesDefeated}");
                        
                        try
                        {
                            GainSkillExperience(SkillCategory.Combat, 1);
                        }
                        catch (System.Exception ex)
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                                "CharacterProgression", $"Exception in enemy stat update: {ex.Message}");
                        }
                        break;
                        
                    case "craft":
                        itemsCrafted += value;
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                            "CharacterProgression", $"Item crafted. Total: {itemsCrafted}");
                        
                        try
                        {
                            GainSkillExperience(SkillCategory.Crafting, 1);
                        }
                        catch (System.Exception ex)
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                                "CharacterProgression", $"Exception in craft stat update: {ex.Message}");
                        }
                        break;
                        
                    case "location":
                        locationsDiscovered += value;
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                            "CharacterProgression", $"Location discovered. Total: {locationsDiscovered}");
                        
                        try
                        {
                            GainSkillExperience(SkillCategory.Exploration, 3);
                        }
                        catch (System.Exception ex)
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                                "CharacterProgression", $"Exception in location stat update: {ex.Message}");
                        }
                        break;
                        
                    case "secret":
                        secretsFound += value;
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                            "CharacterProgression", $"Secret found. Total: {secretsFound}");
                        
                        try
                        {
                            GainSkillExperience(SkillCategory.Exploration, 5);
                        }
                        catch (System.Exception ex)
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                                "CharacterProgression", $"Exception in secret stat update: {ex.Message}");
                        }
                        break;
                        
                    case "companion":
                        companionsRecruited += value;
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                            "CharacterProgression", $"Companion recruited. Total: {companionsRecruited}");
                        
                        try
                        {
                            GainSkillExperience(SkillCategory.Diplomacy, 5);
                        }
                        catch (System.Exception ex)
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                                "CharacterProgression", $"Exception in companion stat update: {ex.Message}");
                        }
                        break;
                        
                    case "dialogue":
                        dialoguesCompleted += value;
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                            "CharacterProgression", $"Dialogue completed. Total: {dialoguesCompleted}");
                        
                        try
                        {
                            GainSkillExperience(SkillCategory.Diplomacy, 1);
                        }
                        catch (System.Exception ex)
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                                "CharacterProgression", $"Exception in dialogue stat update: {ex.Message}");
                        }
                        break;
                        
                    case "death":
                        deaths += value;
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                            "CharacterProgression", $"Death recorded. Total: {deaths}");
                        break;
                        
                    default:
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                            "CharacterProgression", $"Unknown statistic name: {statName}");
                        break;
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "CharacterProgression", $"Exception in UpdateStatistic: {ex.Message}\nStack: {ex.StackTrace}");
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
