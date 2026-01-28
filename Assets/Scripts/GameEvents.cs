using System;
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
    }
}
