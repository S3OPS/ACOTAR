using System;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Serializable save data structure for game state persistence
    /// </summary>
    [Serializable]
    public class SaveData
    {
        // Character Data
        public string characterName;
        public CharacterClass characterClass;
        public Court allegiance;
        public bool isFae;
        public bool isMadeByTheCauldron;
        
        // Stats
        public int health;
        public int maxHealth;
        public int magicPower;
        public int strength;
        public int agility;
        public int experience;
        public int level;
        
        // Abilities
        public MagicType[] abilities;
        
        // Game State
        public string currentLocation;
        public int gameTime;
        public bool hasMetRhysand;
        public bool hasCompletedCurse;
        
        // Quests
        public string[] completedQuestIds;
        public string[] activeQuestIds;
    }

    /// <summary>
    /// Manages game state persistence (save/load functionality)
    /// Uses JSON serialization for cross-platform compatibility
    /// </summary>
    public static class SaveSystem
    {
        private const string SAVE_FILE_NAME = "acotar_save.json";

        /// <summary>
        /// Save current game state to persistent storage
        /// </summary>
        public static bool SaveGame()
        {
            try
            {
                if (GameManager.Instance == null)
                {
                    Debug.LogError("Cannot save: GameManager not initialized");
                    return false;
                }

                SaveData saveData = CreateSaveData();
                string json = JsonUtility.ToJson(saveData, true);
                string savePath = GetSavePath();
                
                System.IO.File.WriteAllText(savePath, json);
                Debug.Log($"Game saved successfully to: {savePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load game state from persistent storage
        /// </summary>
        public static bool LoadGame()
        {
            try
            {
                string savePath = GetSavePath();
                
                if (!System.IO.File.Exists(savePath))
                {
                    Debug.LogWarning("No save file found");
                    return false;
                }

                string json = System.IO.File.ReadAllText(savePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                ApplySaveData(saveData);
                Debug.Log("Game loaded successfully");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if a save file exists
        /// </summary>
        public static bool SaveExists()
        {
            return System.IO.File.Exists(GetSavePath());
        }

        /// <summary>
        /// Delete existing save file
        /// </summary>
        public static bool DeleteSave()
        {
            try
            {
                string savePath = GetSavePath();
                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                    Debug.Log("Save file deleted");
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete save: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Create save data from current game state
        /// </summary>
        private static SaveData CreateSaveData()
        {
            GameManager gm = GameManager.Instance;
            SaveData data = new SaveData();
            
            // Character data
            data.characterName = gm.playerCharacter.name;
            data.characterClass = gm.playerCharacter.characterClass;
            data.allegiance = gm.playerCharacter.allegiance;
            data.isFae = gm.playerCharacter.isFae;
            data.isMadeByTheCauldron = gm.playerCharacter.isMadeByTheCauldron;
            
            // Stats
            data.health = gm.playerCharacter.health;
            data.maxHealth = gm.playerCharacter.maxHealth;
            data.magicPower = gm.playerCharacter.magicPower;
            data.strength = gm.playerCharacter.strength;
            data.agility = gm.playerCharacter.agility;
            data.experience = gm.playerCharacter.experience;
            data.level = gm.playerCharacter.level;
            
            // Abilities
            data.abilities = gm.playerCharacter.abilities.ToArray();
            
            // Game state
            data.currentLocation = gm.currentLocation;
            data.gameTime = gm.gameTime;
            data.hasMetRhysand = gm.hasMetRhysand;
            data.hasCompletedCurse = gm.hasCompletedCurse;
            
            // TODO: Add quest data when QuestManager is accessible
            
            return data;
        }

        /// <summary>
        /// Apply loaded save data to game state
        /// </summary>
        private static void ApplySaveData(SaveData data)
        {
            GameManager gm = GameManager.Instance;
            
            // Recreate character with saved data
            gm.playerCharacter = new Character(data.characterName, data.characterClass, data.allegiance);
            gm.playerCharacter.isFae = data.isFae;
            gm.playerCharacter.isMadeByTheCauldron = data.isMadeByTheCauldron;
            
            // Restore stats
            gm.playerCharacter.health = data.health;
            gm.playerCharacter.maxHealth = data.maxHealth;
            gm.playerCharacter.magicPower = data.magicPower;
            gm.playerCharacter.strength = data.strength;
            gm.playerCharacter.agility = data.agility;
            gm.playerCharacter.experience = data.experience;
            gm.playerCharacter.level = data.level;
            
            // Restore abilities
            foreach (var ability in data.abilities)
            {
                gm.playerCharacter.LearnAbility(ability);
            }
            
            // Restore game state
            gm.currentLocation = data.currentLocation;
            gm.gameTime = data.gameTime;
            gm.hasMetRhysand = data.hasMetRhysand;
            gm.hasCompletedCurse = data.hasCompletedCurse;
        }

        /// <summary>
        /// Get platform-specific save file path
        /// </summary>
        private static string GetSavePath()
        {
            return System.IO.Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        }
    }
}
