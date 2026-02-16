using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Time of day periods for NPC schedules
    /// </summary>
    public enum TimeOfDay
    {
        Morning,    // 6:00 - 12:00
        Afternoon,  // 12:00 - 18:00
        Evening,    // 18:00 - 22:00
        Night       // 22:00 - 6:00
    }

    /// <summary>
    /// NPC activity types
    /// </summary>
    public enum NPCActivity
    {
        Sleeping,
        Working,
        Shopping,
        Training,
        Socializing,
        Eating,
        Patrolling,
        Studying,
        Crafting,
        Performing,
        Wandering,
        SpecialEvent
    }

    /// <summary>
    /// NPC relationship status with player
    /// </summary>
    public enum RelationshipStatus
    {
        Hostile,      // Will attack on sight
        Unfriendly,   // Won't help, limited dialogue
        Neutral,      // Standard interactions
        Friendly,     // More dialogue options, small discounts
        Trusted,      // Quest givers, better prices
        Ally,         // Will assist in combat
        Romantic      // Special romantic interactions
    }

    /// <summary>
    /// Schedule entry for NPC
    /// </summary>
    [System.Serializable]
    public class ScheduleEntry
    {
        public TimeOfDay timeOfDay;
        public string locationName;
        public NPCActivity activity;
        public string activityDescription;
        
        public ScheduleEntry(TimeOfDay time, string location, NPCActivity act, string desc)
        {
            timeOfDay = time;
            locationName = location;
            activity = act;
            activityDescription = desc;
        }
    }

    /// <summary>
    /// NPC character with daily routine
    /// </summary>
    [System.Serializable]
    public class ScheduledNPC
    {
        public string name;
        public CharacterClass npcClass;
        public Court courtAffiliation;
        public List<ScheduleEntry> dailySchedule;
        public RelationshipStatus relationshipWithPlayer;
        public int relationshipPoints;  // -100 to +100
        public bool isRomanceable;
        public bool isQuestGiver;
        public List<string> availableDialogue;
        public Dictionary<RelationshipStatus, List<string>> statusDialogue;
        
        public ScheduledNPC(string npcName, CharacterClass charClass, Court court)
        {
            name = npcName;
            npcClass = charClass;
            courtAffiliation = court;
            dailySchedule = new List<ScheduleEntry>();
            relationshipWithPlayer = RelationshipStatus.Neutral;
            relationshipPoints = 0;
            isRomanceable = false;
            isQuestGiver = false;
            availableDialogue = new List<string>();
            statusDialogue = new Dictionary<RelationshipStatus, List<string>>();
        }

        /// <summary>
        /// Get current location based on time of day
        /// </summary>
        public string GetCurrentLocation(TimeOfDay currentTime)
        {
            var entry = dailySchedule.Find(s => s.timeOfDay == currentTime);
            return entry?.locationName ?? "Unknown";
        }

        /// <summary>
        /// Get current activity based on time of day
        /// </summary>
        public ScheduleEntry GetCurrentActivity(TimeOfDay currentTime)
        {
            return dailySchedule.Find(s => s.timeOfDay == currentTime);
        }

        /// <summary>
        /// Modify relationship points
        /// </summary>
        public void ModifyRelationship(int points)
        {
            relationshipPoints += points;
            relationshipPoints = Mathf.Clamp(relationshipPoints, -100, 100);
            
            // Update relationship status based on points
            RelationshipStatus oldStatus = relationshipWithPlayer;
            
            if (relationshipPoints >= 80)
                relationshipWithPlayer = isRomanceable ? RelationshipStatus.Romantic : RelationshipStatus.Ally;
            else if (relationshipPoints >= 60)
                relationshipWithPlayer = RelationshipStatus.Ally;
            else if (relationshipPoints >= 40)
                relationshipWithPlayer = RelationshipStatus.Trusted;
            else if (relationshipPoints >= 20)
                relationshipWithPlayer = RelationshipStatus.Friendly;
            else if (relationshipPoints >= -20)
                relationshipWithPlayer = RelationshipStatus.Neutral;
            else if (relationshipPoints >= -50)
                relationshipWithPlayer = RelationshipStatus.Unfriendly;
            else
                relationshipWithPlayer = RelationshipStatus.Hostile;
            
            if (oldStatus != relationshipWithPlayer)
            {
                Debug.Log($"💝 Relationship with {name} changed: {oldStatus} → {relationshipWithPlayer}");
            }
        }

        /// <summary>
        /// Get dialogue based on current relationship
        /// </summary>
        public List<string> GetDialogueForRelationship()
        {
            if (statusDialogue.ContainsKey(relationshipWithPlayer))
            {
                return statusDialogue[relationshipWithPlayer];
            }
            return availableDialogue;
        }
    }

    /// <summary>
    /// Random encounter definition
    /// </summary>
    [System.Serializable]
    public class RandomEncounter
    {
        public string npcName;
        public string locationName;
        public float encounterChance;  // 0.0 to 1.0
        public string encounterDialogue;
        public int relationshipChange;
        
        public RandomEncounter(string npc, string location, float chance, string dialogue, int relChange = 0)
        {
            npcName = npc;
            locationName = location;
            encounterChance = chance;
            encounterDialogue = dialogue;
            relationshipChange = relChange;
        }
    }

    /// <summary>
    /// NPC Schedule System - Living World with Daily Routines
    /// Version 2.6.0 - New Feature
    /// 
    /// Creates a dynamic living world where NPCs:
    /// - Follow daily schedules and move between locations
    /// - Have relationship progression with the player
    /// - Provide different dialogue based on relationship status
    /// - Can be encountered randomly at various locations
    /// - React to player choices and reputation
    /// </summary>
    public class NPCScheduleSystem
    {
        private Dictionary<string, ScheduledNPC> allNPCs;
        private List<RandomEncounter> randomEncounters;
        private TimeSystem timeSystem;
        
        // Property accessors (v2.6.0: Following v2.5.x patterns)
        public int NPCCount => allNPCs?.Count ?? 0;
        public bool IsInitialized => allNPCs != null && randomEncounters != null;
        
        public NPCScheduleSystem(TimeSystem timeRef)
        {
            allNPCs = new Dictionary<string, ScheduledNPC>();
            randomEncounters = new List<RandomEncounter>();
            timeSystem = timeRef;
            InitializeNPCs();
            InitializeRandomEncounters();
            Debug.Log($"NPCScheduleSystem initialized with {NPCCount} NPCs and dynamic schedules");
        }

        /// <summary>
        /// Initialize major NPCs with schedules
        /// </summary>
        private void InitializeNPCs()
        {
            // Alis - Spring Court Servant
            var alis = new ScheduledNPC("Alis", CharacterClass.LesserFae, Court.Spring);
            alis.isQuestGiver = true;
            alis.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Morning, "Spring Court Manor - Kitchen", NPCActivity.Working, "Preparing breakfast"));
            alis.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Afternoon, "Spring Court Manor - Garden", NPCActivity.Socializing, "Taking a break in the garden"));
            alis.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Evening, "Spring Court Manor - Kitchen", NPCActivity.Working, "Preparing dinner"));
            alis.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Night, "Spring Court Manor - Servants Quarters", NPCActivity.Sleeping, "Resting"));
            alis.availableDialogue.Add("The Spring Court was beautiful... once.");
            alis.availableDialogue.Add("Be careful around the High Lord.");
            allNPCs["Alis"] = alis;

            // Merchant at Velaris
            var velarisTrader = new ScheduledNPC("Aranea", CharacterClass.LesserFae, Court.Night);
            velarisTrader.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Morning, "Velaris - Market District", NPCActivity.Working, "Opening shop"));
            velarisTrader.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Afternoon, "Velaris - Market District", NPCActivity.Working, "Busy trading hours"));
            velarisTrader.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Evening, "Velaris - Rainbow", NPCActivity.Socializing, "Dinner at local tavern"));
            velarisTrader.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Night, "Velaris - Residential", NPCActivity.Sleeping, "At home"));
            velarisTrader.availableDialogue.Add("Welcome to the City of Starlight!");
            velarisTrader.availableDialogue.Add("The finest goods in all Prythian!");
            allNPCs["Aranea"] = velarisTrader;

            // Training Master at House of Wind
            var trainingMaster = new ScheduledNPC("Devlon", CharacterClass.Illyrian, Court.Night);
            trainingMaster.isQuestGiver = true;
            trainingMaster.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Morning, "Illyrian Mountains - Training Camp", NPCActivity.Training, "Morning drills"));
            trainingMaster.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Afternoon, "Illyrian Mountains - Training Camp", NPCActivity.Training, "Combat instruction"));
            trainingMaster.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Evening, "Illyrian Mountains - Barracks", NPCActivity.Eating, "Evening meal"));
            trainingMaster.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Night, "Illyrian Mountains - Barracks", NPCActivity.Sleeping, "Resting"));
            trainingMaster.availableDialogue.Add("Think you can keep up with Illyrian training?");
            trainingMaster.availableDialogue.Add("Strength without discipline is worthless.");
            allNPCs["Devlon"] = trainingMaster;

            // Clotho - Library of Velaris
            var clotho = new ScheduledNPC("Clotho", CharacterClass.LesserFae, Court.Night);
            clotho.isQuestGiver = true;
            clotho.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Morning, "House of Wind - Library", NPCActivity.Studying, "Organizing books"));
            clotho.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Afternoon, "House of Wind - Library", NPCActivity.Studying, "Assisting researchers"));
            clotho.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Evening, "House of Wind - Library", NPCActivity.Studying, "Personal research"));
            clotho.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Night, "House of Wind - Library", NPCActivity.Sleeping, "In private quarters"));
            clotho.availableDialogue.Add("*writes on paper* Welcome to the library.");
            clotho.availableDialogue.Add("*gestures to the vast collection of books*");
            allNPCs["Clotho"] = clotho;

            // Blacksmith at Dawn Court
            var dawnSmith = new ScheduledNPC("Thesan's Smith", CharacterClass.HighFae, Court.Dawn);
            dawnSmith.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Morning, "Dawn Court - Forge", NPCActivity.Crafting, "Forging weapons"));
            dawnSmith.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Afternoon, "Dawn Court - Forge", NPCActivity.Crafting, "Armor repairs"));
            dawnSmith.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Evening, "Dawn Court - Market", NPCActivity.Shopping, "Buying materials"));
            dawnSmith.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Night, "Dawn Court - Home", NPCActivity.Sleeping, "Resting"));
            dawnSmith.availableDialogue.Add("The finest blades in Prythian, forged at dawn's first light.");
            dawnSmith.availableDialogue.Add("Each strike of the hammer is timed with the sunrise.");
            allNPCs["Thesan's Smith"] = dawnSmith;

            // Traveling Bard
            var bard = new ScheduledNPC("Seraphina", CharacterClass.LesserFae, Court.None);
            bard.isRomanceable = true;
            bard.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Morning, "Various Courts", NPCActivity.Wandering, "Traveling between courts"));
            bard.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Afternoon, "Taverns", NPCActivity.Performing, "Afternoon performance"));
            bard.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Evening, "Taverns", NPCActivity.Performing, "Evening show"));
            bard.dailySchedule.Add(new ScheduleEntry(TimeOfDay.Night, "Local Inn", NPCActivity.Sleeping, "Resting at inn"));
            bard.availableDialogue.Add("Have you heard the tale of the Weaver?");
            bard.availableDialogue.Add("*strums lute* What song would you like to hear?");
            allNPCs["Seraphina"] = bard;

            // Add relationship-specific dialogue
            InitializeRelationshipDialogue();
        }

        /// <summary>
        /// Initialize relationship-specific dialogue for NPCs
        /// </summary>
        private void InitializeRelationshipDialogue()
        {
            // Alis relationship dialogue
            if (allNPCs.ContainsKey("Alis"))
            {
                var alis = allNPCs["Alis"];
                alis.statusDialogue[RelationshipStatus.Friendly] = new List<string>
                {
                    "I'm glad you came to the Spring Court, despite everything.",
                    "You remind me of someone... someone kind."
                };
                alis.statusDialogue[RelationshipStatus.Trusted] = new List<string>
                {
                    "You can trust me. I'll help however I can.",
                    "The others speak highly of you. I understand why."
                };
            }

            // Seraphina (bard) relationship dialogue
            if (allNPCs.ContainsKey("Seraphina"))
            {
                var bard = allNPCs["Seraphina"];
                bard.statusDialogue[RelationshipStatus.Friendly] = new List<string>
                {
                    "Your adventures would make wonderful songs.",
                    "I find myself thinking of you between performances."
                };
                bard.statusDialogue[RelationshipStatus.Romantic] = new List<string>
                {
                    "When I sing of love, I think only of you.",
                    "*smiles warmly* I've been waiting for you."
                };
            }
        }

        /// <summary>
        /// Initialize random encounter events
        /// </summary>
        private void InitializeRandomEncounters()
        {
            // Encounter Alis in the garden
            randomEncounters.Add(new RandomEncounter(
                "Alis",
                "Spring Court Manor - Garden",
                0.3f,
                "Alis is tending to the roses. She seems deep in thought.",
                5
            ));

            // Encounter merchant at night market
            randomEncounters.Add(new RandomEncounter(
                "Aranea",
                "Velaris - Rainbow",
                0.25f,
                "Aranea waves from across the street. 'Come by my shop tomorrow!'",
                3
            ));

            // Encounter bard performing
            randomEncounters.Add(new RandomEncounter(
                "Seraphina",
                "Various Courts",
                0.4f,
                "Beautiful music fills the air. Seraphina is performing nearby.",
                5
            ));

            // Training encounter
            randomEncounters.Add(new RandomEncounter(
                "Devlon",
                "Illyrian Mountains - Training Camp",
                0.2f,
                "Devlon nods approvingly as you approach. 'Ready for more training?'",
                8
            ));
        }

        /// <summary>
        /// Get NPC's current location based on time
        /// </summary>
        public string GetNPCLocation(string npcName, TimeOfDay currentTime)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(npcName))
            {
                Debug.LogWarning("NPCScheduleSystem: Cannot get location for null or empty NPC name");
                return null;
            }

            if (!allNPCs.ContainsKey(npcName))
            {
                return null;
            }

            return allNPCs[npcName].GetCurrentLocation(currentTime);
        }

        /// <summary>
        /// Get all NPCs at a specific location and time
        /// </summary>
        public List<ScheduledNPC> GetNPCsAtLocation(string locationName, TimeOfDay currentTime)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(locationName))
            {
                Debug.LogWarning("NPCScheduleSystem: Cannot get NPCs for null or empty location");
                return new List<ScheduledNPC>();
            }

            var npcsHere = new List<ScheduledNPC>();
            
            foreach (var npc in allNPCs.Values)
            {
                string npcLocation = npc.GetCurrentLocation(currentTime);
                if (npcLocation == locationName)
                {
                    npcsHere.Add(npc);
                }
            }
            
            return npcsHere;
        }

        /// <summary>
        /// Check for random encounters at location
        /// </summary>
        public RandomEncounter CheckForRandomEncounter(string locationName)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(locationName))
            {
                return null;
            }

            foreach (var encounter in randomEncounters)
            {
                if (encounter.locationName == locationName || encounter.locationName == "Various Courts")
                {
                    if (Random.value < encounter.encounterChance)
                    {
                        Debug.Log($"✨ Random Encounter: {encounter.npcName}");
                        return encounter;
                    }
                }
            }
            
            return null;
        }

        /// <summary>
        /// Modify NPC relationship
        /// </summary>
        public void ModifyNPCRelationship(string npcName, int points)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(npcName))
            {
                Debug.LogWarning("NPCScheduleSystem: Cannot modify relationship for null or empty NPC name");
                return;
            }

            if (allNPCs.ContainsKey(npcName))
            {
                allNPCs[npcName].ModifyRelationship(points);
            }
        }

        /// <summary>
        /// Get NPC by name
        /// </summary>
        public ScheduledNPC GetNPC(string npcName)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(npcName))
            {
                return null;
            }

            return allNPCs.ContainsKey(npcName) ? allNPCs[npcName] : null;
        }

        /// <summary>
        /// Get all NPCs with specific relationship status
        /// </summary>
        public List<ScheduledNPC> GetNPCsByRelationship(RelationshipStatus status)
        {
            var matchingNPCs = new List<ScheduledNPC>();
            
            foreach (var npc in allNPCs.Values)
            {
                if (npc.relationshipWithPlayer == status)
                {
                    matchingNPCs.Add(npc);
                }
            }
            
            return matchingNPCs;
        }

        /// <summary>
        /// Get all quest-giver NPCs
        /// </summary>
        public List<ScheduledNPC> GetQuestGivers()
        {
            var questGivers = new List<ScheduledNPC>();
            
            foreach (var npc in allNPCs.Values)
            {
                if (npc.isQuestGiver)
                {
                    questGivers.Add(npc);
                }
            }
            
            return questGivers;
        }

        /// <summary>
        /// Get current time of day from time system
        /// </summary>
        private TimeOfDay GetCurrentTimeOfDay()
        {
            if (timeSystem == null)
            {
                return TimeOfDay.Morning;
            }

            int hour = timeSystem.CurrentHour;
            
            if (hour >= 6 && hour < 12)
                return TimeOfDay.Morning;
            else if (hour >= 12 && hour < 18)
                return TimeOfDay.Afternoon;
            else if (hour >= 18 && hour < 22)
                return TimeOfDay.Evening;
            else
                return TimeOfDay.Night;
        }

        /// <summary>
        /// Get description of location at current time
        /// </summary>
        public string GetLocationDescription(string locationName)
        {
            // Defensive check (v2.6.0)
            if (!IsInitialized)
            {
                Debug.LogWarning("NPCScheduleSystem: Cannot get location description - system not initialized");
                return "Location unavailable";
            }

            TimeOfDay currentTime = GetCurrentTimeOfDay();
            var npcsHere = GetNPCsAtLocation(locationName, currentTime);
            
            string description = $"📍 {locationName}\n";
            description += $"🕐 {currentTime}\n\n";
            
            if (npcsHere.Count > 0)
            {
                description += "NPCs present:\n";
                foreach (var npc in npcsHere)
                {
                    var activity = npc.GetCurrentActivity(currentTime);
                    description += $"• {npc.name} - {activity.activityDescription}\n";
                }
            }
            else
            {
                description += "No NPCs currently at this location.\n";
            }
            
            return description;
        }
    }
}
