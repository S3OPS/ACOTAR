using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ACOTAR
{
    /// <summary>
    /// Save/Load menu interface for managing game saves
    /// Provides 5 save slots with metadata display
    /// </summary>
    public class SaveLoadUI : MonoBehaviour
    {
        public static SaveLoadUI Instance { get; private set; }

        [Header("UI References")]
        public GameObject saveLoadPanel;
        public GameObject saveSlotPrefab;
        public Transform saveSlotContainer;
        public Button saveButton;
        public Button loadButton;
        public Button deleteButton;
        public Button backButton;
        
        [Header("Save Slot Details")]
        public Text slotNumberText;
        public Text characterNameText;
        public Text characterLevelText;
        public Text locationText;
        public Text playTimeText;
        public Text saveTimeText;
        public GameObject emptySlotText;
        
        private int selectedSlot = -1;
        private List<GameObject> slotButtons = new List<GameObject>();
        private const int MAX_SAVE_SLOTS = 5;
        private bool isInSaveMode = true; // true = Save, false = Load

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
            InitializeSaveSlots();
            HidePanel();
        }

        /// <summary>
        /// Initialize the save slot buttons
        /// </summary>
        private void InitializeSaveSlots()
        {
            // Clear existing slots
            foreach (Transform child in saveSlotContainer)
            {
                Destroy(child.gameObject);
            }
            slotButtons.Clear();

            // Create 5 save slots
            for (int i = 1; i <= MAX_SAVE_SLOTS; i++)
            {
                CreateSaveSlotButton(i);
            }

            // Setup button listeners
            if (saveButton != null) saveButton.onClick.AddListener(OnSaveButtonClicked);
            if (loadButton != null) loadButton.onClick.AddListener(OnLoadButtonClicked);
            if (deleteButton != null) deleteButton.onClick.AddListener(OnDeleteButtonClicked);
            if (backButton != null) backButton.onClick.AddListener(HidePanel);

            UpdateButtonStates();
        }

        /// <summary>
        /// Create a save slot button
        /// </summary>
        private void CreateSaveSlotButton(int slotNumber)
        {
            GameObject slotObj = Instantiate(saveSlotPrefab, saveSlotContainer);
            Button button = slotObj.GetComponent<Button>();
            
            if (button != null)
            {
                int slot = slotNumber; // Capture for lambda
                button.onClick.AddListener(() => OnSlotSelected(slot));
            }

            // Update slot display
            UpdateSlotDisplay(slotObj, slotNumber);
            
            slotButtons.Add(slotObj);
        }

        /// <summary>
        /// Update the display for a specific save slot
        /// </summary>
        private void UpdateSlotDisplay(GameObject slotObj, int slotNumber)
        {
            SaveData saveData = SaveSystem.LoadGame(slotNumber);
            
            Text slotText = slotObj.GetComponentInChildren<Text>();
            if (slotText != null)
            {
                if (saveData != null)
                {
                    // Show save info
                    slotText.text = $"Slot {slotNumber}\n" +
                                   $"{saveData.characterName} - Lvl {saveData.level}\n" +
                                   $"{saveData.currentLocation}\n" +
                                   $"Playtime: {FormatPlayTime(saveData.playTimeHours)}";
                }
                else
                {
                    // Empty slot
                    slotText.text = $"Slot {slotNumber}\n<Empty>";
                }
            }
        }

        /// <summary>
        /// Handle slot selection
        /// </summary>
        private void OnSlotSelected(int slotNumber)
        {
            selectedSlot = slotNumber;
            UpdateSlotDetails(slotNumber);
            UpdateButtonStates();
            
            // Highlight selected slot
            for (int i = 0; i < slotButtons.Count; i++)
            {
                Image img = slotButtons[i].GetComponent<Image>();
                if (img != null)
                {
                    img.color = (i + 1 == slotNumber) ? Color.yellow : Color.white;
                }
            }
        }

        /// <summary>
        /// Update the detailed info panel for selected slot
        /// </summary>
        private void UpdateSlotDetails(int slotNumber)
        {
            SaveData saveData = SaveSystem.LoadGame(slotNumber);
            
            if (saveData != null)
            {
                // Show save details
                if (slotNumberText != null) slotNumberText.text = $"Slot {slotNumber}";
                if (characterNameText != null) characterNameText.text = saveData.characterName;
                if (characterLevelText != null) characterLevelText.text = $"Level {saveData.level} {saveData.characterClass}";
                if (locationText != null) locationText.text = saveData.currentLocation;
                if (playTimeText != null) playTimeText.text = $"Playtime: {FormatPlayTime(saveData.playTimeHours)}";
                if (saveTimeText != null) saveTimeText.text = $"Saved: {saveData.saveTimestamp}";
                if (emptySlotText != null) emptySlotText.SetActive(false);
            }
            else
            {
                // Empty slot
                if (slotNumberText != null) slotNumberText.text = $"Slot {slotNumber}";
                if (characterNameText != null) characterNameText.text = "";
                if (characterLevelText != null) characterLevelText.text = "";
                if (locationText != null) locationText.text = "";
                if (playTimeText != null) playTimeText.text = "";
                if (saveTimeText != null) saveTimeText.text = "";
                if (emptySlotText != null) emptySlotText.SetActive(true);
            }
        }

        /// <summary>
        /// Update button enabled/disabled states
        /// </summary>
        private void UpdateButtonStates()
        {
            bool slotSelected = selectedSlot > 0;
            SaveData saveData = slotSelected ? SaveSystem.LoadGame(selectedSlot) : null;
            bool hasData = saveData != null;

            if (saveButton != null)
            {
                // Can save if slot selected and in save mode
                saveButton.interactable = slotSelected && isInSaveMode;
            }

            if (loadButton != null)
            {
                // Can load if slot has data and not in save mode
                loadButton.interactable = slotSelected && hasData && !isInSaveMode;
            }

            if (deleteButton != null)
            {
                // Can delete if slot has data
                deleteButton.interactable = slotSelected && hasData;
            }
        }

        /// <summary>
        /// Handle save button click
        /// </summary>
        private void OnSaveButtonClicked()
        {
            if (selectedSlot <= 0) return;

            if (GameManager.Instance != null)
            {
                bool success = SaveSystem.SaveGame(selectedSlot);
                if (success)
                {
                    Debug.Log($"Game saved to slot {selectedSlot}");
                    ShowNotification($"Game saved to slot {selectedSlot}");
                    
                    // Refresh display
                    RefreshAllSlots();
                    UpdateSlotDetails(selectedSlot);
                }
                else
                {
                    ShowNotification("Failed to save game", true);
                }
            }
        }

        /// <summary>
        /// Handle load button click
        /// </summary>
        private void OnLoadButtonClicked()
        {
            if (selectedSlot <= 0) return;

            if (GameManager.Instance != null)
            {
                bool success = SaveSystem.LoadGame(selectedSlot);
                if (success)
                {
                    Debug.Log($"Game loaded from slot {selectedSlot}");
                    ShowNotification($"Game loaded from slot {selectedSlot}");
                    HidePanel();
                    
                    // Return to HUD
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowHUD();
                    }
                }
                else
                {
                    ShowNotification("Failed to load game", true);
                }
            }
        }

        /// <summary>
        /// Handle delete button click
        /// </summary>
        private void OnDeleteButtonClicked()
        {
            if (selectedSlot <= 0) return;

            // Confirm deletion (in a real game, add a confirmation dialog)
            SaveSystem.DeleteSave(selectedSlot);
            Debug.Log($"Deleted save slot {selectedSlot}");
            ShowNotification($"Deleted save slot {selectedSlot}");
            
            // Refresh display
            RefreshAllSlots();
            selectedSlot = -1;
            UpdateButtonStates();
        }

        /// <summary>
        /// Refresh all save slot displays
        /// </summary>
        private void RefreshAllSlots()
        {
            for (int i = 0; i < slotButtons.Count; i++)
            {
                UpdateSlotDisplay(slotButtons[i], i + 1);
            }
        }

        /// <summary>
        /// Show the save/load panel in save mode
        /// </summary>
        public void ShowSavePanel()
        {
            isInSaveMode = true;
            ShowPanel();
            
            if (saveButton != null) saveButton.gameObject.SetActive(true);
            if (loadButton != null) loadButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Show the save/load panel in load mode
        /// </summary>
        public void ShowLoadPanel()
        {
            isInSaveMode = false;
            ShowPanel();
            
            if (saveButton != null) saveButton.gameObject.SetActive(false);
            if (loadButton != null) loadButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Show the panel
        /// </summary>
        private void ShowPanel()
        {
            if (saveLoadPanel != null)
            {
                saveLoadPanel.SetActive(true);
                RefreshAllSlots();
                selectedSlot = -1;
                UpdateButtonStates();
            }
        }

        /// <summary>
        /// Hide the panel
        /// </summary>
        public void HidePanel()
        {
            if (saveLoadPanel != null)
            {
                saveLoadPanel.SetActive(false);
                selectedSlot = -1;
            }
        }

        /// <summary>
        /// Format play time in hours and minutes
        /// </summary>
        private string FormatPlayTime(float hours)
        {
            int h = Mathf.FloorToInt(hours);
            int m = Mathf.FloorToInt((hours - h) * 60);
            return $"{h}h {m}m";
        }

        /// <summary>
        /// Show a temporary notification message
        /// </summary>
        private void ShowNotification(string message, bool isError = false)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification(message);
            }
            else
            {
                Debug.Log(message);
            }
        }
    }
}
