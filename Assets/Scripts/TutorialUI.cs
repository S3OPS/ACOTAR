using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ACOTAR
{
    /// <summary>
    /// Tutorial and help system for new players
    /// Provides context-sensitive guidance and game mechanics explanations
    /// </summary>
    public class TutorialUI : MonoBehaviour
    {
        public static TutorialUI Instance { get; private set; }

        [Header("UI References")]
        public GameObject tutorialPanel;
        public GameObject helpPanel;
        public Text tutorialTitleText;
        public Text tutorialContentText;
        public Button nextButton;
        public Button prevButton;
        public Button skipButton;
        public Button closeButton;
        
        [Header("Help Menu")]
        public GameObject helpMenuPanel;
        public GameObject helpTopicListContainer;
        public GameObject helpTopicButtonPrefab;
        public Text helpTopicTitleText;
        public Text helpTopicContentText;
        public ScrollRect helpContentScroll;
        
        [Header("Tutorial Popup")]
        public GameObject tutorialPopup;
        public Text popupTitleText;
        public Text popupContentText;
        public Button popupCloseButton;
        public Button popupDontShowAgainButton;
        
        private List<TutorialStep> currentTutorial = new List<TutorialStep>();
        private int currentStepIndex = 0;
        private Dictionary<string, bool> completedTutorials = new Dictionary<string, bool>();
        private Dictionary<string, bool> disabledTutorials = new Dictionary<string, bool>();
        private Dictionary<string, HelpTopic> helpTopics = new Dictionary<string, HelpTopic>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeHelpTopics();
            SetupListeners();
            HideAllPanels();
            LoadTutorialProgress();
        }

        /// <summary>
        /// Setup UI event listeners
        /// </summary>
        private void SetupListeners()
        {
            if (nextButton != null) nextButton.onClick.AddListener(ShowNextStep);
            if (prevButton != null) prevButton.onClick.AddListener(ShowPreviousStep);
            if (skipButton != null) skipButton.onClick.AddListener(SkipTutorial);
            if (closeButton != null) closeButton.onClick.AddListener(HideTutorialPanel);
            if (popupCloseButton != null) popupCloseButton.onClick.AddListener(HideTutorialPopup);
            if (popupDontShowAgainButton != null) popupDontShowAgainButton.onClick.AddListener(DisableCurrentTutorial);
        }

        /// <summary>
        /// Initialize help topics with game information
        /// </summary>
        private void InitializeHelpTopics()
        {
            // Game Basics
            AddHelpTopic("Getting Started", "Game Basics", 
                "Welcome to ACOTAR RPG!\n\n" +
                "You play as Feyre Archeron, beginning as a human hunter in the mortal lands. " +
                "Your journey will take you through all of Prythian, from the Spring Court to Under the Mountain and beyond.\n\n" +
                "Key Features:\n" +
                "• Character progression with XP and levels\n" +
                "• Turn-based combat system\n" +
                "• Quest-driven story following the books\n" +
                "• Companion system with 9 recruitable characters\n" +
                "• Reputation with all 7 courts\n" +
                "• Crafting, dialogue, and time systems");

            // Controls
            AddHelpTopic("Controls", "Game Basics",
                "Keyboard Controls:\n\n" +
                "I - Open/Close Inventory\n" +
                "Q - Open/Close Quest Log\n" +
                "M - Open/Close Map\n" +
                "ESC - Pause Menu\n\n" +
                "Mouse:\n" +
                "• Click buttons to interact\n" +
                "• Click items in inventory to select\n" +
                "• Click enemies in combat to target\n" +
                "• Click locations on map to view details");

            // Character System
            AddHelpTopic("Character Classes", "Character System",
                "Six character classes are available:\n\n" +
                "High Fae - Balanced magic users with immortality\n" +
                "Illyrian - Powerful warriors with wings\n" +
                "Lesser Fae - Agile spellcasters\n" +
                "Human - Can be Made by the Cauldron\n" +
                "Attor - Fast attackers\n" +
                "Suriel - Prophetic beings with immense magic\n\n" +
                "Each class has different base stats and abilities. Choose wisely!");

            AddHelpTopic("Stats & Leveling", "Character System",
                "Character Stats:\n\n" +
                "Health - Your life force. Reaches 0 = death\n" +
                "Magic Power - Strength of magical abilities\n" +
                "Strength - Physical attack damage\n" +
                "Agility - Dodge chance and initiative\n\n" +
                "Leveling Up:\n" +
                "Gain experience (XP) by completing quests and winning battles. " +
                "Each level increases all stats and may unlock new abilities.");

            // Combat System
            AddHelpTopic("Combat Basics", "Combat",
                "Turn-Based Combat:\n\n" +
                "Actions:\n" +
                "• Attack - Physical damage based on Strength\n" +
                "• Magic - Cast abilities using Magic Power\n" +
                "• Defend - Reduce incoming damage by 50%\n" +
                "• Item - Use consumables from inventory\n" +
                "• Flee - Attempt to escape (based on Agility)\n\n" +
                "Combat Tips:\n" +
                "• Critical hits deal 2x damage (15% chance)\n" +
                "• High Agility helps dodge attacks\n" +
                "• Magic is powerful but watch your Magic Power");

            AddHelpTopic("Enemy Types", "Combat",
                "Enemy Difficulty Levels:\n\n" +
                "Trivial - Easy opponents, good for practice\n" +
                "Easy - Slightly challenging\n" +
                "Normal - Standard difficulty\n" +
                "Hard - Tough fights, prepare well\n" +
                "Elite - Very dangerous\n" +
                "Boss - Legendary enemies, epic battles\n\n" +
                "Enemy AI Behaviors:\n" +
                "• Aggressive - All-out attacks\n" +
                "• Defensive - Prioritizes survival\n" +
                "• Tactical - Smart ability usage\n" +
                "• Berserker - Double attacks when hurt\n" +
                "• Balanced - Mix of offense and defense");

            // Quest System
            AddHelpTopic("Quests", "Quests & Story",
                "Quest Types:\n\n" +
                "Main Story - Follow the book's narrative\n" +
                "Side Quest - Optional but rewarding\n" +
                "Court Quest - Build reputation with courts\n" +
                "Companion Quest - Personal storylines\n\n" +
                "Quest Rewards:\n" +
                "• Experience points (XP)\n" +
                "• Gold and items\n" +
                "• Reputation changes\n" +
                "• Unlocked locations or characters\n\n" +
                "Use Q key to open Quest Log and track objectives.");

            // Inventory & Crafting
            AddHelpTopic("Inventory", "Items & Crafting",
                "Inventory System:\n\n" +
                "50 slot inventory with item stacking\n" +
                "Equipment slots: Weapon, Armor\n\n" +
                "Item Rarity:\n" +
                "Common (White) → Uncommon (Green) → Rare (Blue) → Epic (Purple) → Legendary (Orange) → Artifact (Red)\n\n" +
                "Item Types:\n" +
                "• Weapons - Increase attack damage\n" +
                "• Armor - Increase defense\n" +
                "• Consumables - Healing potions, etc.\n" +
                "• Quest Items - Cannot be dropped\n" +
                "• Crafting - Used in recipes\n" +
                "• Magical - Special enchanted items");

            AddHelpTopic("Crafting", "Items & Crafting",
                "Crafting System:\n\n" +
                "5 Crafting Stations:\n" +
                "• Workbench - Basic items\n" +
                "• Forge - Weapons and armor\n" +
                "• Alchemy Table - Potions\n" +
                "• Enchanting Table - Magical items\n" +
                "• Cooking Fire - Food (future)\n\n" +
                "To craft:\n" +
                "1. Gather required materials\n" +
                "2. Meet level requirement\n" +
                "3. Access appropriate station\n" +
                "4. Select recipe and craft\n\n" +
                "Crafting takes time - be patient!");

            // Reputation & Courts
            AddHelpTopic("Court Reputation", "Courts & Politics",
                "Reputation System:\n\n" +
                "Build standing with 7 High Fae Courts:\n" +
                "Spring, Summer, Autumn, Winter, Night, Dawn, Day\n\n" +
                "Reputation Levels:\n" +
                "Hostile → Unfriendly → Neutral → Friendly → Honored → Revered → Exalted\n\n" +
                "Benefits:\n" +
                "• Shop discounts (up to 50%)\n" +
                "• Access to exclusive areas\n" +
                "• Special missions and quests\n" +
                "• Companion recruitment\n\n" +
                "Warning: Some courts are rivals. Gaining reputation with one may hurt another!");

            // Companions
            AddHelpTopic("Companions", "Companions",
                "Companion System:\n\n" +
                "Recruit up to 9 legendary characters:\n" +
                "Rhysand, Cassian, Azriel, Mor, Amren, Lucien, Tamlin, Nesta, Elain\n\n" +
                "Party Management:\n" +
                "• Maximum 3 active companions\n" +
                "• Each has unique role (Tank, DPS, Support, Balanced)\n" +
                "• Loyalty affects combat effectiveness (80-120%)\n\n" +
                "Increase Loyalty:\n" +
                "• Complete companion quests\n" +
                "• Make story choices they agree with\n" +
                "• Keep them in active party\n\n" +
                "Low loyalty reduces their effectiveness in battle!");

            // Magic & Abilities
            AddHelpTopic("Magic System", "Magic & Abilities",
                "Magic Abilities:\n\n" +
                "16 different magic types:\n" +
                "• Shapeshifting - Change form\n" +
                "• Winnowing - Teleportation\n" +
                "• Elemental - Fire, Water, Wind, Ice\n" +
                "• Daemati - Mind reading/control\n" +
                "• Shadowsinger - Darkness manipulation\n" +
                "• Light Wielder - Truth and illumination\n" +
                "• Healing - Restore health\n" +
                "• Shields - Create barriers\n" +
                "• Seer - Prophetic visions\n" +
                "• And more!\n\n" +
                "Learn abilities through story progression and special quests.");

            AddHelpTopic("Moon Phases", "Magic & Abilities",
                "Celestial Magic:\n\n" +
                "The moon affects magic power:\n" +
                "• Full Moon: +30% magic power\n" +
                "• Gibbous Moons: +15% magic power\n" +
                "• Quarter/Crescent: Normal\n" +
                "• New Moon: -15% magic power\n\n" +
                "Special Events:\n" +
                "• Calanmai (Fire Night) - Spring's first full moon\n" +
                "• Starfall - Night Court's celestial event\n\n" +
                "Plan important battles around moon phases!");

            // Saving & Settings
            AddHelpTopic("Saving Your Game", "Game Management",
                "Save System:\n\n" +
                "5 Save Slots Available\n" +
                "• Save from Pause Menu\n" +
                "• Auto-save after major events\n" +
                "• Save includes all progress\n\n" +
                "What's Saved:\n" +
                "• Character stats and abilities\n" +
                "• Quest progress\n" +
                "• Inventory and equipment\n" +
                "• Story progression\n" +
                "• Reputation and relationships\n" +
                "• Play time and location\n\n" +
                "TIP: Save often, especially before tough battles!");

            // Book 1 Specific Content
            AddHelpTopic("Book 1 Overview", "Book 1: Thorns and Roses",
                "A Court of Thorns and Roses - Base Game\n\n" +
                "Your Journey:\n" +
                "• Start as Feyre, a human hunter in the mortal lands\n" +
                "• Travel to the Spring Court after killing a Fae\n" +
                "• Meet Tamlin and Lucien, learn about the curse\n" +
                "• Face Amarantha's trials Under the Mountain\n" +
                "• Break the curse and transform into High Fae\n\n" +
                "Key Story Arcs:\n" +
                "1. Human Lands (Level 1-2)\n" +
                "2. Spring Court (Level 3-5)\n" +
                "3. Under the Mountain (Level 6-9)\n" +
                "4. Aftermath (Level 10)\n\n" +
                "Total XP Available: ~6,250\n" +
                "Expected Completion: 10-15 hours");

            AddHelpTopic("The Three Trials", "Book 1: Thorns and Roses",
                "Amarantha's Challenges:\n\n" +
                "Trial 1: The Middengard Wyrm\n" +
                "• Face a deadly worm in a flooded chamber\n" +
                "• Tip: Use agility to dodge, focus on critical hits\n" +
                "• Reward: 400 XP\n\n" +
                "Trial 2: The Naga\n" +
                "• Answer riddles or fight the poisonous Naga\n" +
                "• Tip: If you can't read, prepare for combat\n" +
                "• Reward: 450 XP\n\n" +
                "Trial 3: Hearts of Stone\n" +
                "• Make an impossible choice\n" +
                "• Tip: Think about what's truly in your heart\n" +
                "• Reward: 500 XP\n\n" +
                "Final Challenge: The Riddle\n" +
                "• Answer correctly to break the curse\n" +
                "• Reward: 600 XP + transformation");

            AddHelpTopic("Difficulty Modes", "Game Management",
                "Choose Your Challenge:\n\n" +
                "STORY MODE (Easy)\n" +
                "• 50% damage taken\n" +
                "• 150% damage dealt\n" +
                "• 150% XP and gold rewards\n" +
                "• Perfect for experiencing the story\n\n" +
                "NORMAL MODE (Balanced)\n" +
                "• 100% damage (standard)\n" +
                "• 100% rewards\n" +
                "• Recommended for first playthrough\n\n" +
                "HARD MODE (Challenge)\n" +
                "• 125% damage taken\n" +
                "• 90% damage dealt\n" +
                "• 125% XP and gold rewards\n" +
                "• For experienced players\n\n" +
                "NIGHTMARE MODE (Brutal)\n" +
                "• 150% damage taken\n" +
                "• 75% damage dealt\n" +
                "• 150% XP and gold rewards\n" +
                "• Extreme challenge, deaths are punishing\n\n" +
                "TIP: Start on Normal, adjust in settings if needed!");

            AddHelpTopic("Combat Strategy", "Combat",
                "Winning Battles:\n\n" +
                "Basic Strategy:\n" +
                "• Use Attack for consistent physical damage\n" +
                "• Save Magic for tough enemies or healing\n" +
                "• Defend when low on health to buy time\n" +
                "• Use items strategically (potions, buffs)\n\n" +
                "Advanced Tips:\n" +
                "• Critical hits (15% chance) deal 2x damage\n" +
                "• High Agility = better dodge chance (up to 75%)\n" +
                "• Defend reduces damage by 50% for one turn\n" +
                "• Flee chance based on Agility vs enemy level\n\n" +
                "Boss Battles:\n" +
                "• Always save before major fights\n" +
                "• Stock up on healing potions\n" +
                "• Bring companions for support\n" +
                "• Learn enemy patterns and exploit weaknesses\n\n" +
                "Party Combat:\n" +
                "• Companions fight automatically based on role\n" +
                "• Tank companions protect the party\n" +
                "• DPS companions deal high damage\n" +
                "• Support companions heal and buff\n" +
                "• Loyalty affects effectiveness (80-120%)");

            AddHelpTopic("Progression Tips", "Character System",
                "Leveling Efficiently:\n\n" +
                "Early Game (Levels 1-3):\n" +
                "• Complete all side quests for extra XP\n" +
                "• Don't skip combat encounters\n" +
                "• Learn to use Defend action effectively\n" +
                "• Save gold for better equipment\n\n" +
                "Mid Game (Levels 4-7):\n" +
                "• Build reputation with Spring Court\n" +
                "• Unlock crafting for better gear\n" +
                "• Focus on companion relationships\n" +
                "• Prepare for Under the Mountain\n\n" +
                "Late Game (Levels 8-10):\n" +
                "• Master all combat actions\n" +
                "• Complete trial challenges\n" +
                "• Max out key abilities\n" +
                "• Prepare for transformation\n\n" +
                "Stat Priorities:\n" +
                "• Warriors: Strength > Health > Agility\n" +
                "• Mages: Magic > Agility > Health\n" +
                "• Balanced: Health > Strength = Magic");

            AddHelpTopic("Spring Court Guide", "Book 1: Thorns and Roses",
                "Life at the Manor:\n\n" +
                "Key Locations:\n" +
                "• Spring Court Manor - Your home in Prythian\n" +
                "• Gardens - Peaceful exploration area\n" +
                "• Training Grounds - Practice combat\n" +
                "• The Wall - Border to human lands\n\n" +
                "Important NPCs:\n" +
                "• Tamlin - High Lord of Spring, cursed beast\n" +
                "• Lucien - Emissary, becomes your friend\n" +
                "• Alis - Servant who helps you adjust\n\n" +
                "Activities:\n" +
                "• Complete quests to build relationships\n" +
                "• Learn about the curse affecting the court\n" +
                "• Prepare for Calanmai (Fire Night)\n" +
                "• Discover the truth about Amarantha\n\n" +
                "Reputation:\n" +
                "• Gain Spring Court reputation through quests\n" +
                "• Higher reputation = better prices at shops\n" +
                "• Unlocks unique dialogue and story options");

            PopulateHelpTopicList();
        }

        /// <summary>
        /// Add a help topic to the system
        /// </summary>
        private void AddHelpTopic(string title, string category, string content)
        {
            string key = $"{category}_{title}";
            helpTopics[key] = new HelpTopic
            {
                title = title,
                category = category,
                content = content
            };
        }

        /// <summary>
        /// Populate the help topic list in the UI
        /// </summary>
        private void PopulateHelpTopicList()
        {
            if (helpTopicListContainer == null || helpTopicButtonPrefab == null) return;

            // Clear existing buttons
            foreach (Transform child in helpTopicListContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Group topics by category
            Dictionary<string, List<HelpTopic>> categorizedTopics = new Dictionary<string, List<HelpTopic>>();
            foreach (var topic in helpTopics.Values)
            {
                if (!categorizedTopics.ContainsKey(topic.category))
                {
                    categorizedTopics[topic.category] = new List<HelpTopic>();
                }
                categorizedTopics[topic.category].Add(topic);
            }

            // Create buttons for each topic
            foreach (var category in categorizedTopics)
            {
                // Category header
                GameObject header = Instantiate(helpTopicButtonPrefab, helpTopicListContainer.transform);
                Text headerText = header.GetComponentInChildren<Text>();
                if (headerText != null)
                {
                    headerText.text = $"--- {category.Key} ---";
                    headerText.fontStyle = FontStyle.Bold;
                }
                Button headerButton = header.GetComponent<Button>();
                if (headerButton != null) headerButton.interactable = false;

                // Topic buttons
                foreach (var topic in category.Value)
                {
                    GameObject topicButton = Instantiate(helpTopicButtonPrefab, helpTopicListContainer.transform);
                    Text buttonText = topicButton.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = "  " + topic.title;
                    }

                    Button button = topicButton.GetComponent<Button>();
                    if (button != null)
                    {
                        HelpTopic t = topic; // Capture for lambda
                        button.onClick.AddListener(() => DisplayHelpTopic(t));
                    }
                }
            }
        }

        /// <summary>
        /// Display a help topic's content
        /// </summary>
        private void DisplayHelpTopic(HelpTopic topic)
        {
            if (helpTopicTitleText != null) helpTopicTitleText.text = topic.title;
            if (helpTopicContentText != null) helpTopicContentText.text = topic.content;
            
            // Scroll to top
            if (helpContentScroll != null)
            {
                helpContentScroll.verticalNormalizedPosition = 1f;
            }
        }

        #region Tutorial System

        /// <summary>
        /// Start a tutorial sequence
        /// </summary>
        public void StartTutorial(string tutorialId, List<TutorialStep> steps)
        {
            // Check if already completed or disabled
            if (completedTutorials.ContainsKey(tutorialId) && completedTutorials[tutorialId]) return;
            if (disabledTutorials.ContainsKey(tutorialId) && disabledTutorials[tutorialId]) return;

            currentTutorial = steps;
            currentStepIndex = 0;
            ShowTutorialPanel();
            DisplayCurrentStep();
        }

        /// <summary>
        /// Show a quick tutorial popup
        /// </summary>
        public void ShowTutorialPopup(string tutorialId, string title, string content)
        {
            // Check if disabled
            if (disabledTutorials.ContainsKey(tutorialId) && disabledTutorials[tutorialId]) return;

            if (tutorialPopup != null)
            {
                tutorialPopup.SetActive(true);
                if (popupTitleText != null) popupTitleText.text = title;
                if (popupContentText != null) popupContentText.text = content;
            }
        }

        private void ShowNextStep()
        {
            if (currentStepIndex < currentTutorial.Count - 1)
            {
                currentStepIndex++;
                DisplayCurrentStep();
            }
            else
            {
                CompleteTutorial();
            }
        }

        private void ShowPreviousStep()
        {
            if (currentStepIndex > 0)
            {
                currentStepIndex--;
                DisplayCurrentStep();
            }
        }

        private void DisplayCurrentStep()
        {
            if (currentStepIndex >= 0 && currentStepIndex < currentTutorial.Count)
            {
                TutorialStep step = currentTutorial[currentStepIndex];
                
                if (tutorialTitleText != null) tutorialTitleText.text = step.title;
                if (tutorialContentText != null) tutorialContentText.text = step.content;
                
                // Update button states
                if (prevButton != null) prevButton.interactable = currentStepIndex > 0;
                if (nextButton != null)
                {
                    Text buttonText = nextButton.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = (currentStepIndex == currentTutorial.Count - 1) ? "Finish" : "Next";
                    }
                }
            }
        }

        private void SkipTutorial()
        {
            CompleteTutorial();
        }

        private void CompleteTutorial()
        {
            HideTutorialPanel();
            // Mark as completed (would save this)
        }

        private void DisableCurrentTutorial()
        {
            // Mark tutorial as disabled so it won't show again
            HideTutorialPopup();
        }

        #endregion

        #region Panel Management

        private void ShowTutorialPanel()
        {
            if (tutorialPanel != null) tutorialPanel.SetActive(true);
        }

        private void HideTutorialPanel()
        {
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }

        public void ShowHelpMenu()
        {
            if (helpMenuPanel != null)
            {
                helpMenuPanel.SetActive(true);
                // Show first topic by default
                if (helpTopics.Count > 0)
                {
                    foreach (var topic in helpTopics.Values)
                    {
                        DisplayHelpTopic(topic);
                        break;
                    }
                }
            }
        }

        public void HideHelpMenu()
        {
            if (helpMenuPanel != null) helpMenuPanel.SetActive(false);
        }

        private void HideTutorialPopup()
        {
            if (tutorialPopup != null) tutorialPopup.SetActive(false);
        }

        private void HideAllPanels()
        {
            HideTutorialPanel();
            HideHelpMenu();
            HideTutorialPopup();
        }

        #endregion

        #region Save/Load

        private void LoadTutorialProgress()
        {
            // Load from PlayerPrefs
            if (PlayerPrefs.HasKey("CompletedTutorials"))
            {
                string json = PlayerPrefs.GetString("CompletedTutorials");
                // Deserialize completed tutorials
            }
        }

        private void SaveTutorialProgress()
        {
            // Save to PlayerPrefs
            // Serialize completed tutorials
            PlayerPrefs.Save();
        }

        #endregion
    }

    /// <summary>
    /// Represents a single step in a tutorial sequence
    /// </summary>
    [System.Serializable]
    public class TutorialStep
    {
        public string title;
        [TextArea(3, 10)]
        public string content;
    }

    /// <summary>
    /// Represents a help topic
    /// </summary>
    [System.Serializable]
    public class HelpTopic
    {
        public string title;
        public string category;
        [TextArea(5, 20)]
        public string content;
    }
}
