using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Manages combat UI display and player actions
    /// Handles turn-based combat interface
    /// </summary>
    public class CombatUI : MonoBehaviour
    {
        [Header("Combat Panel")]
        public GameObject combatPanel;

        [Header("Turn Display")]
        public Text turnIndicatorText;
        public Text combatPhaseText;

        [Header("Player Info")]
        public Slider playerHealthBar;
        public Text playerHealthText;
        public Slider playerMagicBar;
        public Text playerMagicText;

        [Header("Enemy Display")]
        public GameObject enemiesContainer;
        public GameObject enemyPanelPrefab;
        
        [Header("Action Buttons")]
        public Button attackButton;
        public Button magicButton;
        public Button defendButton;
        public Button itemButton;
        public Button fleeButton;

        [Header("Magic Abilities")]
        public GameObject magicPanel;
        public GameObject magicButtonPrefab;
        public GameObject magicAbilitiesContainer;

        [Header("Combat Log")]
        public Text combatLogText;
        public ScrollRect combatLogScrollRect;

        [Header("Result Screen")]
        public GameObject victoryPanel;
        public GameObject defeatPanel;
        public Text victoryRewardsText;

        private CombatEncounter currentEncounter;
        private List<GameObject> enemyPanels = new List<GameObject>();
        private Character playerCharacter;

        void Start()
        {
            InitializeCombatUI();
            SetupEventListeners();
        }

        /// <summary>
        /// Initialize combat UI
        /// </summary>
        private void InitializeCombatUI()
        {
            if (combatPanel != null)
            {
                combatPanel.SetActive(false);
            }

            if (magicPanel != null)
            {
                magicPanel.SetActive(false);
            }

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }

            if (defeatPanel != null)
            {
                defeatPanel.SetActive(false);
            }

            Debug.Log("CombatUI initialized");
        }

        /// <summary>
        /// Setup action button event listeners
        /// </summary>
        private void SetupEventListeners()
        {
            if (attackButton != null)
            {
                attackButton.onClick.AddListener(OnAttackClicked);
            }

            if (magicButton != null)
            {
                magicButton.onClick.AddListener(OnMagicClicked);
            }

            if (defendButton != null)
            {
                defendButton.onClick.AddListener(OnDefendClicked);
            }

            if (itemButton != null)
            {
                itemButton.onClick.AddListener(OnItemClicked);
            }

            if (fleeButton != null)
            {
                fleeButton.onClick.AddListener(OnFleeClicked);
            }
        }

        /// <summary>
        /// Start combat encounter
        /// </summary>
        public void StartCombat(CombatEncounter encounter, Character player)
        {
            currentEncounter = encounter;
            playerCharacter = player;

            if (combatPanel != null)
            {
                combatPanel.SetActive(true);
            }

            // Clear previous combat data
            ClearCombatLog();
            ClearEnemyPanels();

            // Setup player display
            UpdatePlayerDisplay();

            // Setup enemy displays
            CreateEnemyPanels(encounter.GetEnemies());

            // Initial log entry
            AddCombatLogEntry("Combat Started!");
            
            Debug.Log("Combat UI started");
        }

        /// <summary>
        /// Update player health and magic bars
        /// </summary>
        public void UpdatePlayerDisplay()
        {
            if (playerCharacter == null)
                return;

            // Health bar
            if (playerHealthBar != null)
            {
                playerHealthBar.maxValue = playerCharacter.stats.MaxHealth;
                playerHealthBar.value = playerCharacter.stats.CurrentHealth;
            }

            if (playerHealthText != null)
            {
                playerHealthText.text = $"{playerCharacter.stats.CurrentHealth} / {playerCharacter.stats.MaxHealth}";
            }

            // Magic bar
            if (playerMagicBar != null)
            {
                playerMagicBar.maxValue = playerCharacter.stats.MaxMagicPower;
                playerMagicBar.value = playerCharacter.magicPower;
            }

            if (playerMagicText != null)
            {
                playerMagicText.text = $"{playerCharacter.magicPower} / {playerCharacter.stats.MaxMagicPower}";
            }
        }

        /// <summary>
        /// Create UI panels for each enemy
        /// </summary>
        private void CreateEnemyPanels(List<Enemy> enemies)
        {
            if (enemiesContainer == null || enemyPanelPrefab == null)
                return;

            foreach (Enemy enemy in enemies)
            {
                GameObject enemyPanel = Instantiate(enemyPanelPrefab, enemiesContainer.transform);
                
                // Setup enemy name
                Text nameText = enemyPanel.transform.Find("NameText")?.GetComponent<Text>();
                if (nameText != null)
                {
                    nameText.text = enemy.characterName;
                }

                // Setup health bar
                Slider healthBar = enemyPanel.GetComponentInChildren<Slider>();
                if (healthBar != null)
                {
                    healthBar.maxValue = enemy.stats.MaxHealth;
                    healthBar.value = enemy.stats.CurrentHealth;
                }

                // Make enemy clickable for targeting
                Button targetButton = enemyPanel.GetComponent<Button>();
                if (targetButton != null)
                {
                    targetButton.onClick.AddListener(() => OnEnemyTargeted(enemy));
                }

                enemyPanels.Add(enemyPanel);
            }

            Debug.Log($"Created {enemyPanels.Count} enemy panels");
        }

        /// <summary>
        /// Clear enemy panels
        /// </summary>
        private void ClearEnemyPanels()
        {
            foreach (GameObject panel in enemyPanels)
            {
                if (panel != null)
                {
                    Destroy(panel);
                }
            }
            enemyPanels.Clear();
        }

        /// <summary>
        /// Update all enemy displays
        /// </summary>
        public void UpdateEnemyDisplays()
        {
            if (currentEncounter == null)
                return;

            List<Enemy> enemies = currentEncounter.GetEnemies();
            
            for (int i = 0; i < enemies.Count && i < enemyPanels.Count; i++)
            {
                Enemy enemy = enemies[i];
                GameObject panel = enemyPanels[i];

                if (panel != null)
                {
                    Slider healthBar = panel.GetComponentInChildren<Slider>();
                    if (healthBar != null)
                    {
                        healthBar.value = enemy.stats.CurrentHealth;
                    }

                    // Gray out if dead
                    if (enemy.stats.CurrentHealth <= 0)
                    {
                        Image panelImage = panel.GetComponent<Image>();
                        if (panelImage != null)
                        {
                            panelImage.color = Color.gray;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update turn indicator
        /// </summary>
        public void UpdateTurnIndicator(string turnText)
        {
            if (turnIndicatorText != null)
            {
                turnIndicatorText.text = turnText;
            }
        }

        /// <summary>
        /// Add entry to combat log
        /// </summary>
        public void AddCombatLogEntry(string message)
        {
            if (combatLogText != null)
            {
                combatLogText.text += message + "\n";
                
                // Auto-scroll to bottom
                if (combatLogScrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    combatLogScrollRect.verticalNormalizedPosition = 0f;
                }
            }

            Debug.Log($"Combat: {message}");
        }

        /// <summary>
        /// Clear combat log
        /// </summary>
        private void ClearCombatLog()
        {
            if (combatLogText != null)
            {
                combatLogText.text = "";
            }
        }

        /// <summary>
        /// Handle attack button click
        /// </summary>
        private void OnAttackClicked()
        {
            AddCombatLogEntry("Select an enemy to attack...");
            // Enemy selection is handled by clicking on enemy panels
            Debug.Log("Attack action selected");
        }

        /// <summary>
        /// Handle enemy targeting for attack
        /// </summary>
        private void OnEnemyTargeted(Enemy enemy)
        {
            if (currentEncounter == null || playerCharacter == null)
                return;

            if (enemy.stats.CurrentHealth > 0)
            {
                currentEncounter.PlayerPhysicalAttack(enemy);
                AddCombatLogEntry($"You attack {enemy.characterName}!");
                
                UpdatePlayerDisplay();
                UpdateEnemyDisplays();

                // Process enemy turn
                ProcessEnemyTurn();
            }
            else
            {
                AddCombatLogEntry($"{enemy.characterName} is already defeated!");
            }
        }

        /// <summary>
        /// Handle magic button click
        /// </summary>
        private void OnMagicClicked()
        {
            if (magicPanel != null)
            {
                magicPanel.SetActive(!magicPanel.activeSelf);
            }

            if (magicPanel.activeSelf)
            {
                DisplayMagicAbilities();
            }

            Debug.Log("Magic panel toggled");
        }

        /// <summary>
        /// Display available magic abilities
        /// </summary>
        private void DisplayMagicAbilities()
        {
            if (playerCharacter == null || magicAbilitiesContainer == null)
                return;

            // Clear existing buttons
            foreach (Transform child in magicAbilitiesContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Create button for each ability
            foreach (MagicType ability in playerCharacter.abilities.GetLearnedAbilities())
            {
                if (magicButtonPrefab != null)
                {
                    GameObject abilityButton = Instantiate(magicButtonPrefab, magicAbilitiesContainer.transform);
                    
                    Text buttonText = abilityButton.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = ability.ToString();
                    }

                    Button button = abilityButton.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => OnMagicAbilitySelected(ability));
                    }
                }
            }
        }

        /// <summary>
        /// Handle magic ability selection
        /// </summary>
        private void OnMagicAbilitySelected(MagicType ability)
        {
            AddCombatLogEntry($"Select a target for {ability}...");
            Debug.Log($"Magic ability selected: {ability}");
            
            // Hide magic panel
            if (magicPanel != null)
            {
                magicPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Handle defend button click
        /// </summary>
        private void OnDefendClicked()
        {
            if (currentEncounter != null)
            {
                currentEncounter.PlayerDefend();
                AddCombatLogEntry("You take a defensive stance!");
                
                ProcessEnemyTurn();
            }

            Debug.Log("Defend action selected");
        }

        /// <summary>
        /// Handle item button click
        /// </summary>
        private void OnItemClicked()
        {
            AddCombatLogEntry("Opening inventory...");
            
            // Show inventory UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPanel("Inventory");
            }

            Debug.Log("Item action selected");
        }

        /// <summary>
        /// Handle flee button click
        /// </summary>
        private void OnFleeClicked()
        {
            if (currentEncounter != null)
            {
                bool fled = currentEncounter.PlayerFlee();
                
                if (fled)
                {
                    AddCombatLogEntry("You successfully fled from combat!");
                    EndCombat(false, false);
                }
                else
                {
                    AddCombatLogEntry("Failed to flee!");
                    ProcessEnemyTurn();
                }
            }

            Debug.Log("Flee action selected");
        }

        /// <summary>
        /// Process enemy turn(s)
        /// </summary>
        private void ProcessEnemyTurn()
        {
            if (currentEncounter == null)
                return;

            // Check if combat should end
            if (currentEncounter.CheckVictory())
            {
                EndCombat(true, false);
                return;
            }

            if (currentEncounter.CheckDefeat())
            {
                EndCombat(false, true);
                return;
            }

            // Enemy attacks would be processed here
            AddCombatLogEntry("Enemy turn...");
            UpdatePlayerDisplay();
            UpdateEnemyDisplays();
        }

        /// <summary>
        /// End combat encounter
        /// </summary>
        public void EndCombat(bool victory, bool defeat)
        {
            if (victory)
            {
                ShowVictoryScreen();
            }
            else if (defeat)
            {
                ShowDefeatScreen();
            }
            else
            {
                // Fled
                if (combatPanel != null)
                {
                    combatPanel.SetActive(false);
                }

                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowPanel("HUD");
                }
            }

            Debug.Log($"Combat ended - Victory: {victory}, Defeat: {defeat}");
        }

        /// <summary>
        /// Show victory screen with rewards
        /// </summary>
        private void ShowVictoryScreen()
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }

            if (victoryRewardsText != null && currentEncounter != null)
            {
                // Display rewards
                string rewards = "Victory!\n\nRewards:\n";
                // TODO: Get actual rewards from encounter
                rewards += "Experience: ???\n";
                rewards += "Gold: ???\n";
                
                victoryRewardsText.text = rewards;
            }

            AddCombatLogEntry("VICTORY!");
        }

        /// <summary>
        /// Show defeat screen
        /// </summary>
        private void ShowDefeatScreen()
        {
            if (defeatPanel != null)
            {
                defeatPanel.SetActive(true);
            }

            AddCombatLogEntry("DEFEAT!");
        }

        /// <summary>
        /// Continue after victory
        /// </summary>
        public void OnContinueAfterVictory()
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }

            if (combatPanel != null)
            {
                combatPanel.SetActive(false);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPanel("HUD");
            }
        }

        /// <summary>
        /// Handle defeat - return to main menu or load
        /// </summary>
        public void OnDefeat()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ReturnToMainMenu();
            }
        }
    }
}
