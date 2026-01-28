using UnityEngine;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Main game manager that orchestrates the ACOTAR RPG experience
    /// Enhanced with Phase 5 advanced gameplay systems and Phase 6 story content
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Managers")]
        public LocationManager locationManager;
        public QuestManager questManager;
        public CompanionManager companionManager;
        public DialogueSystem dialogueSystem;
        public TimeSystem timeSystem;
        public StoryManager storyManager;

        [Header("Player")]
        public Character playerCharacter;

        [Header("Systems")]
        private InventorySystem inventorySystem;
        private ReputationSystem reputationSystem;
        private CraftingSystem craftingSystem;

        [Header("Game State")]
        public string currentLocation;
        public int gameTime; // In-game days passed
        public bool hasMetRhysand;
        public bool hasCompletedCurse;

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeGame();
        }

        private void InitializeGame()
        {
            Debug.Log("=== ACOTAR Fantasy RPG Initialized ===");
            Debug.Log("Welcome to Prythian!");
            
            // Initialize player with configuration
            playerCharacter = new Character(
                GameConfig.DEFAULT_PLAYER_NAME, 
                GameConfig.DEFAULT_PLAYER_CLASS, 
                GameConfig.DEFAULT_PLAYER_COURT
            );
            
            // Initialize game systems
            inventorySystem = new InventorySystem();
            reputationSystem = new ReputationSystem(playerCharacter);
            craftingSystem = new CraftingSystem(inventorySystem, playerCharacter);
            
            // Set starting location
            currentLocation = GameConfig.DEFAULT_STARTING_LOCATION;
            gameTime = 0;
            hasMetRhysand = false;
            hasCompletedCurse = false;

            Debug.Log($"Created character: {playerCharacter.name}");
            Debug.Log($"Class: {playerCharacter.characterClass}");
            Debug.Log($"Starting location: {currentLocation}");
            Debug.Log("All systems initialized successfully!");
        }

        /// <summary>
        /// Transform player from human to High Fae (lore-accurate transformation)
        /// Optimized to preserve progression and abilities
        /// </summary>
        public void TransformToHighFae()
        {
            if (playerCharacter.characterClass == CharacterClass.Human)
            {
                CharacterClass oldClass = playerCharacter.characterClass;
                
                // Store current progression
                int currentXP = playerCharacter.experience;
                int currentLevel = playerCharacter.level;
                List<MagicType> currentAbilities = new List<MagicType>(playerCharacter.abilities);
                
                // Create new High Fae character preserving identity
                playerCharacter = new Character(
                    playerCharacter.name, 
                    CharacterClass.HighFae, 
                    playerCharacter.allegiance
                );
                playerCharacter.isMadeByTheCauldron = true;
                
                // Restore progression
                playerCharacter.experience = currentXP;
                playerCharacter.level = currentLevel;
                foreach (var ability in currentAbilities)
                {
                    playerCharacter.LearnAbility(ability);
                }

                Debug.Log($"{playerCharacter.name} has been Made by the Cauldron and transformed into High Fae!");
                GameEvents.TriggerCharacterTransformed(playerCharacter, CharacterClass.HighFae);
            }
        }

        /// <summary>
        /// Grant player abilities based on ACOTAR lore
        /// </summary>
        public void GrantAbility(MagicType ability)
        {
            playerCharacter.LearnAbility(ability);
            Debug.Log($"{playerCharacter.name} has learned: {ability}");
        }

        /// <summary>
        /// Travel to a new location with event notification
        /// </summary>
        public void TravelTo(string locationName)
        {
            if (locationManager == null)
            {
                Debug.LogWarning("LocationManager not initialized");
                return;
            }
            
            Location location = locationManager.GetLocation(locationName);
            if (location != null)
            {
                string previousLocation = currentLocation;
                currentLocation = locationName;
                gameTime++;
                Debug.Log($"Traveled to: {locationName}");
                Debug.Log($"Description: {location.description}");
                GameEvents.TriggerLocationChanged(previousLocation, locationName);
            }
            else
            {
                Debug.LogWarning($"Location not found: {locationName}");
            }
        }

        /// <summary>
        /// Change player's court allegiance with event notification
        /// </summary>
        public void ChangeCourtAllegiance(Court newCourt)
        {
            playerCharacter.allegiance = newCourt;
            Debug.Log($"{playerCharacter.name} now serves the {newCourt} Court");
            GameEvents.TriggerCourtAllegianceChanged(playerCharacter, newCourt);
        }

        /// <summary>
        /// Showcase game features for demonstration
        /// </summary>
        private void Start()
        {
            Debug.Log("\n=== Starting ACOTAR RPG Demo ===\n");
            
            // Start the main quest line
            if (questManager != null)
            {
                questManager.StartQuest("main_001");
                Debug.Log("\nMain quest started!");
            }

            // List available locations
            if (locationManager != null)
            {
                Debug.Log("\n=== Available Locations in Prythian ===");
                List<string> locations = locationManager.GetAllLocationNames();
                foreach (string loc in locations)
                {
                    Debug.Log($"- {loc}");
                }
            }

            // Show initial character stats
            ShowCharacterStats();
        }

        public void ShowCharacterStats()
        {
            Debug.Log("\n=== Character Stats ===");
            Debug.Log($"Name: {playerCharacter.name}");
            Debug.Log($"Class: {playerCharacter.characterClass}");
            Debug.Log($"Court: {playerCharacter.allegiance}");
            Debug.Log($"Level: {playerCharacter.level}");
            Debug.Log($"Experience: {playerCharacter.experience}/{playerCharacter.GetXPRequiredForNextLevel()}");
            Debug.Log($"Health: {playerCharacter.health}/{playerCharacter.maxHealth}");
            Debug.Log($"Magic Power: {playerCharacter.magicPower}");
            Debug.Log($"Strength: {playerCharacter.strength}");
            Debug.Log($"Agility: {playerCharacter.agility}");
            Debug.Log($"Is Fae: {playerCharacter.isFae}");
            Debug.Log($"Made by Cauldron: {playerCharacter.isMadeByTheCauldron}");
            
            if (playerCharacter.abilities.Count > 0)
            {
                Debug.Log("Abilities:");
                foreach (var ability in playerCharacter.abilities)
                {
                    Debug.Log($"  - {ability}");
                }
            }
            Debug.Log("======================\n");
        }

        /// <summary>
        /// Demo progression through key story moments
        /// </summary>
        public void DemoStoryProgression()
        {
            Debug.Log("\n=== Demo: Story Progression ===\n");
            
            // Act 1: Human in mortal lands
            Debug.Log("Act 1: A human huntress...");
            ShowCharacterStats();

            // Travel to Spring Court
            TravelTo("Spring Court Manor");
            
            // Complete first quest
            if (questManager != null)
            {
                questManager.CompleteQuest("main_001");
                questManager.StartQuest("main_002");
            }

            // Change allegiance to Spring Court
            ChangeCourtAllegiance(Court.Spring);

            // Act 2: Under the Mountain - transformation
            Debug.Log("\nAct 2: Breaking the curse...");
            TravelTo("Under the Mountain");
            TransformToHighFae();
            ShowCharacterStats();

            // Complete the curse quest
            if (questManager != null)
            {
                questManager.CompleteQuest("main_005");
            }
            hasCompletedCurse = true;

            // Act 3: Night Court
            Debug.Log("\nAct 3: Discovering the Night Court...");
            hasMetRhysand = true;
            TravelTo("Velaris");
            ChangeCourtAllegiance(Court.Night);

            // Grant High Fae abilities
            GrantAbility(MagicType.ShieldCreation);
            GrantAbility(MagicType.DarknessManipulation);
            GrantAbility(MagicType.Daemati);

            ShowCharacterStats();

            Debug.Log("\n=== Demo Complete ===");
        }

        /// <summary>
        /// Demo Phase 5 advanced gameplay systems
        /// </summary>
        public void DemoPhase5Systems()
        {
            Debug.Log("\n\n╔══════════════════════════════════════════════════════════╗");
            Debug.Log("║    PHASE 5: Advanced Gameplay Systems Demo              ║");
            Debug.Log("╚══════════════════════════════════════════════════════════╝\n");

            // 1. Combat System Demo
            Debug.Log("=== 1. COMBAT SYSTEM DEMO ===\n");
            
            // Create enemies
            Enemy bogge = EnemyFactory.CreateBogge(EnemyDifficulty.Normal);
            Enemy naga = EnemyFactory.CreateNaga(EnemyDifficulty.Easy);
            
            List<Enemy> enemies = new List<Enemy> { bogge };
            CombatEncounter encounter = new CombatEncounter(playerCharacter, enemies);
            encounter.StartEncounter();
            
            // Simulate combat turns
            encounter.PlayerPhysicalAttack(bogge);
            if (bogge.IsAlive())
            {
                encounter.PlayerMagicAttack(bogge, MagicType.ShieldCreation);
            }
            
            Debug.Log("\n=== Combat Demo Complete ===\n");

            // 2. Companion System Demo
            Debug.Log("=== 2. COMPANION SYSTEM DEMO ===\n");
            
            if (companionManager != null)
            {
                companionManager.RecruitCompanion("Rhysand");
                companionManager.RecruitCompanion("Cassian");
                companionManager.RecruitCompanion("Azriel");
                
                companionManager.AddToParty("Rhysand");
                companionManager.AddToParty("Cassian");
                companionManager.AddToParty("Azriel");
                
                List<Companion> party = companionManager.GetActiveParty();
                Debug.Log($"\nActive Party ({party.Count}/3):");
                foreach (Companion comp in party)
                {
                    Debug.Log($"  - {comp.name} ({comp.role}) - Loyalty: {comp.loyalty}/100");
                }
            }
            
            Debug.Log("\n=== Companion Demo Complete ===\n");

            // 3. Reputation System Demo
            Debug.Log("=== 3. REPUTATION SYSTEM DEMO ===\n");
            
            reputationSystem.GainReputation(Court.Night, 40);
            reputationSystem.GainReputation(Court.Summer, 15);
            reputationSystem.LoseReputation(Court.Spring, 20);
            
            reputationSystem.DisplayReputations();
            
            Debug.Log("=== Reputation Demo Complete ===\n");

            // 4. Dialogue System Demo
            Debug.Log("=== 4. DIALOGUE SYSTEM DEMO ===\n");
            
            if (dialogueSystem != null)
            {
                dialogueSystem.StartDialogue("rhysand_greeting", playerCharacter, reputationSystem);
                Debug.Log("\n(Dialogue system initialized - player would interact via UI)");
            }
            
            Debug.Log("\n=== Dialogue Demo Complete ===\n");

            // 5. Crafting System Demo
            Debug.Log("=== 5. CRAFTING SYSTEM DEMO ===\n");
            
            // Add some materials to inventory
            inventorySystem.AddItem("crafting_ash_wood", 10);
            inventorySystem.AddItem("crafting_iron_ingot", 5);
            inventorySystem.AddItem("crafting_healing_herb", 15);
            inventorySystem.AddItem("crafting_water_vial", 10);
            
            Debug.Log("Materials added to inventory!");
            Debug.Log("\nAttempting to craft Ash Wood Dagger...");
            craftingSystem.CraftItem("craft_ash_dagger", CraftingStationType.Forge);
            
            Debug.Log("\nAttempting to craft Healing Potions...");
            craftingSystem.CraftItem("craft_healing_potion", CraftingStationType.AlchemyTable);
            
            Debug.Log("\n=== Crafting Demo Complete ===\n");

            // 6. Time System Demo
            Debug.Log("=== 6. TIME SYSTEM DEMO ===\n");
            
            if (timeSystem != null)
            {
                timeSystem.DisplayTimeInfo();
                
                Debug.Log("Advancing time by 6 hours...");
                timeSystem.AddHours(6);
                timeSystem.DisplayTimeInfo();
                
                Debug.Log("Advancing to next day...");
                timeSystem.AddDays(1);
                
                string activeEvent = timeSystem.GetActiveEvent();
                if (activeEvent != "None")
                {
                    Debug.Log($"Special Event Active: {activeEvent}");
                }
            }
            
            Debug.Log("\n=== Time Demo Complete ===\n");

            // 7. Inventory Display
            Debug.Log("=== 7. INVENTORY DISPLAY ===\n");
            
            List<InventorySlot> allItems = inventorySystem.GetAllItems();
            Debug.Log($"Inventory: {allItems.Count} unique items");
            foreach (InventorySlot slot in allItems)
            {
                Debug.Log($"  - {slot.item.name} x{slot.quantity} ({slot.item.rarity})");
            }
            
            Debug.Log("\n=== Inventory Demo Complete ===\n");

            Debug.Log("\n╔══════════════════════════════════════════════════════════╗");
            Debug.Log("║    Phase 5 Advanced Gameplay Systems Demo Complete!     ║");
            Debug.Log("║    All systems operational and ready for gameplay!       ║");
            Debug.Log("╚══════════════════════════════════════════════════════════╝\n");
        }

        /// <summary>
        /// Get inventory system (for external access)
        /// </summary>
        public InventorySystem GetInventorySystem()
        {
            return inventorySystem;
        }

        /// <summary>
        /// Get reputation system (for external access)
        /// </summary>
        public ReputationSystem GetReputationSystem()
        {
            return reputationSystem;
        }

        /// <summary>
        /// Get crafting system (for external access)
        /// </summary>
        public CraftingSystem GetCraftingSystem()
        {
            return craftingSystem;
        }
    }
}
