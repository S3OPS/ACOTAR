using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Story arc identifiers for ACOTAR books
    /// </summary>
    public enum StoryArc
    {
        Book1_HumanLands,
        Book1_SpringCourt,
        Book1_UnderTheMountain,
        Book1_Aftermath,
        Book2_NightCourt,
        Book2_WarPreparations,
        Book2_Hybern,
        Book3_Alliance,
        Book3_War,
        Book3_Resolution
    }

    /// <summary>
    /// Manages story progression through ACOTAR books
    /// Tracks which story arcs are complete and unlocks new content
    /// </summary>
    public class StoryManager : MonoBehaviour
    {
        private Dictionary<StoryArc, bool> completedArcs;
        private StoryArc currentArc;
        private List<string> unlockedLocations;
        private List<string> metCharacters;

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

            // Starting locations
            unlockedLocations.Add("Human Lands");
            unlockedLocations.Add("The Wall");
        }

        /// <summary>
        /// Complete a story arc and unlock next content
        /// </summary>
        public void CompleteArc(StoryArc arc)
        {
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
                    UnlockLocation("Velaris");
                    UnlockLocation("House of Wind");
                    UnlockCharacter("Cassian");
                    UnlockCharacter("Azriel");
                    UnlockCharacter("Mor");
                    break;

                case StoryArc.Book1_Aftermath:
                    UnlockLocation("Hewn City");
                    UnlockLocation("Illyrian Mountains");
                    UnlockCharacter("Amren");
                    break;

                case StoryArc.Book2_NightCourt:
                    UnlockLocation("Summer Court");
                    UnlockLocation("Adriata");
                    UnlockCharacter("Tarquin");
                    break;

                case StoryArc.Book2_WarPreparations:
                    UnlockLocation("Autumn Court");
                    UnlockLocation("Winter Court");
                    UnlockCharacter("Nesta");
                    UnlockCharacter("Elain");
                    break;
            }
        }

        /// <summary>
        /// Advance to the next story arc
        /// </summary>
        private void AdvanceStory(StoryArc completedArc)
        {
            switch (completedArc)
            {
                case StoryArc.Book1_HumanLands:
                    currentArc = StoryArc.Book1_SpringCourt;
                    break;
                case StoryArc.Book1_SpringCourt:
                    currentArc = StoryArc.Book1_UnderTheMountain;
                    break;
                case StoryArc.Book1_UnderTheMountain:
                    currentArc = StoryArc.Book1_Aftermath;
                    break;
                case StoryArc.Book1_Aftermath:
                    currentArc = StoryArc.Book2_NightCourt;
                    break;
                case StoryArc.Book2_NightCourt:
                    currentArc = StoryArc.Book2_WarPreparations;
                    break;
                case StoryArc.Book2_WarPreparations:
                    currentArc = StoryArc.Book2_Hybern;
                    break;
                case StoryArc.Book2_Hybern:
                    currentArc = StoryArc.Book3_Alliance;
                    break;
                case StoryArc.Book3_Alliance:
                    currentArc = StoryArc.Book3_War;
                    break;
                case StoryArc.Book3_War:
                    currentArc = StoryArc.Book3_Resolution;
                    break;
            }

            Debug.Log($"Now in Story Arc: {currentArc}");
        }

        /// <summary>
        /// Unlock a location for travel
        /// </summary>
        public void UnlockLocation(string locationName)
        {
            if (!unlockedLocations.Contains(locationName))
            {
                unlockedLocations.Add(locationName);
                Debug.Log($"Location Unlocked: {locationName}");
            }
        }

        /// <summary>
        /// Track that player has met a character
        /// </summary>
        public void UnlockCharacter(string characterName)
        {
            if (!metCharacters.Contains(characterName))
            {
                metCharacters.Add(characterName);
                Debug.Log($"Character Met: {characterName}");
            }
        }

        /// <summary>
        /// Check if location is unlocked
        /// </summary>
        public bool IsLocationUnlocked(string locationName)
        {
            return unlockedLocations.Contains(locationName);
        }

        /// <summary>
        /// Check if character has been met
        /// </summary>
        public bool HasMetCharacter(string characterName)
        {
            return metCharacters.Contains(characterName);
        }

        /// <summary>
        /// Get current story arc
        /// </summary>
        public StoryArc GetCurrentArc()
        {
            return currentArc;
        }

        /// <summary>
        /// Check if story arc is complete
        /// </summary>
        public bool IsArcComplete(StoryArc arc)
        {
            return completedArcs.ContainsKey(arc) && completedArcs[arc];
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
            Debug.Log($"\nCompleted Arcs:");
            foreach (var arc in completedArcs)
            {
                if (arc.Value)
                {
                    Debug.Log($"  ✓ {arc.Key}");
                }
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
    }
}
