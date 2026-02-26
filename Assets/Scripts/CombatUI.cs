using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

        [Header("Quest Preparation Hint")]
        public GameObject questHintPanel;
        public Text questHintText;
        public Button dismissHintButton;

        [Header("Confirmation Dialog")]
        public GameObject confirmationPanel;
        public Text confirmationMessageText;
        public Button confirmYesButton;
        public Button confirmNoButton;

        [Header("Spell Queue Indicator")]
        public Text pendingSpellText; // v2.6.11: Shows currently queued magic ability

        private CombatEncounter currentEncounter;
        private List<GameObject> enemyPanels = new List<GameObject>();
        private Character playerCharacter;
        private System.Action pendingConfirmAction;
        private MagicType? pendingMagicAbility = null; // v2.6.10: Track selected magic ability for targeting
        private Coroutine spellFadeCoroutine = null; // v2.6.13: Track running fade so we can stop it before restarting
        private Coroutine spellShimmerCoroutine = null; // v2.6.16: Track legendary-ability shimmer so it can be cancelled

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

            if (questHintPanel != null)
            {
                questHintPanel.SetActive(false);
            }

            if (confirmationPanel != null)
            {
                confirmationPanel.SetActive(false);
            }

            // Setup confirmation dialog buttons
            if (dismissHintButton != null)
            {
                dismissHintButton.onClick.AddListener(DismissQuestHint);
            }

            if (confirmYesButton != null)
            {
                confirmYesButton.onClick.AddListener(OnConfirmYes);
            }

            if (confirmNoButton != null)
            {
                confirmNoButton.onClick.AddListener(OnConfirmNo);
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

            // v2.6.8: Show quest preparation hint if available
            ShowActiveQuestPreparationHint();

            // Initial log entry
            AddCombatLogEntry("Combat Started!");
            
            Debug.Log("Combat UI started");
        }

        /// <summary>
        /// Show preparation hint for the currently active quest with a hint defined
        /// v2.6.8: NEW - Displays combat tips from the quest's preparationHint field
        /// </summary>
        private void ShowActiveQuestPreparationHint()
        {
            if (questHintPanel == null || questHintText == null)
                return;

            // Find the first active quest that has a preparation hint
            if (GameManager.Instance == null)
                return;

            QuestManager questManager = GameManager.Instance.GetComponent<QuestManager>();
            if (questManager == null)
                return;

            string hint = null;
            foreach (Quest quest in questManager.GetActiveQuests())
            {
                if (!string.IsNullOrEmpty(quest.preparationHint))
                {
                    hint = quest.preparationHint;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(hint))
            {
                questHintText.text = hint;
                questHintPanel.SetActive(true);
                Debug.Log("CombatUI: Showing quest preparation hint");
            }
        }

        /// <summary>
        /// Dismiss the quest preparation hint panel
        /// v2.6.8: NEW
        /// </summary>
        public void DismissQuestHint()
        {
            if (questHintPanel != null)
            {
                questHintPanel.SetActive(false);
            }
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
                if (pendingMagicAbility.HasValue)
                {
                    // v2.6.10: Execute queued magic attack and check for cascade
                    MagicType ability = pendingMagicAbility.Value;
                    pendingMagicAbility = null;
                    UpdatePendingMagicIndicator(); // v2.6.11

                    currentEncounter.PlayerMagicAttack(enemy, ability);
                    AddCombatLogEntry($"You cast {ability} at {enemy.characterName}!");

                    // v2.6.10: Visual feedback for cascade combo on magic attacks
                    if (CombatSystem.WasLastAttackCascade())
                    {
                        ScreenEffectsManager.Instance?.AlertPulse();
                        AudioManager.Instance?.PlayUISFXByName("combo_cascade");
                    }
                }
                else
                {
                    currentEncounter.PlayerPhysicalAttack(enemy);
                    AddCombatLogEntry($"You attack {enemy.characterName}!");

                    // v2.6.9: Visual feedback for cascade combo
                    if (CombatSystem.WasLastAttackCascade())
                    {
                        ScreenEffectsManager.Instance?.AlertPulse();
                        AudioManager.Instance?.PlayUISFXByName("combo_cascade");
                    }
                }

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

                if (magicPanel.activeSelf)
                {
                    DisplayMagicAbilities();
                }
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
            foreach (MagicType ability in playerCharacter.abilitySystem.GetLearnedAbilities())
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

            AudioManager.Instance?.PlayUISFXByName("ability_select"); // v2.6.13: audio feedback on spell select

            // v2.6.10: Store the ability so OnEnemyTargeted can execute it on the chosen target
            pendingMagicAbility = ability;
            UpdatePendingMagicIndicator(); // v2.6.11

            // Hide magic panel
            if (magicPanel != null)
            {
                magicPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Update the HUD indicator that shows which magic ability is currently queued.
        /// v2.6.11: Gives players clear visual confirmation of their spell selection.
        /// v2.6.12: Adds per-magic-type colour coding and a fade-in animation.
        /// v2.6.16: Plays shimmer flash and "spell_legendary" audio for legendary abilities.
        /// </summary>
        private void UpdatePendingMagicIndicator()
        {
            if (pendingSpellText == null) return;
            if (pendingMagicAbility.HasValue)
            {
                pendingSpellText.text = $"⚡ Spell Queued: {pendingMagicAbility.Value}";
                pendingSpellText.color = GetSpellColor(pendingMagicAbility.Value); // v2.6.12
                if (spellFadeCoroutine != null) StopCoroutine(spellFadeCoroutine); // v2.6.13: cancel stale fade
                spellFadeCoroutine = StartCoroutine(FadeInPendingSpellText()); // v2.6.12

                // v2.6.16: Extra shimmer flash for legendary abilities
                if (IsLegendaryAbility(pendingMagicAbility.Value))
                {
                    if (spellShimmerCoroutine != null) StopCoroutine(spellShimmerCoroutine);
                    spellShimmerCoroutine = StartCoroutine(ShimmerPendingSpellText(GetSpellColor(pendingMagicAbility.Value)));
                    AudioManager.Instance?.PlayUISFXByName("spell_legendary"); // v2.6.16: audio feedback for legendary spell queue
                }
            }
            else
            {
                if (spellFadeCoroutine != null) { StopCoroutine(spellFadeCoroutine); spellFadeCoroutine = null; } // v2.6.14: cancel fade when clearing
                if (spellShimmerCoroutine != null) { StopCoroutine(spellShimmerCoroutine); spellShimmerCoroutine = null; } // v2.6.16: cancel shimmer when clearing
                AudioManager.Instance?.PlayUISFXByName("spell_clear"); // v2.6.15: audio feedback when a queued spell is cancelled
                pendingSpellText.text = string.Empty;
                pendingSpellText.color = Color.white; // reset colour for next spell
                if (pendingSpellText.rectTransform != null) pendingSpellText.rectTransform.localScale = Vector3.one; // v2.6.14: reset scale
            }
        }

        /// <summary>
        /// Map magic type to a thematic colour for the spell-queue HUD indicator.
        /// v2.6.12: NEW — called by UpdatePendingMagicIndicator().
        /// </summary>
        private static Color GetSpellColor(MagicType ability)
        {
            switch (ability)
            {
                case MagicType.FireManipulation:   return new Color(1f,    0.35f, 0.1f);  // Vivid orange-red
                case MagicType.IceManipulation:    return new Color(0.5f,  0.85f, 1f);    // Icy blue
                case MagicType.WaterManipulation:  return new Color(0.2f,  0.55f, 1f);    // Deep water blue
                case MagicType.WindManipulation:   return new Color(0.8f,  1f,    0.8f);  // Soft green-white
                case MagicType.EarthManipulation:  return new Color(0.6f,  0.4f,  0.2f);  // Earthy brown
                case MagicType.DarknessManipulation: return new Color(0.55f, 0.1f, 0.9f); // Deep violet
                case MagicType.LightManipulation:  return new Color(1f,    1f,    0.5f);  // Bright gold
                case MagicType.Healing:            return new Color(0.2f,  1f,    0.5f);  // Healing green
                case MagicType.ShieldCreation:     return new Color(0.4f,  0.7f,  1f);    // Shield blue
                case MagicType.Daemati:            return new Color(1f,    0.2f,  0.8f);  // Mind magic magenta
                case MagicType.DeathManifestation: return new Color(0.7f,  0f,    0.2f);  // Death crimson
                case MagicType.Shadowsinger:       return new Color(0.3f,  0.2f,  0.5f);  // Shadow indigo
                case MagicType.TruthTelling:       return new Color(1f,    0.9f,  0.3f);  // Truth gold
                case MagicType.SpellCleaving:      return new Color(0.9f,  0.5f,  1f);    // Arcane purple
                case MagicType.Winnowing:          return new Color(0.5f,  0.3f,  0.9f);  // v2.6.13: Portal blue-violet
                case MagicType.Seer:               return new Color(0.85f, 0.9f,  1f);    // v2.6.13: Prophetic silver
                case MagicType.MatingBond:         return new Color(1f,    0.6f,  0.7f);  // v2.6.13: Mate rose-gold
                case MagicType.Shapeshifting:      return new Color(1f,    0.65f, 0.1f);  // v2.6.14: Shifting amber
                default:                           return Color.white;
            }
        }

        /// <summary>
        /// Returns true if the given ability is considered legendary/rare in ACOTAR lore.
        /// v2.6.16: Used to decide whether to play the shimmer flash animation and "spell_legendary" audio cue.
        /// Legendary abilities are innate gifts, Cauldron-forged powers, or unique bonds
        /// that set their bearers apart from ordinary Fae magic users.
        /// </summary>
        private static bool IsLegendaryAbility(MagicType ability)
        {
            switch (ability)
            {
                case MagicType.Daemati:            // Rare mind magic of the Night Court inner circle
                case MagicType.DeathManifestation: // Nesta's Cauldron-forged power
                case MagicType.Shadowsinger:       // Azriel's unique gift, not teachable
                case MagicType.TruthTelling:       // Mor's innate compulsion
                case MagicType.SpellCleaving:      // Rarest offensive magic
                case MagicType.MatingBond:         // The bond itself as a wielded force
                case MagicType.Seer:               // Prophetic visions — few are born with them
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Briefly flashes the pendingSpellText between white and its spell colour twice to signal
        /// that a legendary ability has been queued. Runs alongside FadeInPendingSpellText.
        /// v2.6.16: NEW — called by UpdatePendingMagicIndicator when IsLegendaryAbility returns true.
        /// v2.6.17: Shimmer timing moved to GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION / SPELL_SHIMMER_FLASH_COUNT.
        /// </summary>
        private IEnumerator ShimmerPendingSpellText(Color spellColor)
        {
            if (pendingSpellText == null) yield break;

            for (int i = 0; i < GameConfig.UISettings.SPELL_SHIMMER_FLASH_COUNT; i++)
            {
                // Flash to white
                float elapsed = 0f;
                while (elapsed < GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION);
                    pendingSpellText.color = Color.Lerp(spellColor, Color.white, t);
                    yield return null;
                }
                // Flash back to spell colour
                elapsed = 0f;
                while (elapsed < GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION);
                    pendingSpellText.color = Color.Lerp(Color.white, spellColor, t);
                    yield return null;
                }
            }

            pendingSpellText.color = spellColor; // ensure we end on the exact spell colour
            spellShimmerCoroutine = null;
        }


        /// v2.6.12: NEW — draws the player's eye when a spell is queued.
        /// v2.6.14: Also animates a scale-up "punch" from SPELL_SCALE_START to 1 for extra visual impact.
        /// </summary>
        private IEnumerator FadeInPendingSpellText()
        {
            if (pendingSpellText == null) yield break;

            Color targetColor = pendingSpellText.color;
            pendingSpellText.color = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);

            // v2.6.14: Initialise scale to the punch-start size
            RectTransform rt = pendingSpellText.rectTransform;
            if (rt != null) rt.localScale = new Vector3(GameConfig.UISettings.SPELL_SCALE_START, GameConfig.UISettings.SPELL_SCALE_START, 1f);

            float elapsed = 0f;
            while (elapsed < GameConfig.UISettings.SPELL_FADE_IN_DURATION)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / GameConfig.UISettings.SPELL_FADE_IN_DURATION);
                pendingSpellText.color = new Color(targetColor.r, targetColor.g, targetColor.b, t);

                // v2.6.14: Scale from SPELL_SCALE_START to 1 using smooth-step easing
                float scale = Mathf.Lerp(GameConfig.UISettings.SPELL_SCALE_START, 1f, Mathf.SmoothStep(0f, 1f, t));
                if (rt != null) rt.localScale = new Vector3(scale, scale, 1f);

                yield return null;
            }

            pendingSpellText.color = targetColor; // ensure final alpha is exactly 1
            if (rt != null) rt.localScale = Vector3.one; // v2.6.14: ensure final scale is exactly 1
            spellFadeCoroutine = null; // v2.6.14: clear handle when complete
        }

        /// <summary>
        /// Handle defend button click
        /// </summary>
        private void OnDefendClicked()
        {
            pendingMagicAbility = null; // v2.6.10: Cancel any pending magic selection
            UpdatePendingMagicIndicator(); // v2.6.11
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
        /// Handle flee button click - shows confirmation dialog first
        /// v2.6.8: Enhanced with confirmation dialog
        /// </summary>
        private void OnFleeClicked()
        {
            ShowConfirmation("Flee from combat? You may lose progress!", ExecuteFlee);
            Debug.Log("Flee action selected - awaiting confirmation");
        }

        /// <summary>
        /// Execute the flee action after confirmation
        /// v2.6.8: NEW
        /// </summary>
        private void ExecuteFlee()
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

            Debug.Log("Flee executed");
        }

        /// <summary>
        /// Show a confirmation dialog with a message and callback for yes/no
        /// v2.6.8: NEW - Confirmation dialog for critical combat actions
        /// </summary>
        /// <param name="message">Message to display in the dialog</param>
        /// <param name="onConfirm">Action to execute if player confirms</param>
        public void ShowConfirmation(string message, System.Action onConfirm)
        {
            if (confirmationPanel == null)
            {
                // No confirmation panel configured - execute directly
                onConfirm?.Invoke();
                return;
            }

            pendingConfirmAction = onConfirm;

            if (confirmationMessageText != null)
            {
                confirmationMessageText.text = message;
            }

            confirmationPanel.SetActive(true);
            AudioManager.Instance?.PlayUISFXByName("confirm_open");
            Debug.Log($"CombatUI: Showing confirmation dialog - {message}");
        }

        /// <summary>
        /// Handle confirmation dialog Yes button
        /// v2.6.8: NEW
        /// </summary>
        private void OnConfirmYes()
        {
            if (confirmationPanel != null)
            {
                confirmationPanel.SetActive(false);
            }

            AudioManager.Instance?.PlayUISFXByName("confirm_yes");
            System.Action action = pendingConfirmAction;
            pendingConfirmAction = null;
            action?.Invoke();
        }

        /// <summary>
        /// Handle confirmation dialog No button
        /// v2.6.8: NEW
        /// </summary>
        private void OnConfirmNo()
        {
            if (confirmationPanel != null)
            {
                confirmationPanel.SetActive(false);
            }

            AudioManager.Instance?.PlayUISFXByName("confirm_no");
            pendingConfirmAction = null;
            AddCombatLogEntry("Action cancelled.");
            Debug.Log("CombatUI: Confirmation cancelled");
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
                // Display actual rewards from combat encounter
                string rewards = "Victory!\n\nRewards:\n";
                rewards += $"Experience: {currentEncounter.totalExperienceReward} XP\n";
                rewards += $"Gold: {currentEncounter.totalGoldReward}\n";
                
                if (currentEncounter.totalLootDrops.Count > 0)
                {
                    rewards += $"\nLoot:\n";
                    foreach (string loot in currentEncounter.totalLootDrops)
                    {
                        rewards += $"- {loot}\n";
                    }
                }
                
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
