using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Boss combat phases - different behavior patterns as health depletes
    /// </summary>
    public enum BossPhase
    {
        Phase1,  // Full health - 100% to 66%
        Phase2,  // Moderate damage - 66% to 33%
        Phase3,  // Critical - 33% to 0%
        Enraged  // Special triggered state
    }

    /// <summary>
    /// Unique boss-only abilities
    /// </summary>
    public enum BossAbility
    {
        SummonMinions,       // Spawn helper enemies
        AreaOfEffect,        // AOE damage attack
        LifeDrain,           // Steal health from player
        EnrageMode,          // Temporary massive damage boost
        Shield,              // Temporary invulnerability
        Teleport,            // Reposition/dodge
        StatusCurse,         // Apply multiple status effects
        UltimateAttack,      // Devastating final move
        EnvironmentalHazard, // Trigger stage hazards
        SoulBind             // Unique debuff mechanic
    }

    /// <summary>
    /// Environmental hazard types
    /// </summary>
    public enum EnvironmentalHazard
    {
        FallingRocks,    // Periodic damage
        FirePits,        // Standing damage zones
        PoisonGas,       // Slowly drains health
        DarknessWave,    // Reduces visibility/accuracy
        MagicVortex,     // Drains mana
        IcyGround,       // Reduces agility
        ThornWalls       // Blocks escape attempts
    }

    /// <summary>
    /// Boss phase configuration
    /// </summary>
    [System.Serializable]
    public class BossPhaseConfig
    {
        public BossPhase phase;
        public float healthThresholdPercent;  // Trigger when health drops below this
        public float damageMultiplier;        // Damage increase in this phase
        public float attackSpeedMultiplier;   // Attack frequency increase
        public List<BossAbility> availableAbilities;
        public bool canSummonMinions;
        public int minionCount;
        public string phaseTransitionMessage;
        
        public BossPhaseConfig(BossPhase p, float healthThreshold)
        {
            phase = p;
            healthThresholdPercent = healthThreshold;
            damageMultiplier = 1.0f;
            attackSpeedMultiplier = 1.0f;
            availableAbilities = new List<BossAbility>();
            canSummonMinions = false;
            minionCount = 0;
            phaseTransitionMessage = "";
        }
    }

    /// <summary>
    /// Boss-specific combat state
    /// </summary>
    [System.Serializable]
    public class BossEncounterState
    {
        public string bossName;
        public BossPhase currentPhase;
        public int phaseTransitionCount;
        public bool hasEnraged;
        public List<Enemy> summonedMinions;
        public List<EnvironmentalHazard> activeHazards;
        public int ultimateChargeCounter;  // Builds up over turns
        public bool isInvulnerable;
        public int invulnerabilityTurnsLeft;
        
        public BossEncounterState(string name)
        {
            bossName = name;
            currentPhase = BossPhase.Phase1;
            phaseTransitionCount = 0;
            hasEnraged = false;
            summonedMinions = new List<Enemy>();
            activeHazards = new List<EnvironmentalHazard>();
            ultimateChargeCounter = 0;
            isInvulnerable = false;
            invulnerabilityTurnsLeft = 0;
        }
    }

    /// <summary>
    /// Enhanced Boss Mechanics System
    /// Version 2.6.0 - New Feature
    /// 
    /// Provides multi-phase boss encounters with:
    /// - Dynamic phase transitions based on health
    /// - Boss-specific unique abilities
    /// - Minion summoning mechanics
    /// - Environmental hazards
    /// - Ultimate attack charging
    /// - Invulnerability phases
    /// </summary>
    public class EnhancedBossMechanics
    {
        private Dictionary<string, List<BossPhaseConfig>> bossConfigurations;
        private Dictionary<string, BossEncounterState> activeEncounters;
        
        // Property accessors (v2.6.0: Following v2.5.x patterns)
        public bool IsInitialized => bossConfigurations != null && activeEncounters != null;
        public int ActiveEncounterCount => activeEncounters?.Count ?? 0;
        
        public EnhancedBossMechanics()
        {
            bossConfigurations = new Dictionary<string, List<BossPhaseConfig>>();
            activeEncounters = new Dictionary<string, BossEncounterState>();
            InitializeBossConfigurations();
            Debug.Log("EnhancedBossMechanics initialized with multi-phase boss system");
        }

        /// <summary>
        /// Initialize boss phase configurations for all major bosses
        /// Based on ACOTAR lore
        /// </summary>
        private void InitializeBossConfigurations()
        {
            // Amarantha - Under the Mountain
            var amaranthaPhases = new List<BossPhaseConfig>();
            
            var amaranthaP1 = new BossPhaseConfig(BossPhase.Phase1, 0.66f);
            amaranthaP1.damageMultiplier = 1.0f;
            amaranthaP1.availableAbilities.Add(BossAbility.StatusCurse);
            amaranthaP1.availableAbilities.Add(BossAbility.LifeDrain);
            amaranthaP1.phaseTransitionMessage = "Amarantha's eyes gleam with malice as she taps into deeper power...";
            amaranthaPhases.Add(amaranthaP1);
            
            var amaranthaP2 = new BossPhaseConfig(BossPhase.Phase2, 0.33f);
            amaranthaP2.damageMultiplier = 1.25f;
            amaranthaP2.attackSpeedMultiplier = 1.2f;
            amaranthaP2.availableAbilities.Add(BossAbility.SummonMinions);
            amaranthaP2.availableAbilities.Add(BossAbility.AreaOfEffect);
            amaranthaP2.canSummonMinions = true;
            amaranthaP2.minionCount = 2;
            amaranthaP2.phaseTransitionMessage = "Amarantha summons her Attor servants! The curse strengthens!";
            amaranthaPhases.Add(amaranthaP2);
            
            var amaranthaP3 = new BossPhaseConfig(BossPhase.Phase3, 0.0f);
            amaranthaP3.damageMultiplier = 1.5f;
            amaranthaP3.attackSpeedMultiplier = 1.4f;
            amaranthaP3.availableAbilities.Add(BossAbility.EnrageMode);
            amaranthaP3.availableAbilities.Add(BossAbility.UltimateAttack);
            amaranthaP3.phaseTransitionMessage = "Amarantha unleashes her full fury! Her power is overwhelming!";
            amaranthaPhases.Add(amaranthaP3);
            
            bossConfigurations["Amarantha"] = amaranthaPhases;
            
            // The Middengard Wyrm - First Trial
            var wyrmPhases = new List<BossPhaseConfig>();
            
            var wyrmP1 = new BossPhaseConfig(BossPhase.Phase1, 0.50f);
            wyrmP1.damageMultiplier = 1.0f;
            wyrmP1.availableAbilities.Add(BossAbility.AreaOfEffect);
            wyrmP1.phaseTransitionMessage = "The Wyrm roars and thrashes wildly!";
            wyrmPhases.Add(wyrmP1);
            
            var wyrmP2 = new BossPhaseConfig(BossPhase.Phase2, 0.0f);
            wyrmP2.damageMultiplier = 1.3f;
            wyrmP2.availableAbilities.Add(BossAbility.EnrageMode);
            wyrmP2.availableAbilities.Add(BossAbility.EnvironmentalHazard);
            wyrmP2.phaseTransitionMessage = "The Wyrm's thrashing causes the cave to collapse around you!";
            wyrmPhases.Add(wyrmP2);
            
            bossConfigurations["Middengard Wyrm"] = wyrmPhases;
            
            // King of Hybern - Final Book 2 Boss
            var hybernPhases = new List<BossPhaseConfig>();
            
            var hybernP1 = new BossPhaseConfig(BossPhase.Phase1, 0.75f);
            hybernP1.damageMultiplier = 1.2f;
            hybernP1.availableAbilities.Add(BossAbility.StatusCurse);
            hybernP1.availableAbilities.Add(BossAbility.Teleport);
            hybernP1.phaseTransitionMessage = "The King of Hybern channels the Cauldron's power!";
            hybernPhases.Add(hybernP1);
            
            var hybernP2 = new BossPhaseConfig(BossPhase.Phase2, 0.50f);
            hybernP2.damageMultiplier = 1.4f;
            hybernP2.availableAbilities.Add(BossAbility.SummonMinions);
            hybernP2.availableAbilities.Add(BossAbility.Shield);
            hybernP2.canSummonMinions = true;
            hybernP2.minionCount = 3;
            hybernP2.phaseTransitionMessage = "The King summons his elite guard! He becomes shielded!";
            hybernPhases.Add(hybernP2);
            
            var hybernP3 = new BossPhaseConfig(BossPhase.Phase3, 0.25f);
            hybernP3.damageMultiplier = 1.6f;
            hybernP3.attackSpeedMultiplier = 1.5f;
            hybernP3.availableAbilities.Add(BossAbility.LifeDrain);
            hybernP3.availableAbilities.Add(BossAbility.UltimateAttack);
            hybernP3.phaseTransitionMessage = "The King enters his final form, drawing power directly from the Cauldron!";
            hybernPhases.Add(hybernP3);
            
            bossConfigurations["King of Hybern"] = hybernPhases;
            
            // Attor Swarm Leader - Mid-game Boss
            var attorPhases = new List<BossPhaseConfig>();
            
            var attorP1 = new BossPhaseConfig(BossPhase.Phase1, 0.60f);
            attorP1.damageMultiplier = 1.0f;
            attorP1.availableAbilities.Add(BossAbility.SummonMinions);
            attorP1.canSummonMinions = true;
            attorP1.minionCount = 2;
            attorP1.phaseTransitionMessage = "The Attor Leader screeches, calling more of its kind!";
            attorPhases.Add(attorP1);
            
            var attorP2 = new BossPhaseConfig(BossPhase.Phase2, 0.0f);
            attorP2.damageMultiplier = 1.3f;
            attorP2.attackSpeedMultiplier = 1.3f;
            attorP2.availableAbilities.Add(BossAbility.EnvironmentalHazard);
            attorP2.canSummonMinions = true;
            attorP2.minionCount = 3;
            attorP2.phaseTransitionMessage = "The swarm grows more frenzied!";
            attorPhases.Add(attorP2);
            
            bossConfigurations["Attor Leader"] = attorPhases;
        }

        /// <summary>
        /// Start a boss encounter
        /// v2.6.4: Enhanced with error handling, validation, and structured logging
        /// </summary>
        /// <param name="bossName">Name of the boss to start encounter with</param>
        /// <returns>BossEncounterState for the started encounter, or null on error</returns>
        /// <remarks>
        /// This method initializes a new boss encounter with multi-phase mechanics.
        /// Enhanced in v2.6.4 with:
        /// - Try-catch for exception protection
        /// - Enhanced null/empty string validation
        /// - Null checking for configuration dictionary
        /// - Structured logging via LoggingSystem
        /// Returns null if boss is not found or on error.
        /// </remarks>
        public BossEncounterState StartBossEncounter(string bossName)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(bossName))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "BossMechanics", "Cannot start encounter: boss name is null or empty");
                    return null;
                }

                if (bossConfigurations == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "BossMechanics", "Cannot start encounter: boss configurations dictionary is null");
                    return null;
                }

                if (!bossConfigurations.ContainsKey(bossName))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "BossMechanics", $"No configuration found for boss '{bossName}'");
                    return null;
                }

                if (activeEncounters == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "BossMechanics", "Cannot start encounter: active encounters dictionary is null");
                    return null;
                }

                var encounterState = new BossEncounterState(bossName);
                activeEncounters[bossName] = encounterState;
                
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                    "BossMechanics", $"🎭 Boss Encounter Started: {bossName}");
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                    "BossMechanics", $"⚔️ Multi-phase battle initiated for {bossName}");
                
                return encounterState;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "BossMechanics", $"Exception in StartBossEncounter: {ex.Message}\nStack: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Update boss phase based on current health percentage
        /// v2.6.4: Enhanced with error handling, validation, and structured logging
        /// </summary>
        /// <param name="bossName">Name of the boss to update phase for</param>
        /// <param name="currentHealthPercent">Current health as percentage (0.0 to 1.0)</param>
        /// <returns>New BossPhaseConfig if phase changed, null otherwise</returns>
        /// <remarks>
        /// This method checks and updates the boss combat phase based on health thresholds.
        /// Enhanced in v2.6.4 with:
        /// - Try-catch for exception protection
        /// - Enhanced initialization and null checking
        /// - Protected phase transition logic
        /// - Structured logging via LoggingSystem
        /// Returns null if no phase change occurs or on error.
        /// </remarks>
        public BossPhaseConfig UpdateBossPhase(string bossName, float currentHealthPercent)
        {
            try
            {
                // Initialization check
                if (!IsInitialized)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "BossMechanics", "Cannot update phase: system not initialized");
                    return null;
                }

                // Input validation
                if (string.IsNullOrEmpty(bossName))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "BossMechanics", "Cannot update phase: boss name is null or empty");
                    return null;
                }

                if (activeEncounters == null || bossConfigurations == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "BossMechanics", "Cannot update phase: encounter or configuration dictionary is null");
                    return null;
                }

                if (!activeEncounters.ContainsKey(bossName) || !bossConfigurations.ContainsKey(bossName))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                        "BossMechanics", $"Boss '{bossName}' not found in active encounters or configurations");
                    return null;
                }

                var encounterState = activeEncounters[bossName];
                if (encounterState == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "BossMechanics", $"Encounter state for '{bossName}' exists but is null");
                    return null;
                }

                var phases = bossConfigurations[bossName];
                if (phases == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "BossMechanics", $"Phase configuration for '{bossName}' exists but is null");
                    return null;
                }
                
                // Check for phase transition
                foreach (var phase in phases)
                {
                    try
                    {
                        if (phase == null) continue;

                        if (currentHealthPercent <= phase.healthThresholdPercent && 
                            encounterState.currentPhase < phase.phase)
                        {
                            encounterState.currentPhase = phase.phase;
                            encounterState.phaseTransitionCount++;
                            
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                                "BossMechanics", $"⚡ PHASE TRANSITION: {bossName} entering {phase.phase}");
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                                "BossMechanics", $"🎭 {phase.phaseTransitionMessage}");
                            
                            // Trigger phase-specific effects - protected
                            try
                            {
                                OnPhaseTransition(encounterState, phase);
                            }
                            catch (System.Exception transitionEx)
                            {
                                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                                    "BossMechanics", $"Exception in phase transition: {transitionEx.Message}");
                            }
                            
                            return phase;
                        }
                    }
                    catch (System.Exception phaseEx)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "BossMechanics", $"Exception checking phase: {phaseEx.Message}");
                        // Continue with other phases
                    }
                }
                
                // Return current phase config
                return phases.Find(p => p.phase == encounterState.currentPhase);
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "BossMechanics", $"Exception in UpdateBossPhase: {ex.Message}\nStack: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Handle phase transition effects
        /// </summary>
        private void OnPhaseTransition(BossEncounterState state, BossPhaseConfig newPhase)
        {
            // Summon minions if configured
            if (newPhase.canSummonMinions && newPhase.minionCount > 0)
            {
                Debug.Log($"⚔️ Boss summons {newPhase.minionCount} minions!");
                // Minion summoning would be handled by CombatEncounter
            }
            
            // Activate environmental hazards
            if (newPhase.availableAbilities.Contains(BossAbility.EnvironmentalHazard))
            {
                ActivateRandomHazard(state);
            }
            
            // Grant temporary invulnerability during some transitions
            if (newPhase.phase == BossPhase.Phase2)
            {
                state.isInvulnerable = true;
                state.invulnerabilityTurnsLeft = 1;
                Debug.Log($"🛡️ Boss is temporarily invulnerable during phase transition!");
            }
        }

        /// <summary>
        /// Execute a boss ability on a target
        /// v2.6.4: Enhanced with error handling, validation, and structured logging
        /// </summary>
        /// <param name="bossName">Name of the boss executing the ability</param>
        /// <param name="ability">Boss ability to execute</param>
        /// <param name="target">Character target of the ability</param>
        /// <returns>Description message of the executed ability</returns>
        /// <remarks>
        /// This method executes a special boss-only ability during combat.
        /// Enhanced in v2.6.4 with:
        /// - Try-catch for exception protection
        /// - Enhanced validation for all parameters
        /// - Null checking for state and dictionaries
        /// - Protected ability execution calls
        /// - Structured logging via LoggingSystem
        /// Returns error message on failure.
        /// </remarks>
        public string ExecuteBossAbility(string bossName, BossAbility ability, Character target)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(bossName))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "BossMechanics", "Cannot execute ability: boss name is null or empty");
                    return "Invalid boss ability execution";
                }

                if (target == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "BossMechanics", $"Cannot execute ability for {bossName}: target is null");
                    return "Invalid boss ability execution";
                }

                if (activeEncounters == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "BossMechanics", "Cannot execute ability: active encounters dictionary is null");
                    return "Invalid boss ability execution";
                }

                if (!activeEncounters.ContainsKey(bossName))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "BossMechanics", $"Boss {bossName} is not in an active encounter");
                    return $"{bossName} is not in an active encounter";
                }

                var state = activeEncounters[bossName];
                if (state == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "BossMechanics", $"Encounter state for {bossName} exists but is null");
                    return "Invalid boss ability execution";
                }

                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                    "BossMechanics", $"{bossName} executing ability: {ability}");
                
                // Execute ability with protected calls
                string result;
                switch (ability)
                {
                    case BossAbility.LifeDrain:
                        result = ExecuteLifeDrain(bossName, target, state);
                        break;
                        
                    case BossAbility.AreaOfEffect:
                        result = ExecuteAreaOfEffect(bossName, state);
                        break;
                        
                    case BossAbility.StatusCurse:
                        result = ExecuteStatusCurse(bossName, target);
                        break;
                        
                    case BossAbility.EnrageMode:
                        result = ExecuteEnrage(bossName, state);
                        break;
                        
                    case BossAbility.Shield:
                        result = ExecuteShield(state);
                        break;
                        
                    case BossAbility.UltimateAttack:
                        result = ExecuteUltimate(bossName, target, state);
                        break;
                        
                    case BossAbility.Teleport:
                        result = ExecuteTeleport(bossName);
                        break;
                        
                    case BossAbility.SummonMinions:
                        result = "Boss summons reinforcements!";
                        break;
                        
                    case BossAbility.EnvironmentalHazard:
                        result = ExecuteEnvironmentalHazard(state);
                        break;
                        
                    default:
                        result = $"{bossName} uses {ability}!";
                        break;
                }

                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, 
                    "BossMechanics", $"Ability execution result: {result}");
                
                return result;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "BossMechanics", $"Exception in ExecuteBossAbility: {ex.Message}\nStack: {ex.StackTrace}");
                return "Boss ability execution failed";
            }
        }

        private string ExecuteLifeDrain(string bossName, Character target, BossEncounterState state)
        {
            int drainAmount = Mathf.RoundToInt(target.maxHealth * 0.15f); // 15% of max health
            target.TakeDamage(drainAmount);
            return $"💀 {bossName} drains {drainAmount} life from {target.name}!";
        }

        private string ExecuteAreaOfEffect(string bossName, BossEncounterState state)
        {
            return $"💥 {bossName} unleashes a devastating area attack! All party members take damage!";
        }

        private string ExecuteStatusCurse(string bossName, Character target)
        {
            return $"🔮 {bossName} curses {target.name} with weakening magic! Multiple status effects applied!";
        }

        private string ExecuteEnrage(string bossName, BossEncounterState state)
        {
            state.hasEnraged = true;
            return $"😡 {bossName} becomes ENRAGED! Damage increased by 50% for 3 turns!";
        }

        private string ExecuteShield(BossEncounterState state)
        {
            state.isInvulnerable = true;
            state.invulnerabilityTurnsLeft = 2;
            return $"🛡️ {state.bossName} creates a magical shield! Invulnerable for 2 turns!";
        }

        private string ExecuteUltimate(string bossName, Character target, BossEncounterState state)
        {
            int massiveDamage = Mathf.RoundToInt(target.maxHealth * 0.40f); // 40% of max health
            target.TakeDamage(massiveDamage);
            state.ultimateChargeCounter = 0; // Reset charge
            return $"⚡⚡⚡ {bossName} unleashes their ULTIMATE ATTACK for {massiveDamage} damage! ⚡⚡⚡";
        }

        private string ExecuteTeleport(string bossName)
        {
            return $"✨ {bossName} teleports away, avoiding your attack!";
        }

        private string ExecuteEnvironmentalHazard(BossEncounterState state)
        {
            ActivateRandomHazard(state);
            var hazard = state.activeHazards[state.activeHazards.Count - 1];
            return $"⚠️ Environmental hazard activated: {hazard}!";
        }

        /// <summary>
        /// Activate a random environmental hazard
        /// </summary>
        private void ActivateRandomHazard(BossEncounterState state)
        {
            var allHazards = System.Enum.GetValues(typeof(EnvironmentalHazard));
            var randomHazard = (EnvironmentalHazard)allHazards.GetValue(Random.Range(0, allHazards.Length));
            
            if (!state.activeHazards.Contains(randomHazard))
            {
                state.activeHazards.Add(randomHazard);
                Debug.Log($"⚠️ New hazard: {randomHazard}");
            }
        }

        /// <summary>
        /// Process environmental hazard effects each turn
        /// </summary>
        public int ProcessEnvironmentalDamage(BossEncounterState state, Character target)
        {
            // Defensive check (v2.6.0)
            if (state == null || target == null || state.activeHazards.Count == 0)
            {
                return 0;
            }

            int totalDamage = 0;
            
            foreach (var hazard in state.activeHazards)
            {
                int damage = 0;
                switch (hazard)
                {
                    case EnvironmentalHazard.FallingRocks:
                        damage = Random.Range(5, 15);
                        Debug.Log($"🪨 Falling rocks deal {damage} damage!");
                        break;
                    case EnvironmentalHazard.FirePits:
                        damage = Random.Range(8, 20);
                        Debug.Log($"🔥 Fire pits burn for {damage} damage!");
                        break;
                    case EnvironmentalHazard.PoisonGas:
                        damage = Random.Range(3, 10);
                        Debug.Log($"☠️ Poison gas deals {damage} damage!");
                        break;
                    case EnvironmentalHazard.DarknessWave:
                        damage = Random.Range(5, 12);
                        Debug.Log($"🌑 Darkness wave drains {damage} health!");
                        break;
                    case EnvironmentalHazard.MagicVortex:
                        // Drains mana instead
                        int manaDrain = Random.Range(5, 15);
                        target.manaSystem.ConsumeMana(manaDrain);
                        Debug.Log($"🌀 Magic vortex drains {manaDrain} mana!");
                        break;
                }
                totalDamage += damage;
            }
            
            return totalDamage;
        }

        /// <summary>
        /// Update boss turn - charge ultimate, update invulnerability, etc.
        /// </summary>
        public void ProcessBossTurn(string bossName)
        {
            // Defensive check (v2.6.0)
            if (!activeEncounters.ContainsKey(bossName))
            {
                return;
            }

            var state = activeEncounters[bossName];
            
            // Charge ultimate attack
            state.ultimateChargeCounter++;
            if (state.ultimateChargeCounter >= 5)
            {
                Debug.Log($"⚡ {bossName}'s ultimate is CHARGED! Prepare for devastation!");
            }
            
            // Update invulnerability
            if (state.isInvulnerable)
            {
                state.invulnerabilityTurnsLeft--;
                if (state.invulnerabilityTurnsLeft <= 0)
                {
                    state.isInvulnerable = false;
                    Debug.Log($"🛡️ {bossName}'s shield fades!");
                }
            }
        }

        /// <summary>
        /// Check if boss can use ultimate attack
        /// </summary>
        public bool CanUseUltimate(string bossName)
        {
            if (!activeEncounters.ContainsKey(bossName))
            {
                return false;
            }

            return activeEncounters[bossName].ultimateChargeCounter >= 5;
        }

        /// <summary>
        /// Get current boss phase configuration
        /// </summary>
        public BossPhaseConfig GetCurrentPhase(string bossName)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(bossName))
            {
                return null;
            }

            if (!activeEncounters.ContainsKey(bossName) || !bossConfigurations.ContainsKey(bossName))
            {
                return null;
            }

            var state = activeEncounters[bossName];
            var phases = bossConfigurations[bossName];
            
            return phases.Find(p => p.phase == state.currentPhase);
        }

        /// <summary>
        /// End boss encounter
        /// </summary>
        public void EndBossEncounter(string bossName)
        {
            if (activeEncounters.ContainsKey(bossName))
            {
                activeEncounters.Remove(bossName);
                Debug.Log($"🎉 Boss encounter with {bossName} has ended!");
            }
        }

        /// <summary>
        /// Get encounter state for a specific boss
        /// </summary>
        public BossEncounterState GetEncounterState(string bossName)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(bossName))
            {
                return null;
            }

            return activeEncounters.ContainsKey(bossName) ? activeEncounters[bossName] : null;
        }
    }
}
