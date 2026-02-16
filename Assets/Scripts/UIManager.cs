using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Central UI management system for ACOTAR RPG
    /// Manages all UI panels, screens, and user interactions
    /// 
    /// v2.5.3: Enhanced with property accessors and defensive programming
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        public GameObject mainMenuPanel;
        public GameObject hudPanel;
        public GameObject inventoryPanel;
        public GameObject questLogPanel;
        public GameObject dialoguePanel;
        public GameObject combatPanel;
        public GameObject pauseMenuPanel;
        public GameObject characterCreationPanel;

        [Header("HUD Elements")]
        public Slider healthBar;
        public Slider magicBar;
        public Text levelText;
        public Text xpText;
        public Text locationText;
        public Text goldText;
        public GameObject companionsDisplay;

        [Header("Inventory UI")]
        public GameObject inventoryGrid;
        public GameObject itemSlotPrefab;
        public Text inventoryGoldText;
        public GameObject equipmentPanel;

        [Header("Quest Log UI")]
        public GameObject questListContainer;
        public GameObject questDetailPanel;
        public Text questTitleText;
        public Text questDescriptionText;
        public GameObject questObjectivesPrefab;

        [Header("Dialogue UI")]
        public Text dialogueSpeakerText;
        public Text dialogueContentText;
        public GameObject dialogueChoicesContainer;
        public GameObject dialogueChoiceButtonPrefab;

        [Header("Combat UI")]
        public Text combatLogText;
        public GameObject enemyHealthBarPrefab;
        public GameObject actionButtonsPanel;
        public Text turnIndicatorText;

        [Header("Notification System")]
        public GameObject notificationPanel;
        public Text notificationText;
        private Queue<string> notificationQueue = new Queue<string>();
        private bool isShowingNotification = false;

        private Dictionary<string, GameObject> activePanels;
        private bool isGamePaused;

        // Public property accessors for cleaner code (v2.5.3)
        /// <summary>
        /// Check if the game is currently paused
        /// </summary>
        public bool IsPaused => isGamePaused;

        /// <summary>
        /// Check if a notification is currently being shown
        /// </summary>
        public bool IsShowingNotification => isShowingNotification;

        /// <summary>
        /// Get the number of queued notifications
        /// </summary>
        public int NotificationQueueCount => notificationQueue?.Count ?? 0;

        /// <summary>
        /// Check if the UI system is properly initialized
        /// </summary>
        public bool IsInitialized => activePanels != null && hudPanel != null;

        void Awake()
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

            InitializeUI();
        }

        /// <summary>
        /// Initialize UI system and hide all panels except main menu
        /// </summary>
        private void InitializeUI()
        {
            activePanels = new Dictionary<string, GameObject>();
            
            // Register all panels
            if (mainMenuPanel != null) activePanels["MainMenu"] = mainMenuPanel;
            if (hudPanel != null) activePanels["HUD"] = hudPanel;
            if (inventoryPanel != null) activePanels["Inventory"] = inventoryPanel;
            if (questLogPanel != null) activePanels["QuestLog"] = questLogPanel;
            if (dialoguePanel != null) activePanels["Dialogue"] = dialoguePanel;
            if (combatPanel != null) activePanels["Combat"] = combatPanel;
            if (pauseMenuPanel != null) activePanels["PauseMenu"] = pauseMenuPanel;
            if (characterCreationPanel != null) activePanels["CharacterCreation"] = characterCreationPanel;

            // Hide all panels initially
            HideAllPanels();
            
            // Show main menu at start
            ShowPanel("MainMenu");

            Debug.Log("UIManager initialized with " + activePanels.Count + " panels");
        }

        /// <summary>
        /// Show a specific UI panel
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void ShowPanel(string panelName)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("UIManager: Cannot show panel - system not initialized");
                return;
            }

            if (string.IsNullOrEmpty(panelName))
            {
                Debug.LogWarning("UIManager: Cannot show panel with null or empty name");
                return;
            }

            if (activePanels.ContainsKey(panelName) && activePanels[panelName] != null)
            {
                activePanels[panelName].SetActive(true);
                Debug.Log($"Showing panel: {panelName}");
            }
            else
            {
                Debug.LogWarning($"Panel not found: {panelName}");
            }
        }

        /// <summary>
        /// Hide a specific UI panel
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void HidePanel(string panelName)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("UIManager: Cannot hide panel - system not initialized");
                return;
            }

            if (string.IsNullOrEmpty(panelName))
            {
                Debug.LogWarning("UIManager: Cannot hide panel with null or empty name");
                return;
            }

            if (activePanels.ContainsKey(panelName) && activePanels[panelName] != null)
            {
                activePanels[panelName].SetActive(false);
                Debug.Log($"Hiding panel: {panelName}");
            }
        }

        /// <summary>
        /// Toggle a UI panel's visibility
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void TogglePanel(string panelName)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("UIManager: Cannot toggle panel - system not initialized");
                return;
            }

            if (string.IsNullOrEmpty(panelName))
            {
                Debug.LogWarning("UIManager: Cannot toggle panel with null or empty name");
                return;
            }

            if (activePanels.ContainsKey(panelName) && activePanels[panelName] != null)
            {
                bool isActive = activePanels[panelName].activeSelf;
                activePanels[panelName].SetActive(!isActive);
                Debug.Log($"Toggling panel {panelName}: {!isActive}");
            }
            else
            {
                Debug.LogWarning($"Panel not found: {panelName}");
            }
        }

        /// <summary>
        /// Hide all UI panels
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void HideAllPanels()
        {
            // Defensive check (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("UIManager: Cannot hide all panels - system not initialized");
                return;
            }

            foreach (var panel in activePanels.Values)
            {
                if (panel != null)
                {
                    panel.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Update HUD with character information
        /// v2.5.3: Enhanced with defensive checks
        /// </summary>
        public void UpdateHUD(Character character)
        {
            // Defensive checks (v2.5.3)
            if (!IsInitialized)
            {
                Debug.LogWarning("UIManager: Cannot update HUD - system not initialized");
                return;
            }

            if (character == null)
            {
                Debug.LogWarning("UIManager: Cannot update HUD with null character");
                return;
            }

            if (character.stats == null)
            {
                Debug.LogWarning("UIManager: Cannot update HUD - character stats are null");
                return;
            }

            // Update health bar
            if (healthBar != null)
            {
                healthBar.maxValue = character.stats.MaxHealth;
                healthBar.value = character.stats.CurrentHealth;
            }

            // Update magic bar
            if (magicBar != null)
            {
                magicBar.maxValue = character.stats.MaxMagicPower;
                magicBar.value = character.magicPower;
            }

            // Update level and XP
            if (levelText != null)
            {
                levelText.text = $"Level: {character.stats.Level}";
            }

            if (xpText != null)
            {
                int nextLevelXP = character.stats.GetXPRequiredForNextLevel();
                xpText.text = $"XP: {character.stats.Experience} / {nextLevelXP}";
            }

            // Update location
            if (locationText != null && GameManager.Instance != null)
            {
                locationText.text = $"Location: {GameManager.Instance.currentLocation}";
            }
        }

        /// <summary>
        /// Update gold display
        /// </summary>
        public void UpdateGoldDisplay(int gold)
        {
            if (goldText != null)
            {
                goldText.text = $"Gold: {gold}";
            }
            if (inventoryGoldText != null)
            {
                inventoryGoldText.text = $"Gold: {gold}";
            }
        }

        /// <summary>
        /// Show dialogue with speaker and text
        /// </summary>
        public void ShowDialogue(string speaker, string text)
        {
            ShowPanel("Dialogue");
            
            if (dialogueSpeakerText != null)
            {
                dialogueSpeakerText.text = speaker;
            }

            if (dialogueContentText != null)
            {
                dialogueContentText.text = text;
            }
        }

        /// <summary>
        /// Clear dialogue choices
        /// </summary>
        public void ClearDialogueChoices()
        {
            if (dialogueChoicesContainer != null)
            {
                foreach (Transform child in dialogueChoicesContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        /// <summary>
        /// Add a dialogue choice button
        /// </summary>
        public void AddDialogueChoice(string choiceText, System.Action onChoiceSelected)
        {
            if (dialogueChoicesContainer != null && dialogueChoiceButtonPrefab != null)
            {
                GameObject choiceButton = Instantiate(dialogueChoiceButtonPrefab, dialogueChoicesContainer.transform);
                Text buttonText = choiceButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = choiceText;
                }

                Button button = choiceButton.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => onChoiceSelected?.Invoke());
                }
            }
        }

        /// <summary>
        /// Update combat log with optimized string handling
        /// </summary>
        public void UpdateCombatLog(string message)
        {
            if (combatLogText != null)
            {
                System.Text.StringBuilder logBuilder = new System.Text.StringBuilder(combatLogText.text);
                logBuilder.AppendLine(message);
                
                // Limit log size to last 10 lines for performance
                string fullLog = logBuilder.ToString();
                string[] lines = fullLog.Split('\n');
                if (lines.Length > 10)
                {
                    logBuilder.Clear();
                    for (int i = lines.Length - 10; i < lines.Length; i++)
                    {
                        logBuilder.Append(lines[i]);
                        if (i < lines.Length - 1)
                            logBuilder.AppendLine();
                    }
                }
                
                combatLogText.text = logBuilder.ToString();
            }
        }

        /// <summary>
        /// Clear combat log
        /// </summary>
        public void ClearCombatLog()
        {
            if (combatLogText != null)
            {
                combatLogText.text = "";
            }
        }

        /// <summary>
        /// Update turn indicator
        /// </summary>
        public void UpdateTurnIndicator(string turn)
        {
            if (turnIndicatorText != null)
            {
                turnIndicatorText.text = $"Turn: {turn}";
            }
        }

        /// <summary>
        /// Toggle pause menu
        /// </summary>
        public void TogglePauseMenu()
        {
            isGamePaused = !isGamePaused;
            
            if (isGamePaused)
            {
                ShowPanel("PauseMenu");
                Time.timeScale = 0f; // Pause game
            }
            else
            {
                HidePanel("PauseMenu");
                Time.timeScale = 1f; // Resume game
            }
        }

        /// <summary>
        /// Start new game - show character creation
        /// </summary>
        public void StartNewGame()
        {
            HidePanel("MainMenu");
            ShowPanel("CharacterCreation");
        }

        /// <summary>
        /// Continue game - load and show HUD
        /// </summary>
        public void ContinueGame()
        {
            HidePanel("MainMenu");
            ShowPanel("HUD");
            
            // Load game through GameManager
            if (GameManager.Instance != null)
            {
                // Load game logic here
                Debug.Log("Continuing game...");
            }
        }

        /// <summary>
        /// Return to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            HideAllPanels();
            ShowPanel("MainMenu");
            Time.timeScale = 1f; // Ensure game is unpaused
        }

        /// <summary>
        /// Quit application
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting game...");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        /// <summary>
        /// Handle input for UI navigation
        /// </summary>
        void Update()
        {
            // Toggle inventory with 'I' key
            if (Input.GetKeyDown(KeyCode.I))
            {
                TogglePanel("Inventory");
            }

            // Toggle quest log with 'Q' key
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TogglePanel("QuestLog");
            }

            // Toggle pause menu with ESC
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
            }
        }

        /// <summary>
        /// Display a notification message with queue system
        /// </summary>
        public void ShowNotification(string message, float duration = 3f)
        {
            Debug.Log($"Notification: {message}");
            
            // Add notification to queue
            notificationQueue.Enqueue(message);
            
            // Start showing notifications if not already showing
            if (!isShowingNotification)
            {
                StartCoroutine(ShowNotificationCoroutine(duration));
            }
        }

        private System.Collections.IEnumerator ShowNotificationCoroutine(float duration)
        {
            isShowingNotification = true;
            
            while (notificationQueue.Count > 0)
            {
                string message = notificationQueue.Dequeue();
                
                // If notification panel exists, use it
                if (notificationPanel != null && notificationText != null)
                {
                    notificationText.text = message;
                    notificationPanel.SetActive(true);
                    
                    // Get or add CanvasGroup for fading
                    CanvasGroup canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = notificationPanel.AddComponent<CanvasGroup>();
                    }
                    
                    // Fade in
                    float fadeTime = 0.3f;
                    for (float t = 0; t < fadeTime; t += Time.deltaTime)
                    {
                        canvasGroup.alpha = t / fadeTime;
                        yield return null;
                    }
                    canvasGroup.alpha = 1f;
                    
                    // Wait for display duration
                    yield return new WaitForSeconds(duration);
                    
                    // Fade out
                    for (float t = 0; t < fadeTime; t += Time.deltaTime)
                    {
                        canvasGroup.alpha = 1f - (t / fadeTime);
                        yield return null;
                    }
                    canvasGroup.alpha = 0f;
                    
                    notificationPanel.SetActive(false);
                }
                else
                {
                    // Fallback: just log and wait
                    Debug.LogWarning("Notification panel not configured in UIManager");
                    yield return new WaitForSeconds(duration);
                }
            }
            
            isShowingNotification = false;
        }
    }
}
