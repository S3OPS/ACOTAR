using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Story arc identifiers for ACOTAR books
    /// </summary>
    public enum StoryArc
    {
        // BASE GAME - Book 1: A Court of Thorns and Roses
        Book1_HumanLands,
        Book1_SpringCourt,
        Book1_UnderTheMountain,
        Book1_Aftermath,
        
        // DLC 1 - Book 2: A Court of Mist and Fury
        Book2_NightCourt,
        Book2_WarPreparations,
        Book2_Hybern,
        
        // DLC 2 - Book 3: A Court of Wings and Ruin
        Book3_Alliance,
        Book3_War,
        Book3_Resolution
    }

    /// <summary>
    /// Manages story progression through ACOTAR books
    /// Tracks which story arcs are complete and unlocks new content
    /// 
    /// BASE GAME: Book 1 story arcs (Book1_*)
    /// DLC 1: Book 2 story arcs (Book2_*) - requires DLC purchase
    /// DLC 2: Book 3 story arcs (Book3_*) - requires DLC purchase
    /// 
    /// v2.5.3: Enhanced with property accessors and defensive programming
    /// </summary>
    public class StoryManager : MonoBehaviour
    {
        private Dictionary<StoryArc, bool> completedArcs;
        private StoryArc currentArc;
        private List<string> unlockedLocations;
        private List<string> metCharacters;

        // Public property accessors for cleaner code (v2.5.3)
        /// <summary>
        /// Get the current story arc
        /// </summary>
        public StoryArc CurrentArc => currentArc;

        /// <summary>
        /// Get the number of unlocked locations
        /// </summary>
        public int UnlockedLocationCount => unlockedLocations?.Count ?? 0;

        /// <summary>
        /// Get the number of characters met
        /// </summary>
        public int MetCharacterCount => metCharacters?.Count ?? 0;

        /// <summary>
        /// Check if the story system is properly initialized
        /// </summary>
        public bool IsInitialized => completedArcs != null && unlockedLocations != null && metCharacters != null;

        void Awake()
        {
            InitializeStory();
        }

        /// <summary>
        /// Initialize story progression system
        /// </summary>
        private void InitializeStory()
        {
            completedArcs = new Dictionary<StoryArc, bool>();
            unlockedLocations = new List<string>();
            metCharacters = new List<string>();
            
            // Initialize all story arcs as incomplete
            foreach (StoryArc arc in System.Enum.GetValues(typeof(StoryArc)))
            {
                completedArcs[arc] = false;
            }

            // Set starting arc
            currentArc = StoryArc.Book1_HumanLands;

            // Starting locations (using "Mortal Lands" to match book terminology)
            unlockedLocations.Add("Mortal Lands");
            unlockedLocations.Add("The Wall");
        }

        /// <summary>
        /// Check if a story arc is part of DLC content
        /// </summary>
        public bool IsArcDLCContent(StoryArc arc)
        {
            switch (arc)
            {
                case StoryArc.Book2_NightCourt:
                case StoryArc.Book2_WarPreparations:
                case StoryArc.Book2_Hybern:
                    return true; // DLC 1
                    
                case StoryArc.Book3_Alliance:
                case StoryArc.Book3_War:
                case StoryArc.Book3_Resolution:
                    return true; // DLC 2
                    
                default:
                    return false; // Base game
            }
        }

        /// <summary>
        /// Get which DLC a story arc belongs to
        /// </summary>
        public DLCPackage? GetArcDLCPackage(StoryArc arc)
        {
            switch (arc)
            {
                case StoryArc.Book2_NightCourt:
                case StoryArc.Book2_WarPreparations:
                case StoryArc.Book2_Hybern:
                    return DLCPackage.ACOMAF_MistAndFury;
                    
                case StoryArc.Book3_Alliance:
                case StoryArc.Book3_War:
                case StoryArc.Book3_Resolution:
                    return DLCPackage.ACOWAR_WingsAndRuin;
                    
                default:
                    return null;
            }
        }

        /// <summary>
        /// Complete a story arc and unlock next content
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void CompleteArc(StoryArc arc)
        {
            // Defensive check (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("StoryManager: Cannot complete arc - system not initialized");
                return;
            }

            if (!completedArcs.ContainsKey(arc))
            {
                Debug.LogWarning($"StoryManager: Story arc '{arc}' not found in dictionary");
                return;
            }

            if (!completedArcs[arc])
            {
                completedArcs[arc] = true;
                Debug.Log($"Story Arc Completed: {arc}");
                
                // Unlock content based on arc completion
                UnlockContentForArc(arc);
                
                // Move to next arc
                AdvanceStory(arc);
            }
        }

        /// <summary>
        /// Unlock locations and characters for completed arc
        /// </summary>
        private void UnlockContentForArc(StoryArc arc)
        {
            switch (arc)
            {
                // =====================================================
                // BASE GAME - Book 1: A Court of Thorns and Roses
                // =====================================================
                case StoryArc.Book1_HumanLands:
                    UnlockLocation("Spring Court Manor");
                    UnlockCharacter("Tamlin");
                    UnlockCharacter("Lucien");
                    break;

                case StoryArc.Book1_SpringCourt:
                    UnlockLocation("Under the Mountain");
                    UnlockCharacter("Rhysand");
                    UnlockCharacter("Amarantha");
                    break;

                case StoryArc.Book1_UnderTheMountain:
                    // Preview of Night Court from Rhysand's memory (base game teaser)
                    UnlockCharacter("Alis");
                    break;

                case StoryArc.Book1_Aftermath:
                    // Base game ending - player becomes High Fae
                    Debug.Log("=== BASE GAME COMPLETE ===");
                    Debug.Log("Congratulations! You have completed A Court of Thorns and Roses!");
                    Debug.Log("To continue Feyre's journey, purchase DLC 1: A Court of Mist and Fury");
                    break;

                // =====================================================
                // DLC 1 - Book 2: A Court of Mist and Fury
                // =====================================================
                case StoryArc.Book2_NightCourt:
                    UnlockLocation("Velaris");
                    UnlockLocation("House of Wind");
                    UnlockLocation("Hewn City");
                    UnlockLocation("Illyrian Mountains");
                    UnlockCharacter("Cassian");
                    UnlockCharacter("Azriel");
                    UnlockCharacter("Mor");
                    UnlockCharacter("Amren");
                    break;

                case StoryArc.Book2_WarPreparations:
                    UnlockLocation("Summer Court");
                    UnlockLocation("Adriata");
                    UnlockCharacter("Tarquin");
                    UnlockCharacter("Nesta");
                    UnlockCharacter("Elain");
                    break;

                case StoryArc.Book2_Hybern:
                    UnlockLocation("Hybern");
                    UnlockLocation("Mortal Lands");
                    UnlockCharacter("King of Hybern");
                    UnlockCharacter("Jurian");
                    Debug.Log("=== DLC 1 COMPLETE ===");
                    Debug.Log("Congratulations! You have completed A Court of Mist and Fury!");
                    Debug.Log("To continue the story, purchase DLC 2: A Court of Wings and Ruin");
                    break;

                // =====================================================
                // DLC 2 - Book 3: A Court of Wings and Ruin
                // =====================================================
                case StoryArc.Book3_Alliance:
                    UnlockLocation("Dawn Court");
                    UnlockLocation("Day Court");
                    UnlockLocation("Autumn Court");
                    UnlockLocation("Winter Court");
                    UnlockCharacter("Thesan");
                    UnlockCharacter("Helion");
                    UnlockCharacter("Kallias");
                    UnlockCharacter("Viviane");
                    UnlockCharacter("Beron");
                    UnlockCharacter("Eris");
                    break;

                case StoryArc.Book3_War:
                    UnlockLocation("The Battlefield");
                    UnlockLocation("The Prison");
                    UnlockCharacter("Bryaxis");
                    UnlockCharacter("Bone Carver (freed)");
                    UnlockCharacter("Vassa");
                    break;

                case StoryArc.Book3_Resolution:
                    UnlockLocation("Rebuilt Velaris");
                    UnlockLocation("New Prythian");
                    Debug.Log("=== DLC 2 COMPLETE ===");
                    Debug.Log("Congratulations! You have completed A Court of Wings and Ruin!");
                    Debug.Log("You have finished the complete ACOTAR trilogy!");
                    break;
            }
        }

        /// <summary>
        /// Advance to the next story arc
        /// </summary>
        private void AdvanceStory(StoryArc completedArc)
        {
            StoryArc nextArc = GetNextArc(completedArc);
            
            // Check if next arc requires DLC
            DLCPackage? dlcPackage = GetArcDLCPackage(nextArc);
            if (dlcPackage.HasValue)
            {
                if (DLCManager.Instance == null || !DLCManager.Instance.IsDLCInstalled(dlcPackage.Value))
                {
                    Debug.Log($"Next story content requires DLC: {dlcPackage.Value}");
                    return;
                }
            }
            
            currentArc = nextArc;
            Debug.Log($"Now in Story Arc: {currentArc}");
        }

        /// <summary>
        /// Get the next story arc after completing the current one
        /// </summary>
        private StoryArc GetNextArc(StoryArc completedArc)
        {
            switch (completedArc)
            {
                // Base Game progression
                case StoryArc.Book1_HumanLands:
                    return StoryArc.Book1_SpringCourt;
                case StoryArc.Book1_SpringCourt:
                    return StoryArc.Book1_UnderTheMountain;
                case StoryArc.Book1_UnderTheMountain:
                    return StoryArc.Book1_Aftermath;
                case StoryArc.Book1_Aftermath:
                    return StoryArc.Book2_NightCourt; // Requires DLC 1
                    
                // DLC 1 progression
                case StoryArc.Book2_NightCourt:
                    return StoryArc.Book2_WarPreparations;
                case StoryArc.Book2_WarPreparations:
                    return StoryArc.Book2_Hybern;
                case StoryArc.Book2_Hybern:
                    return StoryArc.Book3_Alliance; // Requires DLC 2
                    
                // DLC 2 progression
                case StoryArc.Book3_Alliance:
                    return StoryArc.Book3_War;
                case StoryArc.Book3_War:
                    return StoryArc.Book3_Resolution;
                    
                // Final arc - no more progression
                case StoryArc.Book3_Resolution:
                    return StoryArc.Book3_Resolution; // Game complete, stay at final arc
                    
                default:
                    return completedArc;
            }
        }

        /// <summary>
        /// Unlock a location for the player
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void UnlockLocation(string locationName)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("StoryManager: Cannot unlock location - system not initialized");
                return;
            }

            if (string.IsNullOrEmpty(locationName))
            {
                Debug.LogWarning("StoryManager: Cannot unlock location with null or empty name");
                return;
            }

            if (!unlockedLocations.Contains(locationName))
            {
                unlockedLocations.Add(locationName);
                Debug.Log($"Location Unlocked: {locationName}");
            }
        }

        /// <summary>
        /// Track that player has met a character
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void UnlockCharacter(string characterName)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("StoryManager: Cannot unlock character - system not initialized");
                return;
            }

            if (string.IsNullOrEmpty(characterName))
            {
                Debug.LogWarning("StoryManager: Cannot unlock character with null or empty name");
                return;
            }

            if (!metCharacters.Contains(characterName))
            {
                metCharacters.Add(characterName);
                Debug.Log($"Character Met: {characterName}");
            }
        }

        /// <summary>
        /// Check if location is unlocked
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public bool IsLocationUnlocked(string locationName)
        {
            if (!IsInitialized || string.IsNullOrEmpty(locationName))
            {
                return false;
            }
            return unlockedLocations.Contains(locationName);
        }

        /// <summary>
        /// Check if character has been met
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public bool HasMetCharacter(string characterName)
        {
            if (!IsInitialized || string.IsNullOrEmpty(characterName))
            {
                return false;
            }
            return metCharacters.Contains(characterName);
        }

        /// <summary>
        /// Get current story arc
        /// NOTE: Prefer using the 'CurrentArc' property for cleaner code (v2.5.3)
        /// </summary>
        public StoryArc GetCurrentArc()
        {
            return currentArc;
        }

        /// <summary>
        /// Check if story arc is complete
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public bool IsArcComplete(StoryArc arc)
        {
            if (!IsInitialized)
            {
                return false;
            }
            return completedArcs.ContainsKey(arc) && completedArcs[arc];
        }

        /// <summary>
        /// Check if base game is complete
        /// </summary>
        public bool IsBaseGameComplete()
        {
            return IsArcComplete(StoryArc.Book1_Aftermath);
        }

        /// <summary>
        /// Get all unlocked locations
        /// </summary>
        public List<string> GetUnlockedLocations()
        {
            return new List<string>(unlockedLocations);
        }

        /// <summary>
        /// Get all met characters
        /// </summary>
        public List<string> GetMetCharacters()
        {
            return new List<string>(metCharacters);
        }

        /// <summary>
        /// Display story progress
        /// </summary>
        public void DisplayStoryProgress()
        {
            Debug.Log("\n=== Story Progress ===");
            Debug.Log($"Current Arc: {currentArc}");
            
            Debug.Log("\n--- BASE GAME (Book 1) ---");
            DisplayArcStatus(StoryArc.Book1_HumanLands);
            DisplayArcStatus(StoryArc.Book1_SpringCourt);
            DisplayArcStatus(StoryArc.Book1_UnderTheMountain);
            DisplayArcStatus(StoryArc.Book1_Aftermath);
            
            if (DLCManager.Instance != null && DLCManager.Instance.IsDLCInstalled(DLCPackage.ACOMAF_MistAndFury))
            {
                Debug.Log("\n--- DLC 1 (Book 2) ---");
                DisplayArcStatus(StoryArc.Book2_NightCourt);
                DisplayArcStatus(StoryArc.Book2_WarPreparations);
                DisplayArcStatus(StoryArc.Book2_Hybern);
            }
            else
            {
                Debug.Log("\n--- DLC 1 (Book 2) --- NOT INSTALLED");
            }
            
            if (DLCManager.Instance != null && DLCManager.Instance.IsDLCInstalled(DLCPackage.ACOWAR_WingsAndRuin))
            {
                Debug.Log("\n--- DLC 2 (Book 3) ---");
                DisplayArcStatus(StoryArc.Book3_Alliance);
                DisplayArcStatus(StoryArc.Book3_War);
                DisplayArcStatus(StoryArc.Book3_Resolution);
            }
            else
            {
                Debug.Log("\n--- DLC 2 (Book 3) --- NOT INSTALLED");
            }
            
            Debug.Log($"\nUnlocked Locations ({unlockedLocations.Count}):");
            foreach (string location in unlockedLocations)
            {
                Debug.Log($"  - {location}");
            }
            Debug.Log($"\nMet Characters ({metCharacters.Count}):");
            foreach (string character in metCharacters)
            {
                Debug.Log($"  - {character}");
            }
            Debug.Log("======================\n");
        }

        private void DisplayArcStatus(StoryArc arc)
        {
            string status = completedArcs[arc] ? "✓" : " ";
            string current = (arc == currentArc) ? " <-- CURRENT" : "";
            Debug.Log($"  [{status}] {arc}{current}");
        }

        /// <summary>
        /// Get all unlocked location names
        /// v2.5.3: New helper method for safe access
        /// </summary>
        /// <returns>Read-only list of unlocked location names</returns>
        public List<string> GetUnlockedLocations()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("StoryManager: Cannot get unlocked locations - system not initialized");
                return new List<string>();
            }
            return new List<string>(unlockedLocations);
        }

        /// <summary>
        /// Get all met character names
        /// v2.5.3: New helper method for safe access
        /// </summary>
        /// <returns>Read-only list of met character names</returns>
        public List<string> GetMetCharacters()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("StoryManager: Cannot get met characters - system not initialized");
                return new List<string>();
            }
            return new List<string>(metCharacters);
        }

        /// <summary>
        /// Get story progression percentage (0-100)
        /// v2.5.3: New helper method for progress tracking
        /// </summary>
        /// <returns>Percentage of story arcs completed</returns>
        public float GetProgressPercentage()
        {
            if (!IsInitialized)
            {
                return 0f;
            }

            int totalArcs = completedArcs.Count;
            if (totalArcs == 0)
            {
                return 0f;
            }

            int completedCount = 0;
            foreach (bool completed in completedArcs.Values)
            {
                if (completed)
                {
                    completedCount++;
                }
            }

            return (float)completedCount / totalArcs * 100f;
        }
    }
}
