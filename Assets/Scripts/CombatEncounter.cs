using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Combat encounter state
    /// </summary>
    public enum EncounterState
    {
        NotStarted,
        PlayerTurn,
        EnemyTurn,
        Victory,
        Defeat,
        Fled
    }

    /// <summary>
    /// Manages turn-based combat encounters between player and enemies
    /// Coordinates combat flow, turn order, and victory/defeat conditions
    /// </summary>
    public class CombatEncounter
    {
        public Character player;
        public List<Enemy> enemies;
        public EncounterState state;
        public int turnNumber;
        public int currentEnemyIndex;

        private List<string> combatLog;

        /// <summary>
        /// Initialize a new combat encounter
        /// </summary>
        public CombatEncounter(Character player, List<Enemy> enemies)
        {
            this.player = player;
            this.enemies = enemies;
            this.state = EncounterState.NotStarted;
            this.turnNumber = 0;
            this.currentEnemyIndex = 0;
            this.combatLog = new List<string>();
        }

        /// <summary>
        /// Start the combat encounter
        /// </summary>
        public void StartEncounter()
        {
            state = EncounterState.PlayerTurn;
            turnNumber = 1;
            LogMessage("=== Combat Encounter Started ===");
            LogMessage($"Enemies: {GetEnemyListString()}");
            GameEvents.TriggerCombatStarted(player, enemies);
        }

        /// <summary>
        /// Execute player's physical attack
        /// </summary>
        public void PlayerPhysicalAttack(Enemy target)
        {
            if (state != EncounterState.PlayerTurn)
            {
                Debug.LogWarning("Not player's turn!");
                return;
            }

            CombatResult result = CombatSystem.CalculatePhysicalAttack(player, target);
            target.TakeDamage(result.damage);
            LogMessage(result.description);

            // Trigger visual effects
            TriggerHitVisualEffects(result, Vector3.zero);

            CheckEnemyDefeated(target);
            EndPlayerTurn();
        }

        /// <summary>
        /// Execute player's magic attack
        /// </summary>
        public void PlayerMagicAttack(Enemy target, MagicType magicType)
        {
            if (state != EncounterState.PlayerTurn)
            {
                Debug.LogWarning("Not player's turn!");
                return;
            }

            CombatResult result = CombatSystem.CalculateMagicAttack(player, target, magicType);
            target.TakeDamage(result.damage);
            LogMessage(result.description);

            // Trigger magic visual effects
            TriggerMagicVisualEffects(magicType, result, Vector3.zero, Vector3.zero);

            CheckEnemyDefeated(target);
            EndPlayerTurn();
        }

        /// <summary>
        /// Player defends (reduces incoming damage next turn)
        /// </summary>
        public void PlayerDefend()
        {
            if (state != EncounterState.PlayerTurn)
            {
                Debug.LogWarning("Not player's turn!");
                return;
            }

            LogMessage($"{player.name} takes a defensive stance!");
            // Defense will be applied during enemy turn
            EndPlayerTurn();
        }

        /// <summary>
        /// Player attempts to flee from combat
        /// </summary>
        public bool PlayerFlee()
        {
            if (state != EncounterState.PlayerTurn)
            {
                Debug.LogWarning("Not player's turn!");
                return false;
            }

            // Calculate flee chance based on strongest enemy
            Enemy strongestEnemy = GetStrongestEnemy();
            bool success = CombatSystem.AttemptFlee(player, strongestEnemy);

            if (success)
            {
                LogMessage($"{player.name} successfully fled from combat!");
                state = EncounterState.Fled;
                GameEvents.TriggerCombatEnded(player, enemies, false);
                return true;
            }
            else
            {
                LogMessage($"{player.name} failed to escape!");
                EndPlayerTurn();
                return false;
            }
        }

        /// <summary>
        /// Use a consumable item
        /// </summary>
        public void PlayerUseItem(string itemId)
        {
            if (state != EncounterState.PlayerTurn)
            {
                Debug.LogWarning("Not player's turn!");
                return;
            }

            // Item usage would be implemented here
            LogMessage($"{player.name} used {itemId}");
            EndPlayerTurn();
        }

        /// <summary>
        /// End player turn and start enemy turns
        /// </summary>
        private void EndPlayerTurn()
        {
            if (CheckVictory())
            {
                HandleVictory();
                return;
            }

            state = EncounterState.EnemyTurn;
            currentEnemyIndex = 0;
            ExecuteEnemyTurns();
        }

        /// <summary>
        /// Execute all enemy turns
        /// </summary>
        private void ExecuteEnemyTurns()
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.IsAlive())
                {
                    ExecuteEnemyAI(enemy);
                }
            }

            if (CheckDefeat())
            {
                HandleDefeat();
                return;
            }

            // Start next turn
            turnNumber++;
            state = EncounterState.PlayerTurn;
            LogMessage($"\n--- Turn {turnNumber} ---");
        }

        /// <summary>
        /// Execute AI behavior for an enemy
        /// </summary>
        private void ExecuteEnemyAI(Enemy enemy)
        {
            // Simple AI based on behavior pattern
            switch (enemy.behavior)
            {
                case EnemyBehavior.Aggressive:
                    ExecuteAggressiveAI(enemy);
                    break;
                case EnemyBehavior.Defensive:
                    ExecuteDefensiveAI(enemy);
                    break;
                case EnemyBehavior.Tactical:
                    ExecuteTacticalAI(enemy);
                    break;
                case EnemyBehavior.Berserker:
                    ExecuteBerserkerAI(enemy);
                    break;
                default:
                    ExecuteBalancedAI(enemy);
                    break;
            }
        }

        /// <summary>
        /// Aggressive AI - always attacks with highest damage
        /// </summary>
        private void ExecuteAggressiveAI(Enemy enemy)
        {
            // Check if enemy has magic abilities
            if (enemy.abilities.Count > 0 && Random.value < 0.5f)
            {
                MagicType ability = enemy.abilities[Random.Range(0, enemy.abilities.Count)];
                CombatResult result = CombatSystem.CalculateMagicAttack(enemy, player, ability);
                player.TakeDamage(result.damage);
                LogMessage(result.description);
            }
            else
            {
                CombatResult result = CombatSystem.CalculatePhysicalAttack(enemy, player);
                player.TakeDamage(result.damage);
                LogMessage(result.description);
            }
        }

        /// <summary>
        /// Defensive AI - prioritizes survival
        /// </summary>
        private void ExecuteDefensiveAI(Enemy enemy)
        {
            if (enemy.health < enemy.maxHealth * 0.3f)
            {
                // Low health, defend
                LogMessage($"{enemy.name} takes a defensive stance!");
            }
            else
            {
                // Attack with 50% chance
                if (Random.value < 0.5f)
                {
                    CombatResult result = CombatSystem.CalculatePhysicalAttack(enemy, player);
                    player.TakeDamage(result.damage);
                    LogMessage(result.description);
                }
                else
                {
                    LogMessage($"{enemy.name} defends!");
                }
            }
        }

        /// <summary>
        /// Tactical AI - uses abilities strategically
        /// </summary>
        private void ExecuteTacticalAI(Enemy enemy)
        {
            if (enemy.abilities.Count > 0 && enemy.magicPower > 0)
            {
                // Prefer magic attacks
                MagicType ability = enemy.abilities[Random.Range(0, enemy.abilities.Count)];
                CombatResult result = CombatSystem.CalculateMagicAttack(enemy, player, ability);
                player.TakeDamage(result.damage);
                LogMessage(result.description);
            }
            else
            {
                CombatResult result = CombatSystem.CalculatePhysicalAttack(enemy, player);
                player.TakeDamage(result.damage);
                LogMessage(result.description);
            }
        }

        /// <summary>
        /// Berserker AI - all-out attack
        /// </summary>
        private void ExecuteBerserkerAI(Enemy enemy)
        {
            // Double attack if health is low
            CombatResult result = CombatSystem.CalculatePhysicalAttack(enemy, player);
            player.TakeDamage(result.damage);
            LogMessage(result.description);

            if (enemy.health < enemy.maxHealth * 0.5f)
            {
                result = CombatSystem.CalculatePhysicalAttack(enemy, player);
                player.TakeDamage(result.damage);
                LogMessage($"{enemy.name} attacks again in a frenzy!");
                LogMessage(result.description);
            }
        }

        /// <summary>
        /// Balanced AI - mix of attacks
        /// </summary>
        private void ExecuteBalancedAI(Enemy enemy)
        {
            ExecuteAggressiveAI(enemy);
        }

        /// <summary>
        /// Check if all enemies are defeated (public accessor)
        /// </summary>
        public bool CheckVictory()
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.IsAlive())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check if player is defeated (public accessor)
        /// </summary>
        public bool CheckDefeat()
        {
            return !player.IsAlive();
        }

        /// <summary>
        /// Get list of enemies in the encounter
        /// </summary>
        public List<Enemy> GetEnemies()
        {
            return enemies;
        }

        /// <summary>
        /// Check if specific enemy is defeated and remove from combat
        /// </summary>
        private void CheckEnemyDefeated(Enemy enemy)
        {
            if (!enemy.IsAlive())
            {
                LogMessage($"{enemy.name} has been defeated!");
            }
        }

        /// <summary>
        /// Handle combat victory
        /// </summary>
        private void HandleVictory()
        {
            state = EncounterState.Victory;
            LogMessage("\n=== VICTORY ===");

            // Trigger victory visual effects
            TriggerVictoryEffects();

            // Grant experience
            int totalXP = 0;
            List<string> allLoot = new List<string>();

            foreach (Enemy enemy in enemies)
            {
                totalXP += enemy.experienceReward;
                List<string> loot = enemy.DropLoot();
                allLoot.AddRange(loot);
            }

            player.GainExperience(totalXP);
            LogMessage($"Gained {totalXP} experience!");

            if (allLoot.Count > 0)
            {
                LogMessage($"Loot dropped: {string.Join(", ", allLoot)}");
            }

            GameEvents.TriggerCombatEnded(player, enemies, true);
        }

        /// <summary>
        /// Handle combat defeat
        /// </summary>
        private void HandleDefeat()
        {
            state = EncounterState.Defeat;
            LogMessage("\n=== DEFEAT ===");
            LogMessage($"{player.name} has fallen...");
            
            // Trigger defeat visual effects
            TriggerDefeatEffects();
            
            GameEvents.TriggerCombatEnded(player, enemies, false);
        }

        /// <summary>
        /// Get strongest enemy (for flee calculations)
        /// </summary>
        private Enemy GetStrongestEnemy()
        {
            Enemy strongest = enemies[0];
            foreach (Enemy enemy in enemies)
            {
                if (enemy.IsAlive() && enemy.strength > strongest.strength)
                {
                    strongest = enemy;
                }
            }
            return strongest;
        }

        /// <summary>
        /// Get string representation of enemy list
        /// </summary>
        private string GetEnemyListString()
        {
            List<string> names = new List<string>();
            foreach (Enemy enemy in enemies)
            {
                names.Add(enemy.name);
            }
            return string.Join(", ", names);
        }

        /// <summary>
        /// Add message to combat log
        /// </summary>
        private void LogMessage(string message)
        {
            combatLog.Add(message);
            Debug.Log(message);
        }

        /// <summary>
        /// Get full combat log
        /// </summary>
        public List<string> GetCombatLog()
        {
            return new List<string>(combatLog);
        }

        #region Visual Effects Integration

        /// <summary>
        /// Trigger visual effects for physical hit
        /// </summary>
        private void TriggerHitVisualEffects(CombatResult result, Vector3 targetPosition)
        {
            // Spawn hit particle effect
            if (VisualEffectsManager.Instance != null)
            {
                VisualEffectsManager.Instance.SpawnHitEffect(targetPosition, result.isCritical, result.damageType);
            }

            // Trigger screen effects
            if (ScreenEffectsManager.Instance != null)
            {
                ScreenEffectsManager.Instance.CombatHitFeedback(result.isCritical, result.damageType);
            }
        }

        /// <summary>
        /// Trigger visual effects for magic attack
        /// </summary>
        private void TriggerMagicVisualEffects(MagicType magicType, CombatResult result, Vector3 casterPosition, Vector3 targetPosition)
        {
            // Spawn magic effect
            if (VisualEffectsManager.Instance != null)
            {
                VisualEffectsManager.Instance.SpawnMagicEffect(magicType, casterPosition, targetPosition);
            }

            // Trigger screen flash for elemental magic
            if (ScreenEffectsManager.Instance != null)
            {
                Element element = ElementalSystem.GetElementFromMagic(magicType);
                if (element != Element.None)
                {
                    ScreenEffectsManager.Instance.MagicFlash(element);
                }
                
                if (result.isCritical)
                {
                    ScreenEffectsManager.Instance.CriticalFlash();
                }
            }
        }

        /// <summary>
        /// Trigger visual effects when player takes damage
        /// </summary>
        private void TriggerPlayerDamageEffects(int damage)
        {
            if (ScreenEffectsManager.Instance != null)
            {
                float damagePercent = (float)damage / player.maxHealth;
                ScreenEffectsManager.Instance.PlayerDamageFeedback(damagePercent);
            }
        }

        /// <summary>
        /// Trigger victory visual effects
        /// </summary>
        private void TriggerVictoryEffects()
        {
            if (VisualEffectsManager.Instance != null)
            {
                VisualEffectsManager.Instance.SpawnEffect(VFXType.QuestComplete, Vector3.zero);
            }

            if (ScreenEffectsManager.Instance != null)
            {
                ScreenEffectsManager.Instance.QuestCompleteFeedback();
            }
        }

        /// <summary>
        /// Trigger defeat visual effects
        /// </summary>
        private void TriggerDefeatEffects()
        {
            if (ScreenEffectsManager.Instance != null)
            {
                ScreenEffectsManager.Instance.FadeToBlack(1f);
            }
        }

        #endregion
    }
}
