using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Available DLC packages for ACOTAR
    /// Base game is Book 1: A Court of Thorns and Roses
    /// </summary>
    public enum DLCPackage
    {
        /// <summary>
        /// DLC 1: A Court of Mist and Fury (Book 2)
        /// Night Court, Inner Circle, Summer Court heist, Hybern introduction
        /// </summary>
        ACOMAF_MistAndFury,

        /// <summary>
        /// DLC 2: A Court of Wings and Ruin (Book 3)
        /// War with Hybern, Alliance building, Final battle
        /// </summary>
        ACOWAR_WingsAndRuin
    }

    /// <summary>
    /// Information about a specific DLC package
    /// </summary>
    [Serializable]
    public class DLCInfo
    {
        public DLCPackage package;
        public string name;
        public string description;
        public string version;
        public bool isOwned;
        public bool isInstalled;
        public int questCount;
        public int experienceAvailable;
        public List<string> newLocations;
        public List<string> newCharacters;
        public List<string> newAbilities;
        public StoryArc[] storyArcs;

        public DLCInfo(DLCPackage pkg, string name, string description)
        {
            this.package = pkg;
            this.name = name;
            this.description = description;
            this.version = "1.0.0";
            this.isOwned = false;
            this.isInstalled = false;
            this.newLocations = new List<string>();
            this.newCharacters = new List<string>();
            this.newAbilities = new List<string>();
        }
    }

    /// <summary>
    /// Manages DLC content for ACOTAR
    /// 
    /// Game Structure:
    /// - BASE GAME: Book 1 - A Court of Thorns and Roses
    ///   - Complete story from Human Lands through Under the Mountain
    ///   - 20+ quests, main antagonist Amarantha
    ///   - Transformation into High Fae
    /// 
    /// - DLC 1: Book 2 - A Court of Mist and Fury
    ///   - Night Court and Inner Circle
    ///   - Training and power development
    ///   - Summer Court mission
    ///   - Hybern confrontation
    ///   - Becoming High Lady
    /// 
    /// - DLC 2: Book 3 - A Court of Wings and Ruin
    ///   - Spring Court infiltration
    ///   - Alliance building
    ///   - War with Hybern
    ///   - Final battle
    /// </summary>
    public class DLCManager : MonoBehaviour
    {
        public static DLCManager Instance { get; private set; }

        private Dictionary<DLCPackage, DLCInfo> dlcRegistry;
        
        /// <summary>
        /// Event fired when DLC is purchased/activated
        /// </summary>
        public event Action<DLCPackage> OnDLCActivated;
        
        /// <summary>
        /// Event fired when DLC content is loaded
        /// </summary>
        public event Action<DLCPackage> OnDLCLoaded;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDLCRegistry();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initialize the DLC registry with all available DLC packages
        /// </summary>
        private void InitializeDLCRegistry()
        {
            dlcRegistry = new Dictionary<DLCPackage, DLCInfo>();

            // DLC 1: A Court of Mist and Fury
            DLCInfo acomaf = new DLCInfo(
                DLCPackage.ACOMAF_MistAndFury,
                "A Court of Mist and Fury",
                "Continue Feyre's journey as she discovers the Night Court, meets the Inner Circle, " +
                "and learns the truth about Rhysand. Face new challenges including the theft from " +
                "the Summer Court and the first encounter with Hybern. Become the first High Lady in Prythian history."
            );
            acomaf.questCount = 31;
            acomaf.experienceAvailable = 11350;
            acomaf.newLocations.AddRange(new[] { 
                "Velaris", "House of Wind", "Hewn City", "Illyrian Mountains",
                "Summer Court", "Adriata", "Hybern", "Mortal Lands"
            });
            acomaf.newCharacters.AddRange(new[] {
                "Cassian", "Azriel", "Mor", "Amren", "Tarquin",
                "Nesta", "Elain", "King of Hybern"
            });
            acomaf.newAbilities.AddRange(new[] {
                "Advanced Winnowing", "Mental Shields", "High Lady Powers"
            });
            acomaf.storyArcs = new[] { 
                StoryArc.Book2_NightCourt, 
                StoryArc.Book2_WarPreparations, 
                StoryArc.Book2_Hybern 
            };
            dlcRegistry[DLCPackage.ACOMAF_MistAndFury] = acomaf;

            // DLC 2: A Court of Wings and Ruin
            DLCInfo acowar = new DLCInfo(
                DLCPackage.ACOWAR_WingsAndRuin,
                "A Court of Wings and Ruin",
                "The war for Prythian begins. Infiltrate the Spring Court as a spy, unite the seven courts " +
                "against Hybern, and lead the final battle for the fate of the world. Experience the epic " +
                "conclusion to the original trilogy."
            );
            acowar.questCount = 30;
            acowar.experienceAvailable = 12900;
            acowar.newLocations.AddRange(new[] {
                "Dawn Court", "Day Court", "The Battlefield", "The Prison",
                "Rebuilt Velaris", "New Prythian"
            });
            acowar.newCharacters.AddRange(new[] {
                "Thesan", "Helion", "Kallias", "Viviane", "Beron", "Eris",
                "Bryaxis", "Bone Carver (freed)", "Vassa", "Jurian"
            });
            acowar.newAbilities.AddRange(new[] {
                "Alliance Leadership", "War Strategy", "United Court Magic"
            });
            acowar.storyArcs = new[] {
                StoryArc.Book3_Alliance,
                StoryArc.Book3_War,
                StoryArc.Book3_Resolution
            };
            dlcRegistry[DLCPackage.ACOWAR_WingsAndRuin] = acowar;

            Debug.Log("DLC Registry initialized with 2 DLC packages.");
        }

        /// <summary>
        /// Check if a specific DLC is owned by the player
        /// </summary>
        public bool IsDLCOwned(DLCPackage package)
        {
            return dlcRegistry.ContainsKey(package) && dlcRegistry[package].isOwned;
        }

        /// <summary>
        /// Check if a specific DLC is installed and active
        /// </summary>
        public bool IsDLCInstalled(DLCPackage package)
        {
            return dlcRegistry.ContainsKey(package) && dlcRegistry[package].isInstalled;
        }

        /// <summary>
        /// Get information about a specific DLC
        /// </summary>
        public DLCInfo GetDLCInfo(DLCPackage package)
        {
            return dlcRegistry.ContainsKey(package) ? dlcRegistry[package] : null;
        }

        /// <summary>
        /// Get all available DLC packages
        /// </summary>
        public List<DLCInfo> GetAllDLC()
        {
            return new List<DLCInfo>(dlcRegistry.Values);
        }

        /// <summary>
        /// Activate/purchase a DLC package
        /// In a real implementation, this would verify purchase through a store
        /// </summary>
        public bool ActivateDLC(DLCPackage package)
        {
            if (!dlcRegistry.ContainsKey(package))
            {
                Debug.LogWarning($"DLC package not found: {package}");
                return false;
            }

            // Check prerequisites
            if (package == DLCPackage.ACOWAR_WingsAndRuin && !IsDLCOwned(DLCPackage.ACOMAF_MistAndFury))
            {
                Debug.LogWarning("Cannot activate Wings and Ruin without owning Mist and Fury DLC.");
                return false;
            }

            DLCInfo info = dlcRegistry[package];
            info.isOwned = true;
            Debug.Log($"DLC Activated: {info.name}");
            
            OnDLCActivated?.Invoke(package);
            return true;
        }

        /// <summary>
        /// Install and load DLC content into the game
        /// </summary>
        public bool InstallDLC(DLCPackage package, Dictionary<string, Quest> quests)
        {
            if (!IsDLCOwned(package))
            {
                Debug.LogWarning($"Cannot install DLC that is not owned: {package}");
                return false;
            }

            if (IsDLCInstalled(package))
            {
                Debug.Log($"DLC already installed: {package}");
                return true;
            }

            DLCInfo info = dlcRegistry[package];
            
            switch (package)
            {
                case DLCPackage.ACOMAF_MistAndFury:
                    Book2Quests.InitializeBook2Quests(quests);
                    Debug.Log($"Loaded {info.questCount} quests from {info.name}");
                    break;
                    
                case DLCPackage.ACOWAR_WingsAndRuin:
                    // Ensure DLC 1 is installed first
                    if (!IsDLCInstalled(DLCPackage.ACOMAF_MistAndFury))
                    {
                        InstallDLC(DLCPackage.ACOMAF_MistAndFury, quests);
                    }
                    Book3Quests.InitializeBook3Quests(quests);
                    Debug.Log($"Loaded {info.questCount} quests from {info.name}");
                    break;
            }

            info.isInstalled = true;
            OnDLCLoaded?.Invoke(package);
            
            return true;
        }

        /// <summary>
        /// Check if player can access DLC content based on story progress
        /// </summary>
        public bool CanAccessDLCContent(DLCPackage package, StoryManager storyManager)
        {
            if (!IsDLCInstalled(package))
            {
                return false;
            }

            switch (package)
            {
                case DLCPackage.ACOMAF_MistAndFury:
                    // Need to complete Book 1 to access Book 2
                    return storyManager.IsArcComplete(StoryArc.Book1_Aftermath);
                    
                case DLCPackage.ACOWAR_WingsAndRuin:
                    // Need to complete Book 2 to access Book 3
                    return storyManager.IsArcComplete(StoryArc.Book2_Hybern);
                    
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get which DLC a quest belongs to (null if base game)
        /// </summary>
        public DLCPackage? GetQuestDLCPackage(string questId)
        {
            if (questId.StartsWith("book2_") || 
                (questId.StartsWith("side_") && int.Parse(questId.Substring(5)) >= 10 && int.Parse(questId.Substring(5)) <= 15) ||
                (questId.StartsWith("companion_") && int.Parse(questId.Substring(10)) >= 3 && int.Parse(questId.Substring(10)) <= 5))
            {
                return DLCPackage.ACOMAF_MistAndFury;
            }
            
            if (questId.StartsWith("book3_") ||
                (questId.StartsWith("side_") && int.Parse(questId.Substring(5)) >= 16) ||
                (questId.StartsWith("companion_") && int.Parse(questId.Substring(10)) >= 6))
            {
                return DLCPackage.ACOWAR_WingsAndRuin;
            }

            return null; // Base game content
        }

        /// <summary>
        /// Display DLC status for debugging
        /// </summary>
        public void DisplayDLCStatus()
        {
            Debug.Log("\n=== DLC Status ===");
            Debug.Log("BASE GAME: A Court of Thorns and Roses (Book 1) - INCLUDED");
            Debug.Log("  - 20+ quests, 6,250 XP available");
            Debug.Log("  - Complete story from mortal to High Fae");
            
            foreach (var dlc in dlcRegistry.Values)
            {
                string status = dlc.isOwned ? (dlc.isInstalled ? "INSTALLED" : "OWNED") : "NOT OWNED";
                Debug.Log($"\n{dlc.name}:");
                Debug.Log($"  Status: {status}");
                Debug.Log($"  {dlc.questCount} quests, {dlc.experienceAvailable} XP");
                Debug.Log($"  New Locations: {string.Join(", ", dlc.newLocations)}");
                Debug.Log($"  New Characters: {string.Join(", ", dlc.newCharacters)}");
            }
            Debug.Log("==================\n");
        }

        /// <summary>
        /// Simulate purchasing all DLC for testing
        /// </summary>
        public void UnlockAllDLCForTesting()
        {
            foreach (DLCPackage package in Enum.GetValues(typeof(DLCPackage)))
            {
                ActivateDLC(package);
            }
            Debug.Log("All DLC unlocked for testing purposes.");
        }
    }
}
