using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Achievement tracking and management system
    /// Tracks player accomplishments and milestones
    /// Phase 10 - Book 1 Polish
    /// </summary>
    public class AchievementSystem : MonoBehaviour
    {
        public static AchievementSystem Instance { get; private set; }

        [Header("Achievement Data")]
        private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
        private HashSet<string> unlockedAchievements = new HashSet<string>();

        [Header("Events")]
        public event Action<Achievement> OnAchievementUnlocked;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAchievements();
                LoadProgress();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initialize all achievements
        /// </summary>
        private void InitializeAchievements()
        {
            // Story Achievements
            AddAchievement("beyond_the_wall", "Beyond the Wall", 
                "Complete your first quest and venture into Prythian", 
                AchievementCategory.Story, 10);

            AddAchievement("beasts_captive", "Beast's Captive", 
                "Arrive at the Spring Court Manor", 
                AchievementCategory.Story, 10);

            AddAchievement("fire_night", "Fire Night", 
                "Witness the ancient ritual of Calanmai", 
                AchievementCategory.Story, 15);

            AddAchievement("into_darkness", "Into Darkness", 
                "Enter Under the Mountain for the first time", 
                AchievementCategory.Story, 15);

            AddAchievement("first_trial_complete", "Trial by Combat", 
                "Defeat the Middengard Wyrm in the first trial", 
                AchievementCategory.Story, 20);

            AddAchievement("second_trial_complete", "Riddle of the Naga", 
                "Survive the second trial and the Naga's poison", 
                AchievementCategory.Story, 20);

            AddAchievement("hearts_of_stone", "Hearts of Stone", 
                "Complete the third trial and make an impossible choice", 
                AchievementCategory.Story, 25);

            AddAchievement("answer_is_love", "The Answer is Love", 
                "Solve Amarantha's riddle and break the curse", 
                AchievementCategory.Story, 30);

            AddAchievement("made_by_seven", "Made by Seven", 
                "Be resurrected and transformed by the seven High Lords", 
                AchievementCategory.Story, 40);

            AddAchievement("curse_breaker", "Curse Breaker", 
                "Defeat Amarantha and free all of Prythian", 
                AchievementCategory.Story, 50);

            AddAchievement("bargain_kept", "A Bargain Kept", 
                "Complete Book 1 and honor your bargain with Rhysand", 
                AchievementCategory.Story, 50);

            // Combat Achievements
            AddAchievement("first_blood", "First Blood", 
                "Win your first combat encounter", 
                AchievementCategory.Combat, 5);

            AddAchievement("flawless_victory", "Flawless Victory", 
                "Win a combat without taking any damage", 
                AchievementCategory.Combat, 20);

            AddAchievement("critical_master", "Critical Master", 
                "Land 50 critical hits in combat", 
                AchievementCategory.Combat, 25);

            AddAchievement("magic_prodigy", "Magic Prodigy", 
                "Use magic abilities 100 times", 
                AchievementCategory.Combat, 25);

            AddAchievement("survivor", "Survivor", 
                "Successfully flee from a dangerous combat", 
                AchievementCategory.Combat, 10);

            AddAchievement("legendary_fighter", "Legendary Fighter", 
                "Defeat a boss on Hard difficulty or above", 
                AchievementCategory.Combat, 30);

            AddAchievement("unstoppable", "Unstoppable", 
                "Win 50 combat encounters", 
                AchievementCategory.Combat, 20);

            // Exploration Achievements
            AddAchievement("court_hopper", "Court Hopper", 
                "Visit all seven High Fae courts", 
                AchievementCategory.Exploration, 25);

            AddAchievement("cartographer", "Cartographer", 
                "Unlock all Book 1 locations", 
                AchievementCategory.Exploration, 20);

            AddAchievement("well_traveled", "Well Traveled", 
                "Travel between locations 100 times", 
                AchievementCategory.Exploration, 15);

            AddAchievement("under_mountain", "Under the Mountain", 
                "Discover the underground realm beneath the mountain", 
                AchievementCategory.Exploration, 15);

            // Companion Achievements
            AddAchievement("dream_lover", "Dream Lover", 
                "Recruit Rhysand to your cause", 
                AchievementCategory.Companion, 20);

            AddAchievement("springs_warrior", "Spring's Warrior", 
                "Recruit Lucien to your cause", 
                AchievementCategory.Companion, 15);

            AddAchievement("full_party", "Full Party", 
                "Have 3 active companions simultaneously", 
                AchievementCategory.Companion, 15);

            AddAchievement("loyal_friend", "Loyal Friend", 
                "Max out loyalty with any companion (100)", 
                AchievementCategory.Companion, 25);

            AddAchievement("everyones_friend", "Everyone's Friend", 
                "Recruit all available Book 1 companions", 
                AchievementCategory.Companion, 30);

            // Collection Achievements
            AddAchievement("loremaster", "Loremaster", 
                "Read all help topics in the tutorial system", 
                AchievementCategory.Collection, 20);

            AddAchievement("craftsman", "Craftsman", 
                "Craft 10 different items", 
                AchievementCategory.Collection, 15);

            AddAchievement("hoarder", "Hoarder", 
                "Collect 50 unique items in your inventory", 
                AchievementCategory.Collection, 15);

            AddAchievement("wealthy", "Wealthy", 
                "Accumulate 10,000 gold", 
                AchievementCategory.Collection, 20);

            AddAchievement("library_master", "Library Master", 
                "Learn to read and complete all reading quests", 
                AchievementCategory.Collection, 15);

            // Challenge Achievements
            AddAchievement("speedrunner", "Speedrunner", 
                "Complete Book 1 in under 3 hours of playtime", 
                AchievementCategory.Challenge, 40);

            AddAchievement("perfectionist", "Perfectionist", 
                "Complete all Book 1 quests (main and side)", 
                AchievementCategory.Challenge, 35);

            AddAchievement("no_deaths", "Immortal", 
                "Complete Book 1 without dying in combat", 
                AchievementCategory.Challenge, 50);

            AddAchievement("hard_mode_hero", "Hard Mode Hero", 
                "Complete Book 1 on Hard difficulty", 
                AchievementCategory.Challenge, 40);

            AddAchievement("nightmare_victor", "Nightmare Conqueror", 
                "Complete Book 1 on Nightmare difficulty", 
                AchievementCategory.Challenge, 60);

            AddAchievement("minimalist", "Minimalist", 
                "Complete Book 1 using only starting equipment", 
                AchievementCategory.Challenge, 45);

            // Secret Achievements
            AddAchievement("suriels_wisdom", "The Suriel's Wisdom", 
                "Complete the Suriel side quest and gain knowledge", 
                AchievementCategory.Secret, 20, true);

            AddAchievement("bone_carver_gift", "Bone Carver's Gift", 
                "Complete the Bone Carver side quest", 
                AchievementCategory.Secret, 20, true);

            AddAchievement("court_of_nightmares", "Court of Nightmares", 
                "Visit the dark city of Hewn City", 
                AchievementCategory.Secret, 20, true);

            AddAchievement("memory_of_starlight", "Memory of Starlight", 
                "Experience Rhysand's memory of Starfall", 
                AchievementCategory.Secret, 25, true);

            AddAchievement("painter", "The Artist", 
                "Complete all painting side quests", 
                AchievementCategory.Secret, 15, true);

            AddAchievement("transformation", "The Transformation", 
                "Witness the complete transformation from human to High Fae", 
                AchievementCategory.Secret, 30, true);

            Debug.Log($"AchievementSystem: Initialized {achievements.Count} achievements");
        }

        /// <summary>
        /// Add an achievement to the system
        /// </summary>
        private void AddAchievement(string id, string name, string description, 
            AchievementCategory category, int points, bool isSecret = false)
        {
            Achievement achievement = new Achievement
            {
                id = id,
                name = name,
                description = description,
                category = category,
                points = points,
                isSecret = isSecret,
                unlockedDate = null
            };
            
            achievements[id] = achievement;
        }

        /// <summary>
        /// Unlock an achievement
        /// </summary>
        public void UnlockAchievement(string achievementId)
        {
            if (!achievements.ContainsKey(achievementId))
            {
                Debug.LogWarning($"Achievement not found: {achievementId}");
                return;
            }

            if (unlockedAchievements.Contains(achievementId))
            {
                return; // Already unlocked
            }

            unlockedAchievements.Add(achievementId);
            Achievement achievement = achievements[achievementId];
            achievement.unlockedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Debug.Log($"🏆 Achievement Unlocked: {achievement.name} ({achievement.points} points)");

            // Show popup notification
            ShowAchievementPopup(achievement);

            // Trigger event
            OnAchievementUnlocked?.Invoke(achievement);

            // Save progress
            SaveProgress();

            // Play sound if AudioManager available
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFXByName("achievement_unlock");
            }
        }

        /// <summary>
        /// Check if an achievement is unlocked
        /// </summary>
        public bool IsAchievementUnlocked(string achievementId)
        {
            return unlockedAchievements.Contains(achievementId);
        }

        /// <summary>
        /// Get achievement data
        /// </summary>
        public Achievement GetAchievement(string achievementId)
        {
            if (achievements.ContainsKey(achievementId))
            {
                return achievements[achievementId];
            }
            return null;
        }

        /// <summary>
        /// Get all achievements
        /// </summary>
        public List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>(achievements.Values);
        }

        /// <summary>
        /// Get achievements by category
        /// </summary>
        public List<Achievement> GetAchievementsByCategory(AchievementCategory category)
        {
            List<Achievement> result = new List<Achievement>();
            foreach (var achievement in achievements.Values)
            {
                if (achievement.category == category)
                {
                    result.Add(achievement);
                }
            }
            return result;
        }

        /// <summary>
        /// Get unlocked achievement count
        /// </summary>
        public int GetUnlockedCount()
        {
            return unlockedAchievements.Count;
        }

        /// <summary>
        /// Get total achievement count
        /// </summary>
        public int GetTotalCount()
        {
            return achievements.Count;
        }

        /// <summary>
        /// Get completion percentage
        /// </summary>
        public float GetCompletionPercentage()
        {
            if (achievements.Count == 0) return 0f;
            return (float)unlockedAchievements.Count / achievements.Count * 100f;
        }

        /// <summary>
        /// Get total points earned
        /// </summary>
        public int GetTotalPointsEarned()
        {
            int total = 0;
            foreach (string achievementId in unlockedAchievements)
            {
                if (achievements.ContainsKey(achievementId))
                {
                    total += achievements[achievementId].points;
                }
            }
            return total;
        }

        /// <summary>
        /// Get maximum possible points
        /// </summary>
        public int GetMaxPoints()
        {
            int total = 0;
            foreach (var achievement in achievements.Values)
            {
                total += achievement.points;
            }
            return total;
        }

        /// <summary>
        /// Show achievement unlock popup
        /// </summary>
        private void ShowAchievementPopup(Achievement achievement)
        {
            // Integration with UI system
            if (UIManager.Instance != null)
            {
                string message = $"🏆 Achievement Unlocked!\n\n{achievement.name}\n{achievement.description}\n+{achievement.points} points";
                UIManager.Instance.ShowNotification(message);
            }
        }

        #region Progress Tracking Helpers

        /// <summary>
        /// Track combat wins for achievements
        /// </summary>
        public void TrackCombatWin(bool flawless, int criticalHits)
        {
            IncrementProgress("combat_wins");
            
            if (GetProgress("combat_wins") == 1)
            {
                UnlockAchievement("first_blood");
            }
            
            if (GetProgress("combat_wins") >= 50)
            {
                UnlockAchievement("unstoppable");
            }

            if (flawless)
            {
                UnlockAchievement("flawless_victory");
            }

            IncrementProgress("critical_hits", criticalHits);
            if (GetProgress("critical_hits") >= 50)
            {
                UnlockAchievement("critical_master");
            }
        }

        /// <summary>
        /// Track magic usage
        /// </summary>
        public void TrackMagicUse()
        {
            IncrementProgress("magic_uses");
            if (GetProgress("magic_uses") >= 100)
            {
                UnlockAchievement("magic_prodigy");
            }
        }

        /// <summary>
        /// Track travel
        /// </summary>
        public void TrackTravel()
        {
            IncrementProgress("travels");
            if (GetProgress("travels") >= 100)
            {
                UnlockAchievement("well_traveled");
            }
        }

        /// <summary>
        /// Track crafting
        /// </summary>
        public void TrackCrafting(string itemId)
        {
            AddToSet("crafted_items", itemId);
            if (GetSetCount("crafted_items") >= 10)
            {
                UnlockAchievement("craftsman");
            }
        }

        // Progress tracking storage
        private Dictionary<string, int> progressCounters = new Dictionary<string, int>();
        private Dictionary<string, HashSet<string>> progressSets = new Dictionary<string, HashSet<string>>();

        private void IncrementProgress(string key, int amount = 1)
        {
            if (!progressCounters.ContainsKey(key))
            {
                progressCounters[key] = 0;
            }
            progressCounters[key] += amount;
        }

        private int GetProgress(string key)
        {
            return progressCounters.ContainsKey(key) ? progressCounters[key] : 0;
        }

        private void AddToSet(string key, string value)
        {
            if (!progressSets.ContainsKey(key))
            {
                progressSets[key] = new HashSet<string>();
            }
            progressSets[key].Add(value);
        }

        private int GetSetCount(string key)
        {
            return progressSets.ContainsKey(key) ? progressSets[key].Count : 0;
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Save achievement progress
        /// </summary>
        private void SaveProgress()
        {
            string json = JsonUtility.ToJson(new AchievementSaveData
            {
                unlockedIds = new List<string>(unlockedAchievements)
            });
            
            PlayerPrefs.SetString("AchievementProgress", json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load achievement progress
        /// </summary>
        private void LoadProgress()
        {
            if (PlayerPrefs.HasKey("AchievementProgress"))
            {
                string json = PlayerPrefs.GetString("AchievementProgress");
                AchievementSaveData data = JsonUtility.FromJson<AchievementSaveData>(json);
                
                unlockedAchievements = new HashSet<string>(data.unlockedIds);
                
                // Update achievement unlock dates
                foreach (string id in unlockedAchievements)
                {
                    if (achievements.ContainsKey(id) && string.IsNullOrEmpty(achievements[id].unlockedDate))
                    {
                        achievements[id].unlockedDate = "Previously unlocked";
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Achievement data structure
    /// </summary>
    [Serializable]
    public class Achievement
    {
        public string id;
        public string name;
        public string description;
        public AchievementCategory category;
        public int points;
        public bool isSecret;
        public string unlockedDate;

        public bool IsUnlocked()
        {
            return !string.IsNullOrEmpty(unlockedDate);
        }
    }

    /// <summary>
    /// Achievement categories
    /// </summary>
    public enum AchievementCategory
    {
        Story,
        Combat,
        Exploration,
        Companion,
        Collection,
        Challenge,
        Secret
    }

    /// <summary>
    /// Achievement save data
    /// </summary>
    [Serializable]
    public class AchievementSaveData
    {
        public List<string> unlockedIds;
    }
}
