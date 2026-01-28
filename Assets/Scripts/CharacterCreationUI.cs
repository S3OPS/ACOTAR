using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Manages the character creation interface
    /// Allows player to choose name, class, and court allegiance
    /// </summary>
    public class CharacterCreationUI : MonoBehaviour
    {
        [Header("Input Fields")]
        public InputField characterNameInput;

        [Header("Class Selection")]
        public Dropdown classDropdown;
        public Text classDescriptionText;

        [Header("Court Selection")]
        public Dropdown courtDropdown;
        public Text courtDescriptionText;

        [Header("Stat Preview")]
        public Text healthPreviewText;
        public Text magicPreviewText;
        public Text strengthPreviewText;
        public Text agilityPreviewText;

        [Header("Buttons")]
        public Button confirmButton;
        public Button randomizeButton;
        public Button backButton;

        private CharacterClass selectedClass = CharacterClass.Human;
        private Court selectedCourt = Court.None;

        private Dictionary<CharacterClass, string> classDescriptions = new Dictionary<CharacterClass, string>()
        {
            { CharacterClass.HighFae, "Powerful immortal beings with enhanced abilities and natural magic affinity. Balanced stats for versatile playstyle." },
            { CharacterClass.Illyrian, "Elite warrior race with incredible physical prowess and combat skills. High strength and agility, moderate magic." },
            { CharacterClass.LesserFae, "Agile magical creatures with quick reflexes. Balanced approach with emphasis on speed and magic." },
            { CharacterClass.Human, "Mortals with great potential. Can be Made by the Cauldron. Start weaker but have unique transformation path." },
            { CharacterClass.Attor, "Flying monsters with incredible speed and deadly attacks. High agility, low magic." },
            { CharacterClass.Suriel, "Ancient prophetic beings with vast magical knowledge. Highest magic, lowest physical stats." }
        };

        private Dictionary<Court, string> courtDescriptions = new Dictionary<Court, string>()
        {
            { Court.None, "No court allegiance. You walk your own path." },
            { Court.Spring, "Court of eternal spring. Ruled by Tamlin. Known for shapeshifters and nature magic." },
            { Court.Summer, "Coastal paradise ruled by Tarquin. Masters of water magic and diplomacy." },
            { Court.Autumn, "Forest realm of eternal fall. Ruled by Beron. Fire magic and old traditions." },
            { Court.Winter, "Frozen kingdom ruled by Kallias. Ice magic and fierce warriors." },
            { Court.Night, "The most powerful court, split between Velaris and Hewn City. Ruled by Rhysand. Masters of darkness and dreams." },
            { Court.Dawn, "Land of perpetual sunrise. Ruled by Thesan. Healers and scholars." },
            { Court.Day, "Bathed in eternal sunlight. Ruled by Helion. Light magic and ancient knowledge." }
        };

        void Start()
        {
            InitializeDropdowns();
            SetupEventListeners();
            UpdatePreview();
        }

        /// <summary>
        /// Initialize dropdown menus with options
        /// </summary>
        private void InitializeDropdowns()
        {
            // Setup class dropdown
            if (classDropdown != null)
            {
                classDropdown.ClearOptions();
                List<string> classOptions = new List<string>();
                
                foreach (CharacterClass charClass in System.Enum.GetValues(typeof(CharacterClass)))
                {
                    classOptions.Add(charClass.ToString());
                }
                
                classDropdown.AddOptions(classOptions);
                classDropdown.value = 0; // Default to first option
            }

            // Setup court dropdown
            if (courtDropdown != null)
            {
                courtDropdown.ClearOptions();
                List<string> courtOptions = new List<string>();
                
                foreach (Court court in System.Enum.GetValues(typeof(Court)))
                {
                    courtOptions.Add(court.ToString());
                }
                
                courtDropdown.AddOptions(courtOptions);
                courtDropdown.value = 0; // Default to None
            }
        }

        /// <summary>
        /// Setup button and dropdown event listeners
        /// </summary>
        private void SetupEventListeners()
        {
            if (classDropdown != null)
            {
                classDropdown.onValueChanged.AddListener(OnClassChanged);
            }

            if (courtDropdown != null)
            {
                courtDropdown.onValueChanged.AddListener(OnCourtChanged);
            }

            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmClicked);
            }

            if (randomizeButton != null)
            {
                randomizeButton.onClick.AddListener(OnRandomizeClicked);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        /// <summary>
        /// Handle class selection change
        /// </summary>
        private void OnClassChanged(int index)
        {
            selectedClass = (CharacterClass)index;
            UpdatePreview();
            UpdateClassDescription();
        }

        /// <summary>
        /// Handle court selection change
        /// </summary>
        private void OnCourtChanged(int index)
        {
            selectedCourt = (Court)index;
            UpdateCourtDescription();
        }

        /// <summary>
        /// Update stat preview based on selected class
        /// </summary>
        private void UpdatePreview()
        {
            // Get base stats for selected class
            Dictionary<CharacterClass, (int health, int magic, int strength, int agility)> baseStats = 
                new Dictionary<CharacterClass, (int, int, int, int)>()
            {
                { CharacterClass.HighFae, (150, 100, 80, 70) },
                { CharacterClass.Illyrian, (180, 60, 120, 90) },
                { CharacterClass.LesserFae, (100, 60, 60, 80) },
                { CharacterClass.Human, (80, 0, 50, 60) },
                { CharacterClass.Attor, (120, 40, 90, 100) },
                { CharacterClass.Suriel, (70, 150, 30, 40) }
            };

            if (baseStats.ContainsKey(selectedClass))
            {
                var stats = baseStats[selectedClass];
                
                if (healthPreviewText != null)
                    healthPreviewText.text = $"Health: {stats.health}";
                
                if (magicPreviewText != null)
                    magicPreviewText.text = $"Magic: {stats.magic}";
                
                if (strengthPreviewText != null)
                    strengthPreviewText.text = $"Strength: {stats.strength}";
                
                if (agilityPreviewText != null)
                    agilityPreviewText.text = $"Agility: {stats.agility}";
            }
        }

        /// <summary>
        /// Update class description text
        /// </summary>
        private void UpdateClassDescription()
        {
            if (classDescriptionText != null && classDescriptions.ContainsKey(selectedClass))
            {
                classDescriptionText.text = classDescriptions[selectedClass];
            }
        }

        /// <summary>
        /// Update court description text
        /// </summary>
        private void UpdateCourtDescription()
        {
            if (courtDescriptionText != null && courtDescriptions.ContainsKey(selectedCourt))
            {
                courtDescriptionText.text = courtDescriptions[selectedCourt];
            }
        }

        /// <summary>
        /// Randomize character selections
        /// </summary>
        private void OnRandomizeClicked()
        {
            // Random class
            int randomClass = Random.Range(0, System.Enum.GetValues(typeof(CharacterClass)).Length);
            if (classDropdown != null)
            {
                classDropdown.value = randomClass;
            }

            // Random court
            int randomCourt = Random.Range(0, System.Enum.GetValues(typeof(Court)).Length);
            if (courtDropdown != null)
            {
                courtDropdown.value = randomCourt;
            }

            // Random name
            if (characterNameInput != null)
            {
                string[] randomNames = { "Feyre", "Elain", "Nesta", "Mor", "Amren", "Vassa", "Gwyn", "Emerie" };
                characterNameInput.text = randomNames[Random.Range(0, randomNames.Length)];
            }

            Debug.Log("Character randomized");
        }

        /// <summary>
        /// Confirm character creation
        /// </summary>
        private void OnConfirmClicked()
        {
            string characterName = characterNameInput != null ? characterNameInput.text : "Unnamed";

            if (string.IsNullOrWhiteSpace(characterName))
            {
                Debug.LogWarning("Character name cannot be empty!");
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Please enter a character name!");
                }
                return;
            }

            // Create character
            CreateCharacter(characterName, selectedClass, selectedCourt);

            // Hide character creation panel and show HUD
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePanel("CharacterCreation");
                UIManager.Instance.ShowPanel("HUD");
            }

            Debug.Log($"Character created: {characterName}, Class: {selectedClass}, Court: {selectedCourt}");
        }

        /// <summary>
        /// Create character and initialize game
        /// </summary>
        private void CreateCharacter(string name, CharacterClass charClass, Court court)
        {
            if (GameManager.Instance != null && GameManager.Instance.playerCharacter != null)
            {
                // Set character properties
                GameManager.Instance.playerCharacter.characterName = name;
                GameManager.Instance.playerCharacter.characterClass = charClass;
                GameManager.Instance.playerCharacter.courtAllegiance = court;

                // Initialize stats based on class
                GameManager.Instance.playerCharacter.InitializeStats();

                // Notify game manager
                Debug.Log($"Player character initialized: {name}");
                
                // Update UI
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.UpdateHUD(GameManager.Instance.playerCharacter);
                    UIManager.Instance.ShowNotification($"Welcome, {name} of {court} Court!");
                }
            }
            else
            {
                Debug.LogError("GameManager or player character not found!");
            }
        }

        /// <summary>
        /// Return to main menu
        /// </summary>
        private void OnBackClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePanel("CharacterCreation");
                UIManager.Instance.ShowPanel("MainMenu");
            }
        }

        /// <summary>
        /// Validate character name input
        /// </summary>
        public bool ValidateCharacterName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (name.Length < 2 || name.Length > 20)
            {
                return false;
            }

            return true;
        }
    }
}
