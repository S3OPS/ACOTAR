using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Event system for game state changes
    /// Enables loose coupling between game systems
    /// </summary>
    public static class GameEvents
    {
        // Character Events
        public static event Action<Character> OnCharacterCreated;
        public static event Action<Character, int> OnCharacterTakeDamage;
        public static event Action<Character, int> OnCharacterHealed;
        public static event Action<Character, int> OnCharacterLevelUp;
        public static event Action<Character, CharacterClass> OnCharacterTransformed;
        public static event Action<Character, MagicType> OnAbilityLearned;

        // Location Events
        public static event Action<string, string> OnLocationChanged;
        
        // Court Events
        public static event Action<Character, Court> OnCourtAllegianceChanged;
        
        // Quest Events
        public static event Action<Quest> OnQuestStarted;
        public static event Action<Quest> OnQuestCompleted;

        // Combat Events
        public static event Action<Character, List<Enemy>> OnCombatStarted;
        public static event Action<Character, List<Enemy>, bool> OnCombatEnded;

        // Commerce Events
        public static event Action<string, int, string> OnItemPurchased; // itemId, price, merchantName
        public static event Action<string, int, string> OnItemSold; // itemId, price, merchantName

        // Equipment Events (v2.3.3: NEW)
        public static event Action OnEquipmentChanged;

        // Accessibility Events (Phase 8: NEW)
        public static event Action OnAccessibilityChanged;

        // Difficulty Events (v2.5.0: NEW)
        public static event Action OnDifficultyChanged;
        
        // Game State Events (v2.5.0: NEW)
        public static event Action OnGameOver;

        // Companion Events
        public static event Action<string> OnCompanionRecruited; // companionName
        public static event Action<string, string> OnLocationDiscovered; // locationName, courtName

        // Trigger character created event
        public static void TriggerCharacterCreated(Character character)
        {
            OnCharacterCreated?.Invoke(character);
        }

        // Trigger damage event
        public static void TriggerCharacterTakeDamage(Character character, int damage)
        {
            OnCharacterTakeDamage?.Invoke(character, damage);
        }

        // Trigger heal event
        public static void TriggerCharacterHealed(Character character, int amount)
        {
            OnCharacterHealed?.Invoke(character, amount);
        }

        // Trigger level up event
        public static void TriggerCharacterLevelUp(Character character, int newLevel)
        {
            OnCharacterLevelUp?.Invoke(character, newLevel);
        }

        // Trigger transformation event
        public static void TriggerCharacterTransformed(Character character, CharacterClass newClass)
        {
            OnCharacterTransformed?.Invoke(character, newClass);
        }

        // Trigger ability learned event
        public static void TriggerAbilityLearned(Character character, MagicType ability)
        {
            OnAbilityLearned?.Invoke(character, ability);
        }

        // Trigger location changed event
        public static void TriggerLocationChanged(string fromLocation, string toLocation)
        {
            OnLocationChanged?.Invoke(fromLocation, toLocation);
        }

        // Trigger court allegiance changed event
        public static void TriggerCourtAllegianceChanged(Character character, Court newCourt)
        {
            OnCourtAllegianceChanged?.Invoke(character, newCourt);
        }

        // Trigger quest started event
        public static void TriggerQuestStarted(Quest quest)
        {
            OnQuestStarted?.Invoke(quest);
        }

        // Trigger quest completed event
        public static void TriggerQuestCompleted(Quest quest)
        {
            OnQuestCompleted?.Invoke(quest);
        }

        // Trigger combat started event
        public static void TriggerCombatStarted(Character player, List<Enemy> enemies)
        {
            OnCombatStarted?.Invoke(player, enemies);
        }

        // Trigger combat ended event
        public static void TriggerCombatEnded(Character player, List<Enemy> enemies, bool victory)
        {
            OnCombatEnded?.Invoke(player, enemies, victory);
        }

        // Trigger item purchased event
        public static void TriggerItemPurchased(string itemId, int price, string merchantName)
        {
            OnItemPurchased?.Invoke(itemId, price, merchantName);
        }

        // Trigger item sold event
        public static void TriggerItemSold(string itemId, int price, string merchantName)
        {
            OnItemSold?.Invoke(itemId, price, merchantName);
        }

        // Trigger companion recruited event
        public static void TriggerCompanionRecruited(string companionName)
        {
            OnCompanionRecruited?.Invoke(companionName);
        }

        // Trigger location discovered event  
        public static void TriggerLocationDiscovered(string locationName, string courtName)
        {
            OnLocationDiscovered?.Invoke(locationName, courtName);
        }

        // Trigger accessibility changed event (Phase 8: NEW)
        public static void TriggerAccessibilityChanged()
        {
            OnAccessibilityChanged?.Invoke();
        }

        // Trigger difficulty changed event (v2.5.0: NEW)
        public static void TriggerDifficultyChanged()
        {
            OnDifficultyChanged?.Invoke();
        }

        // Trigger game over event (v2.5.0: NEW)
        public static void TriggerGameOver()
        {
            OnGameOver?.Invoke();
        }

        // Keybinding Events (v2.5.1: NEW)
        public static event Action<KeybindingSystem.GameAction> OnKeybindingAction;
        
        // Trigger keybinding action event (v2.5.1: NEW)
        public static void TriggerKeybindingAction(KeybindingSystem.GameAction action)
        {
            OnKeybindingAction?.Invoke(action);
        }
    }
}
