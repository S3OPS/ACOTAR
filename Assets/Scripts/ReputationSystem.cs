using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Reputation standing levels with each court
    /// </summary>
    public enum ReputationLevel
    {
        Hostile = 0,      // -100 to -51
        Unfriendly = 1,   // -50 to -26
        Neutral = 2,      // -25 to 25
        Friendly = 3,     // 26 to 50
        Honored = 4,      // 51 to 75
        Revered = 5,      // 76 to 100
        Exalted = 6       // 100+ (special status)
    }

    /// <summary>
    /// Reputation standing with a specific court
    /// </summary>
    [System.Serializable]
    public class CourtReputation
    {
        public Court court;
        public int reputationPoints; // -100 to 100+
        public ReputationLevel level;

        public CourtReputation(Court court)
        {
            this.court = court;
            this.reputationPoints = 0; // Start neutral
            UpdateLevel();
        }

        /// <summary>
        /// Gain reputation with this court
        /// </summary>
        public void GainReputation(int amount)
        {
            reputationPoints += amount;
            UpdateLevel();
            LogReputationChange(amount);
        }

        /// <summary>
        /// Lose reputation with this court
        /// </summary>
        public void LoseReputation(int amount)
        {
            reputationPoints -= amount;
            UpdateLevel();
            LogReputationChange(-amount);
        }

        /// <summary>
        /// Update reputation level based on points
        /// </summary>
        /// <remarks>
        /// Logs level changes to the LoggingSystem.
        /// v2.6.2: Enhanced with structured logging integration
        /// </remarks>
        private void UpdateLevel()
        {
            ReputationLevel oldLevel = level;

            if (reputationPoints >= 100)
                level = ReputationLevel.Exalted;
            else if (reputationPoints >= 76)
                level = ReputationLevel.Revered;
            else if (reputationPoints >= 51)
                level = ReputationLevel.Honored;
            else if (reputationPoints >= 26)
                level = ReputationLevel.Friendly;
            else if (reputationPoints >= -25)
                level = ReputationLevel.Neutral;
            else if (reputationPoints >= -50)
                level = ReputationLevel.Unfriendly;
            else
                level = ReputationLevel.Hostile;

            if (oldLevel != level)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Reputation", 
                    $"Reputation with {court} Court changed from {oldLevel} to {level}");
            }
        }

        /// <summary>
        /// Log reputation changes
        /// </summary>
        /// <remarks>
        /// v2.6.2: Enhanced with structured logging integration
        /// </remarks>
        private void LogReputationChange(int amount)
        {
            string change = amount > 0 ? "increased" : "decreased";
            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, "Reputation", 
                $"Reputation with {court} Court {change} by {Mathf.Abs(amount)} (Now: {reputationPoints}, {level})");
        }

        /// <summary>
        /// Get reputation discount multiplier for shops
        /// </summary>
        public float GetPriceMultiplier()
        {
            switch (level)
            {
                case ReputationLevel.Hostile:
                    return 1.5f;  // 50% more expensive
                case ReputationLevel.Unfriendly:
                    return 1.25f; // 25% more expensive
                case ReputationLevel.Neutral:
                    return 1.0f;  // Normal prices
                case ReputationLevel.Friendly:
                    return 0.9f;  // 10% discount
                case ReputationLevel.Honored:
                    return 0.8f;  // 20% discount
                case ReputationLevel.Revered:
                    return 0.7f;  // 30% discount
                case ReputationLevel.Exalted:
                    return 0.5f;  // 50% discount
                default:
                    return 1.0f;
            }
        }
    }

    /// <summary>
    /// Manages player's reputation with all seven High Fae Courts
    /// Tracks standing, provides benefits/penalties, and handles faction relations
    /// </summary>
    public class ReputationSystem
    {
        private Dictionary<Court, CourtReputation> courtReputations;
        private Character player;

        public ReputationSystem(Character player)
        {
            this.player = player;
            courtReputations = new Dictionary<Court, CourtReputation>();
            InitializeReputations();
        }

        /// <summary>
        /// Initialize reputation with all courts
        /// </summary>
        private void InitializeReputations()
        {
            courtReputations[Court.Spring] = new CourtReputation(Court.Spring);
            courtReputations[Court.Summer] = new CourtReputation(Court.Summer);
            courtReputations[Court.Autumn] = new CourtReputation(Court.Autumn);
            courtReputations[Court.Winter] = new CourtReputation(Court.Winter);
            courtReputations[Court.Night] = new CourtReputation(Court.Night);
            courtReputations[Court.Dawn] = new CourtReputation(Court.Dawn);
            courtReputations[Court.Day] = new CourtReputation(Court.Day);

            // Set player's allegiance court to Friendly
            if (courtReputations.ContainsKey(player.allegiance))
            {
                courtReputations[player.allegiance].GainReputation(30);
            }
        }

        /// <summary>
        /// Gain reputation with a specific court
        /// </summary>
        /// <param name="court">The court to gain reputation with</param>
        /// <param name="amount">The amount of reputation points to gain</param>
        /// <remarks>
        /// May cause reputation loss with rival courts.
        /// v2.6.2: Enhanced with error handling and structured logging
        /// </remarks>
        public void GainReputation(Court court, int amount)
        {
            try
            {
                if (amount <= 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Reputation", 
                        $"Cannot gain reputation: invalid amount {amount}");
                    return;
                }

                if (courtReputations == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Reputation", 
                        "Cannot gain reputation: courtReputations dictionary is null");
                    return;
                }

                if (courtReputations.ContainsKey(court))
                {
                    courtReputations[court].GainReputation(amount);
                    
                    // Rival courts may lose reputation
                    ApplyRivalPenalty(court, amount);
                }
                else
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Reputation", 
                        $"Court {court} not found in reputation system");
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Reputation", 
                    $"Exception in GainReputation({court}, {amount}): {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Lose reputation with a specific court
        /// </summary>
        /// <param name="court">The court to lose reputation with</param>
        /// <param name="amount">The amount of reputation points to lose</param>
        /// <remarks>
        /// v2.6.2: Enhanced with error handling and structured logging
        /// </remarks>
        public void LoseReputation(Court court, int amount)
        {
            try
            {
                if (amount <= 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Reputation", 
                        $"Cannot lose reputation: invalid amount {amount}");
                    return;
                }

                if (courtReputations == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Reputation", 
                        "Cannot lose reputation: courtReputations dictionary is null");
                    return;
                }

                if (courtReputations.ContainsKey(court))
                {
                    courtReputations[court].LoseReputation(amount);
                }
                else
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Reputation", 
                        $"Court {court} not found in reputation system");
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Reputation", 
                    $"Exception in LoseReputation({court}, {amount}): {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Get reputation with a specific court
        /// </summary>
        public CourtReputation GetReputation(Court court)
        {
            return courtReputations.ContainsKey(court) ? courtReputations[court] : null;
        }

        /// <summary>
        /// Get reputation level with a court
        /// </summary>
        public ReputationLevel GetReputationLevel(Court court)
        {
            return courtReputations.ContainsKey(court) ? courtReputations[court].level : ReputationLevel.Neutral;
        }

        /// <summary>
        /// Check if player is welcome in a court
        /// </summary>
        public bool IsWelcome(Court court)
        {
            ReputationLevel level = GetReputationLevel(court);
            return level >= ReputationLevel.Neutral;
        }

        /// <summary>
        /// Check if player is hostile to a court
        /// </summary>
        public bool IsHostile(Court court)
        {
            ReputationLevel level = GetReputationLevel(court);
            return level == ReputationLevel.Hostile;
        }

        /// <summary>
        /// Apply reputation penalties to rival courts
        /// Some courts have historical rivalries
        /// </summary>
        private void ApplyRivalPenalty(Court gainedCourt, int amount)
        {
            // Spring vs. Night rivalry
            if (gainedCourt == Court.Night && amount > 10)
            {
                LoseReputation(Court.Spring, amount / 4);
            }
            else if (gainedCourt == Court.Spring && amount > 10)
            {
                LoseReputation(Court.Night, amount / 4);
            }

            // Add more court rivalries as needed
        }

        /// <summary>
        /// Handle reputation changes from quest completion
        /// </summary>
        public void OnQuestCompleted(Quest quest)
        {
            // Court quests grant reputation
            if (quest.type == QuestType.CourtQuest)
            {
                // Determine which court the quest is for (would need quest metadata)
                // For now, grant reputation to player's allegiance court
                GainReputation(player.allegiance, 10);
            }
        }

        /// <summary>
        /// Handle reputation changes from combat victories
        /// </summary>
        public void OnEnemyDefeated(Enemy enemy)
        {
            // Defeating enemies of hostile courts may grant reputation
            if (enemy.allegiance != player.allegiance)
            {
                GainReputation(player.allegiance, 5);
                LoseReputation(enemy.allegiance, 5);
            }
        }

        /// <summary>
        /// Display all court reputations
        /// </summary>
        public void DisplayReputations()
        {
            Debug.Log("\n=== Court Reputations ===");
            foreach (var kvp in courtReputations)
            {
                CourtReputation rep = kvp.Value;
                string status = rep.court == player.allegiance ? " (Allegiance)" : "";
                Debug.Log($"{rep.court} Court: {rep.level} ({rep.reputationPoints} points){status}");
            }
            Debug.Log("========================\n");
        }

        /// <summary>
        /// Get price multiplier for a court's shops
        /// </summary>
        public float GetCourtPriceMultiplier(Court court)
        {
            return courtReputations.ContainsKey(court) ? courtReputations[court].GetPriceMultiplier() : 1.0f;
        }

        /// <summary>
        /// Check if player can receive special court missions
        /// </summary>
        public bool CanReceiveCourtMissions(Court court)
        {
            ReputationLevel level = GetReputationLevel(court);
            return level >= ReputationLevel.Friendly;
        }

        /// <summary>
        /// Check if player has access to court's exclusive areas
        /// </summary>
        public bool HasCourtAccess(Court court)
        {
            ReputationLevel level = GetReputationLevel(court);
            return level >= ReputationLevel.Honored;
        }
    }
}
