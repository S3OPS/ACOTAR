using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Monitors game events and automatically awards appropriate titles
    /// Integrates character progression with story events and achievements
    /// </summary>
    public class TitleAwardManager : MonoBehaviour
    {
        public static TitleAwardManager Instance { get; private set; }

        private Character player;
        private CharacterProgression progression;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            SubscribeToEvents();
        }

        void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        /// <summary>
        /// Subscribe to relevant game events
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnCharacterCreated += HandleCharacterCreated;
            GameEvents.OnQuestCompleted += HandleQuestCompleted;
            GameEvents.OnCombatEnded += HandleCombatEnded;
            GameEvents.OnLocationDiscovered += HandleLocationDiscovered;
            GameEvents.OnCompanionRecruited += HandleCompanionRecruited;
        }

        /// <summary>
        /// Unsubscribe from game events
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnCharacterCreated -= HandleCharacterCreated;
            GameEvents.OnQuestCompleted -= HandleQuestCompleted;
            GameEvents.OnCombatEnded -= HandleCombatEnded;
            GameEvents.OnLocationDiscovered -= HandleLocationDiscovered;
            GameEvents.OnCompanionRecruited -= HandleCompanionRecruited;
        }

        /// <summary>
        /// Set the player character to track
        /// </summary>
        public void SetPlayer(Character character)
        {
            player = character;
            if (player != null)
            {
                progression = player.progression;
            }
        }

        /// <summary>
        /// Handle character creation event
        /// </summary>
        private void HandleCharacterCreated(Character character)
        {
            SetPlayer(character);
            
            // Award starting title based on character class
            if (progression != null)
            {
                if (character.characterClass == CharacterClass.Human)
                {
                    progression.EarnTitle(CharacterTitle.MortalHuntress);
                }
            }
        }

        /// <summary>
        /// Handle quest completion
        /// </summary>
        private void HandleQuestCompleted(string questId, string questName)
        {
            if (progression == null)
                return;

            // Award titles based on specific quests
            switch (questId)
            {
                case "main_001": // Crossing the Wall
                    CheckCrossingWallTitle();
                    break;
                    
                case "main_007": // The Three Trials - Worm
                    progression.EarnTitle(CharacterTitle.Survivor);
                    break;
                    
                case "main_008": // The Three Trials - Riddle
                    progression.EarnTitle(CharacterTitle.RiddleSolver);
                    break;
                    
                case "main_009": // The Three Trials - Hearts
                    progression.EarnTitle(CharacterTitle.Heartbreaker);
                    break;
                    
                case "main_010": // Breaking the Curse
                    progression.EarnTitle(CharacterTitle.CurseBreaker);
                    progression.EarnTitle(CharacterTitle.HighFae);
                    break;
                    
                case "main_book2_010": // High Lady Declaration
                    progression.EarnTitle(CharacterTitle.HighLadyOfNight);
                    break;
                    
                case "main_book2_015": // Joining Inner Circle
                    progression.EarnTitle(CharacterTitle.InnerCircleMember);
                    break;
                    
                case "main_book3_001": // Spy Mission
                    progression.EarnTitle(CharacterTitle.Spy);
                    break;
                    
                case "main_book3_015": // Unite the Courts
                    progression.EarnTitle(CharacterTitle.Diplomat);
                    break;
                    
                case "main_book3_020": // Final Battle
                    progression.EarnTitle(CharacterTitle.Warlord);
                    progression.EarnTitle(CharacterTitle.SaviorOfPrythian);
                    break;
            }

            // Check if Valkyrie training quest
            if (questId.Contains("valkyrie") || questName.ToLower().Contains("valkyrie"))
            {
                progression.EarnTitle(CharacterTitle.Valkyrie);
            }
        }

        /// <summary>
        /// Handle combat victory
        /// </summary>
        private void HandleCombatEnded(Character winner, System.Collections.Generic.List<Enemy> enemies, bool victory)
        {
            if (progression == null || !victory || winner != player)
                return;

            // Check for beast slayer title
            foreach (Enemy enemy in enemies)
            {
                if (enemy.enemyType == EnemyType.Naga || 
                    enemy.enemyType == EnemyType.Attor ||
                    enemy.enemyType == EnemyType.MountainWyrm)
                {
                    progression.EarnTitle(CharacterTitle.BeastSlayer);
                    break;
                }
            }

            // Check for ultimate warrior title (defeating bosses on nightmare)
            if (GameManager.Instance != null)
            {
                var difficultySettings = GameManager.Instance.GetComponent<DifficultySettings>();
                if (difficultySettings != null && difficultySettings.GetCurrentDifficulty() == DifficultyLevel.Nightmare)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy.difficulty == EnemyDifficulty.Boss)
                        {
                            CheckUltimateWarriorProgress();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle location discovery
        /// </summary>
        private void HandleLocationDiscovered(string locationName, string courtName)
        {
            if (progression == null)
                return;

            // Award Night Court Resident when first visiting Night Court locations
            if (courtName == "Night Court" && !progression.earnedTitles.Contains(CharacterTitle.NightCourtResident))
            {
                // Check if they've visited multiple Night Court locations
                if (progression.locationsDiscovered >= 3)
                {
                    progression.EarnTitle(CharacterTitle.NightCourtResident);
                }
            }

            // Award Court of Dreams title for mastering Night Court role
            if (locationName.Contains("House of Wind") || locationName.Contains("Velaris"))
            {
                CheckCourtOfDreamsProgress();
            }
        }

        /// <summary>
        /// Handle companion recruitment
        /// </summary>
        private void HandleCompanionRecruited(string companionName)
        {
            if (progression == null)
                return;

            // Check if all companions recruited
            if (progression.companionsRecruited >= 9) // All 9 main companions
            {
                // Check if all have max loyalty
                CheckCompanionOfLegendsProgress();
            }
        }

        /// <summary>
        /// Check progress for Crossing the Wall title conditions
        /// </summary>
        private void CheckCrossingWallTitle()
        {
            // Custom logic for specific title conditions
            if (progression != null)
            {
                Debug.Log("Crossed the Wall - milestone reached");
            }
        }

        /// <summary>
        /// Check progress for Court of Dreams title
        /// </summary>
        private void CheckCourtOfDreamsProgress()
        {
            if (progression == null)
                return;

            // Requires: Inner Circle member + High Lady + visited key Night Court locations
            if (progression.earnedTitles.Contains(CharacterTitle.InnerCircleMember) &&
                progression.earnedTitles.Contains(CharacterTitle.HighLadyOfNight) &&
                progression.questsCompleted >= 30)
            {
                progression.EarnTitle(CharacterTitle.CourtOfDreams);
            }
        }

        /// <summary>
        /// Check progress for Companion of Legends title
        /// </summary>
        private void CheckCompanionOfLegendsProgress()
        {
            if (progression == null || GameManager.Instance == null)
                return;

            var companionSystem = GameManager.Instance.GetComponent<CompanionSystem>();
            if (companionSystem == null)
                return;

            // Check if all companions have max loyalty (100)
            bool allMaxLoyalty = true;
            var companions = companionSystem.GetAllCompanions();
            
            foreach (var companion in companions)
            {
                if (companion.loyalty < 100)
                {
                    allMaxLoyalty = false;
                    break;
                }
            }

            if (allMaxLoyalty && companions.Count >= 9)
            {
                progression.EarnTitle(CharacterTitle.CompanionOfLegends);
            }
        }

        /// <summary>
        /// Check progress for Ultimate Warrior title
        /// </summary>
        private void CheckUltimateWarriorProgress()
        {
            if (progression == null)
                return;

            // Requires completing major bosses on Nightmare difficulty
            // Would need to track boss defeats specifically
            if (progression.enemiesDefeated >= 200 && progression.deaths == 0)
            {
                progression.EarnTitle(CharacterTitle.UltimateWarrior);
            }
        }

        /// <summary>
        /// Manually award a title (for special story events)
        /// </summary>
        public void AwardTitle(CharacterTitle title)
        {
            if (progression != null)
            {
                progression.EarnTitle(title);
                
                // Show UI notification
                if (UIManager.Instance != null)
                {
                    string titleName = progression.GetTitleName(title);
                    UIManager.Instance.ShowNotification($"Title Earned: {titleName}!", 5f);
                }
            }
        }

        /// <summary>
        /// Check for Cauldron-Born title
        /// </summary>
        public void CheckCauldronBornTitle()
        {
            if (progression == null || player == null)
                return;

            // Award if player has mastered certain magical abilities from the Cauldron
            if (player.abilities.KnowsAbility(MagicType.Shapeshifting) &&
                player.abilities.KnowsAbility(MagicType.DarknessManipulation) &&
                player.stats.magicPower >= 100)
            {
                progression.EarnTitle(CharacterTitle.CauldronBorn);
            }
        }
    }
}
