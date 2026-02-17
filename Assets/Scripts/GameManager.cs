using UnityEngine;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Main game manager that orchestrates the ACOTAR RPG experience
    /// Enhanced with Phase 5 advanced gameplay systems, Phase 6 story content,
    /// and base game improvements (status effects, difficulty, currency, elements)
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
        private CurrencySystem currencySystem;
        private StatusEffectManager statusEffectManager;
        
        // Public property accessors for systems (v2.5.2: Added for cleaner access patterns)
        public InventorySystem inventory => inventorySystem;
        public ReputationSystem reputation => reputationSystem;
        public CraftingSystem crafting => craftingSystem;
        public CurrencySystem currency => currencySystem;
        public StatusEffectManager statusEffects => statusEffectManager;

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

        /// <summary>
        /// Initialize all game systems and create initial player character
        /// Enhanced in v2.6.5: Added comprehensive error handling and logging
        /// </summary>
        /// <remarks>
        /// This method is critical for game startup. Any failure here would prevent the game from functioning.
        /// All system initializations are protected to allow partial initialization if needed.
        /// The save system is initialized first as other systems may depend on it.
        /// Event notifications are sent to inform other systems of game readiness.
        /// </remarks>
        private void InitializeGame()
        {
            try
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                    "GameManager", "=== ACOTAR Fantasy RPG Initialized ===\nWelcome to Prythian!");
                
                // Initialize save system
                try
                {
                    SaveSystem.Initialize();
                }
                catch (System.Exception saveEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception initializing SaveSystem: {saveEx.Message}");
                    // Continue initialization - game can function without save system
                }
                
                // Initialize player with configuration
                try
                {
                    if (string.IsNullOrEmpty(GameConfig.DEFAULT_PLAYER_NAME))
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                            "GameManager", "Default player name is null or empty, using fallback");
                        playerCharacter = new Character("Feyre", GameConfig.DEFAULT_PLAYER_CLASS, GameConfig.DEFAULT_PLAYER_COURT);
                    }
                    else
                    {
                        playerCharacter = new Character(
                            GameConfig.DEFAULT_PLAYER_NAME, 
                            GameConfig.DEFAULT_PLAYER_CLASS, 
                            GameConfig.DEFAULT_PLAYER_COURT
                        );
                    }
                }
                catch (System.Exception charEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception creating player character: {charEx.Message}");
                    // Create minimal fallback character
                    playerCharacter = new Character("Feyre", CharacterClass.Human, Court.None);
                }
                
                // Initialize game systems with individual error handling
                try
                {
                    inventorySystem = new InventorySystem();
                }
                catch (System.Exception invEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception initializing InventorySystem: {invEx.Message}");
                }
                
                try
                {
                    if (playerCharacter != null)
                    {
                        reputationSystem = new ReputationSystem(playerCharacter);
                    }
                    else
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                            "GameManager", "Cannot initialize ReputationSystem: player character is null");
                    }
                }
                catch (System.Exception repEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception initializing ReputationSystem: {repEx.Message}");
                }
                
                try
                {
                    if (inventorySystem != null && playerCharacter != null)
                    {
                        craftingSystem = new CraftingSystem(inventorySystem, playerCharacter);
                    }
                    else
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                            "GameManager", "Cannot initialize CraftingSystem: dependencies are null");
                    }
                }
                catch (System.Exception craftEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception initializing CraftingSystem: {craftEx.Message}");
                }
                
                try
                {
                    currencySystem = new CurrencySystem();
                }
                catch (System.Exception currEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception initializing CurrencySystem: {currEx.Message}");
                }
                
                try
                {
                    statusEffectManager = new StatusEffectManager();
                }
                catch (System.Exception statusEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception initializing StatusEffectManager: {statusEx.Message}");
                }
                
                // Set starting location
                if (string.IsNullOrEmpty(GameConfig.DEFAULT_STARTING_LOCATION))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "GameManager", "Default starting location is null or empty, using fallback");
                    currentLocation = "Human Lands";
                }
                else
                {
                    currentLocation = GameConfig.DEFAULT_STARTING_LOCATION;
                }
                
                gameTime = 0;
                hasMetRhysand = false;
                hasCompletedCurse = false;

                // Log initialization results
                if (playerCharacter != null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "GameManager", $"Created character: {playerCharacter.name}\nClass: {playerCharacter.characterClass}\nStarting location: {currentLocation}\nDifficulty: {DifficultySettings.CurrentDifficulty}");
                }
                
                if (currencySystem != null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                        "GameManager", $"Starting Gold: {currencySystem.Gold}");
                }
                
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                    "GameManager", "All systems initialized successfully!");
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "GameManager", $"Critical exception in InitializeGame: {ex.Message}\nStack: {ex.StackTrace}");
                // Attempt to create minimal viable game state
                if (playerCharacter == null)
                {
                    playerCharacter = new Character("Feyre", CharacterClass.Human, Court.None);
                }
                if (string.IsNullOrEmpty(currentLocation))
                {
                    currentLocation = "Human Lands";
                }
            }
        }

        private void Update()
        {
            // Try auto-save periodically
            SaveSystem.TryAutoSave();
        }

        /// <summary>
        /// Transform player from human to High Fae (lore-accurate transformation)
        /// Optimized to preserve progression and abilities
        /// </summary>
        /// <summary>
        /// Transform player character from Human to High Fae (story event)
        /// Enhanced in v2.6.5: Added comprehensive error handling and logging
        /// </summary>
        /// <remarks>
        /// This is a critical story moment that must preserve all character progression.
        /// The transformation stores and restores XP, level, and all learned abilities.
        /// Event handlers are protected to prevent transformation failures from breaking the game.
        /// If the character is already High Fae, the method safely returns without changes.
        /// </remarks>
        public void TransformToHighFae()
        {
            try
            {
                // Validation
                if (playerCharacter == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", "Cannot transform to High Fae: player character is null");
                    return;
                }
                
                if (playerCharacter.characterClass != CharacterClass.Human)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "GameManager", $"Cannot transform to High Fae: character is already {playerCharacter.characterClass}");
                    return;
                }
                
                CharacterClass oldClass = playerCharacter.characterClass;
                
                // Store current progression
                int currentXP = playerCharacter.experience;
                int currentLevel = playerCharacter.level;
                List<MagicType> currentAbilities = new List<MagicType>();
                
                if (playerCharacter.abilities != null)
                {
                    currentAbilities = new List<MagicType>(playerCharacter.abilities);
                }
                
                string characterName = playerCharacter.name;
                Court characterCourt = playerCharacter.allegiance;
                
                // Create new High Fae character preserving identity
                try
                {
                    playerCharacter = new Character(
                        characterName, 
                        CharacterClass.HighFae, 
                        characterCourt
                    );
                    playerCharacter.isMadeByTheCauldron = true;
                    
                    // Restore progression
                    playerCharacter.experience = currentXP;
                    playerCharacter.level = currentLevel;
                    
                    foreach (var ability in currentAbilities)
                    {
                        try
                        {
                            playerCharacter.LearnAbility(ability);
                        }
                        catch (System.Exception abilityEx)
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                                "GameManager", $"Exception restoring ability {ability}: {abilityEx.Message}");
                        }
                    }

                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "GameManager", $"{characterName} has been Made by the Cauldron and transformed into High Fae!");
                    
                    // Trigger event with protection
                    try
                    {
                        GameEvents.TriggerCharacterTransformed(playerCharacter, CharacterClass.HighFae);
                    }
                    catch (System.Exception eventEx)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "GameManager", $"Exception in CharacterTransformed event handler: {eventEx.Message}");
                    }
                }
                catch (System.Exception charEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception creating new High Fae character: {charEx.Message}");
                    // Character creation failed - try to preserve old character
                    if (playerCharacter == null)
                    {
                        // Create minimal High Fae character as fallback
                        playerCharacter = new Character(characterName, CharacterClass.HighFae, characterCourt);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "GameManager", $"Exception in TransformToHighFae: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Grant player abilities based on ACOTAR lore
        /// Enhanced in v2.5.2: Added null checks
        /// </summary>
        public void GrantAbility(MagicType ability)
        {
            if (playerCharacter == null)
            {
                Debug.LogWarning("Cannot grant ability: player character not initialized");
                return;
            }

            playerCharacter.LearnAbility(ability);
            Debug.Log($"{playerCharacter.name} has learned: {ability}");
        }

        /// <summary>
        /// Safely get player stats with null checking (v2.5.2: NEW)
        /// Useful for UI and other systems that need safe stat access
        /// </summary>
        public CharacterStats GetPlayerStats()
        {
            if (playerCharacter == null)
            {
                Debug.LogWarning("Cannot get player stats: player character not initialized");
                return null;
            }
            return playerCharacter.stats;
        }

        /// <summary>
        /// Travel to a new location with event notification
        /// Enhanced in v2.6.5: Added comprehensive error handling and logging
        /// </summary>
        /// <param name="locationName">Name of the destination location</param>
        /// <remarks>
        /// This method handles player movement between locations in Prythian.
        /// Location validation is performed before travel to prevent invalid moves.
        /// Game time advances by one day when traveling.
        /// Event handlers are protected to prevent travel failures from breaking the game.
        /// If location is invalid, the player remains at their current location.
        /// </remarks>
        public void TravelTo(string locationName)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(locationName))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "GameManager", "Cannot travel: destination location name is null or empty");
                    return;
                }
                
                if (locationManager == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", "Cannot travel: LocationManager not initialized");
                    return;
                }
                
                Location location = null;
                try
                {
                    location = locationManager.GetLocation(locationName);
                }
                catch (System.Exception locEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception getting location {locationName}: {locEx.Message}");
                    return;
                }
                
                if (location != null)
                {
                    string previousLocation = currentLocation;
                    currentLocation = locationName;
                    gameTime++;
                    
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "GameManager", $"Traveled to: {locationName}\nDescription: {location.description}");
                    
                    // Trigger event with protection
                    try
                    {
                        GameEvents.TriggerLocationChanged(previousLocation, locationName);
                    }
                    catch (System.Exception eventEx)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "GameManager", $"Exception in LocationChanged event handler: {eventEx.Message}");
                    }
                }
                else
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "GameManager", $"Location not found: {locationName}");
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "GameManager", $"Exception in TravelTo: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Change player's court allegiance with event notification
        /// Enhanced in v2.6.5: Added comprehensive error handling and logging
        /// </summary>
        /// <param name="newCourt">The court to pledge allegiance to</param>
        /// <remarks>
        /// This method changes the player's court allegiance, a significant story choice.
        /// The allegiance change immediately affects reputation with all courts.
        /// Event handlers are protected to prevent allegiance failures from breaking the game.
        /// If player character is null, the operation safely fails without changing state.
        /// </remarks>
        public void ChangeCourtAllegiance(Court newCourt)
        {
            try
            {
                // Validation
                if (playerCharacter == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", "Cannot change court allegiance: player character is null");
                    return;
                }
                
                Court previousCourt = playerCharacter.allegiance;
                playerCharacter.allegiance = newCourt;
                
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                    "GameManager", $"{playerCharacter.name} now serves the {newCourt} Court");
                
                // Trigger event with protection
                try
                {
                    GameEvents.TriggerCourtAllegianceChanged(playerCharacter, newCourt);
                }
                catch (System.Exception eventEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "GameManager", $"Exception in CourtAllegianceChanged event handler: {eventEx.Message}");
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "GameManager", $"Exception in ChangeCourtAllegiance: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Set game difficulty
        /// </summary>
        public void SetDifficulty(DifficultyLevel difficulty)
        {
            DifficultySettings.CurrentDifficulty = difficulty;
            Debug.Log($"Difficulty set to: {difficulty}");
            DifficultySettings.DisplaySettings();
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
            Debug.Log($"Gold: {currencySystem.Gold}");
            
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
            
            List<InventoryItem> allItems = inventorySystem.GetAllItems();
            Debug.Log($"Inventory: {allItems.Count} unique items");
            foreach (InventoryItem item in allItems)
            {
                Debug.Log($"  - {item.name} x{item.quantity} ({item.rarity})");
            }
            
            Debug.Log("\n=== Inventory Demo Complete ===\n");

            Debug.Log("\n╔══════════════════════════════════════════════════════════╗");
            Debug.Log("║    Phase 5 Advanced Gameplay Systems Demo Complete!     ║");
            Debug.Log("║    All systems operational and ready for gameplay!       ║");
            Debug.Log("╚══════════════════════════════════════════════════════════╝\n");
        }

        /// <summary>
        /// Demo new base game enhancements
        /// </summary>
        public void DemoBaseGameEnhancements()
        {
            Debug.Log("\n\n╔══════════════════════════════════════════════════════════╗");
            Debug.Log("║    BASE GAME ENHANCEMENTS Demo                          ║");
            Debug.Log("╚══════════════════════════════════════════════════════════╝\n");

            // 1. Difficulty Settings Demo
            Debug.Log("=== 1. DIFFICULTY SETTINGS ===\n");
            DifficultySettings.DisplaySettings();
            
            Debug.Log("Changing difficulty to Hard...");
            SetDifficulty(DifficultyLevel.Hard);
            
            Debug.Log("\n=== Difficulty Demo Complete ===\n");

            // 2. Currency System Demo
            Debug.Log("=== 2. CURRENCY SYSTEM ===\n");
            
            currencySystem.DisplayCurrency();
            
            Debug.Log("Earning gold from combat...");
            Enemy testEnemy = EnemyFactory.CreateBogge(EnemyDifficulty.Normal);
            int goldDrop = CurrencySystem.CalculateEnemyGoldDrop(testEnemy);
            currencySystem.AddGold(goldDrop);
            
            Debug.Log("Earning court tokens...");
            currencySystem.AddCourtTokens(Court.Night, 10);
            currencySystem.AddFaeCrystals(5);
            
            currencySystem.DisplayCurrency();
            
            Debug.Log("\n=== Currency Demo Complete ===\n");

            // 3. Elemental System Demo
            Debug.Log("=== 3. ELEMENTAL SYSTEM ===\n");
            
            Debug.Log("Fire vs Ice: " + ElementalSystem.GetDamageMultiplier(Element.Fire, Element.Ice));
            Debug.Log("Ice vs Fire: " + ElementalSystem.GetDamageMultiplier(Element.Ice, Element.Fire));
            Debug.Log("Darkness vs Light: " + ElementalSystem.GetDamageMultiplier(Element.Darkness, Element.Light));
            Debug.Log("Light vs Darkness: " + ElementalSystem.GetDamageMultiplier(Element.Light, Element.Darkness));
            
            ElementalSystem.DisplayElementInfo(Element.Darkness);
            
            Debug.Log("\n=== Elemental Demo Complete ===\n");

            // 4. Status Effects Demo
            Debug.Log("=== 4. STATUS EFFECTS ===\n");
            
            Debug.Log("Applying Burning to enemy...");
            Enemy burnTarget = EnemyFactory.CreateNaga(EnemyDifficulty.Easy);
            statusEffectManager.ApplyEffect(burnTarget, StatusEffectType.Burning, 3, 1);
            
            Debug.Log("Applying Strengthened to player...");
            statusEffectManager.ApplyEffect(playerCharacter, StatusEffectType.Strengthened, 3, 1);
            statusEffectManager.ApplyEffect(playerCharacter, StatusEffectType.Regenerating, 5, 2);
            
            Debug.Log("\nPlayer active effects:");
            foreach (var effect in statusEffectManager.GetActiveEffects(playerCharacter))
            {
                Debug.Log($"  - {effect.name}: {effect.description} ({effect.duration} turns)");
            }
            
            Debug.Log("\nProcessing turn for player...");
            statusEffectManager.ProcessTurnStart(playerCharacter);
            
            Debug.Log("\nProcessing turn for enemy...");
            statusEffectManager.ProcessTurnStart(burnTarget);
            
            Debug.Log("\n=== Status Effects Demo Complete ===\n");

            // 5. Save System Demo
            Debug.Log("=== 5. ENHANCED SAVE SYSTEM ===\n");
            
            SaveSystem.DisplaySaveSlots();
            
            Debug.Log($"Current play time: {SaveSystem.GetPlayTimeHours():F2} hours");
            Debug.Log($"Auto-save enabled: {SaveSystem.AutoSaveEnabled}");
            
            Debug.Log("\n=== Save System Demo Complete ===\n");

            Debug.Log("\n╔══════════════════════════════════════════════════════════╗");
            Debug.Log("║    Base Game Enhancements Demo Complete!                ║");
            Debug.Log("║    New systems: Difficulty, Currency, Elements, Status  ║");
            Debug.Log("╚══════════════════════════════════════════════════════════╝\n");
        }

        /// <summary>
        /// Get inventory system (for external access)
        /// NOTE: Prefer using the 'inventory' property for cleaner code
        /// </summary>
        public InventorySystem GetInventorySystem()
        {
            return inventorySystem;
        }

        /// <summary>
        /// Get reputation system (for external access)
        /// NOTE: Prefer using the 'reputation' property for cleaner code
        /// </summary>
        public ReputationSystem GetReputationSystem()
        {
            return reputationSystem;
        }

        /// <summary>
        /// Get crafting system (for external access)
        /// NOTE: Prefer using the 'crafting' property for cleaner code
        /// </summary>
        public CraftingSystem GetCraftingSystem()
        {
            return craftingSystem;
        }

        /// <summary>
        /// Get currency system (for external access)
        /// NOTE: Prefer using the 'currency' property for cleaner code
        /// </summary>
        public CurrencySystem GetCurrencySystem()
        {
            return currencySystem;
        }

        /// <summary>
        /// Get status effect manager (for external access)
        /// NOTE: Prefer using the 'statusEffects' property for cleaner code
        /// </summary>
        public StatusEffectManager GetStatusEffectManager()
        {
            return statusEffectManager;
        }

        /// <summary>
        /// Check if all critical game systems are initialized (v2.5.2: NEW)
        /// Useful for debugging initialization issues
        /// </summary>
        public bool AreSystemsInitialized()
        {
            return inventorySystem != null 
                && reputationSystem != null 
                && craftingSystem != null
                && currencySystem != null
                && statusEffectManager != null;
        }

        /// <summary>
        /// Validate game state for safe operations (v2.5.2: NEW)
        /// </summary>
        public bool IsGameReady()
        {
            return Instance != null 
                && playerCharacter != null 
                && AreSystemsInitialized();
        }
    }
}
