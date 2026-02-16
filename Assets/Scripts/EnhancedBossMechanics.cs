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
        /// </summary>
        public BossEncounterState StartBossEncounter(string bossName)
        {
            // Defensive check (v2.6.0)
            if (string.IsNullOrEmpty(bossName))
            {
                Debug.LogWarning("EnhancedBossMechanics: Cannot start encounter with null or empty boss name");
                return null;
            }

            if (!bossConfigurations.ContainsKey(bossName))
            {
                Debug.LogWarning($"EnhancedBossMechanics: No configuration found for boss '{bossName}'");
                return null;
            }

            var encounterState = new BossEncounterState(bossName);
            activeEncounters[bossName] = encounterState;
            
            Debug.Log($"🎭 Boss Encounter Started: {bossName}");
            Debug.Log($"⚔️ Prepare yourself! This will be a multi-phase battle!");
            
            return encounterState;
        }

        /// <summary>
        /// Update boss phase based on current health percentage
        /// </summary>
        public BossPhaseConfig UpdateBossPhase(string bossName, float currentHealthPercent)
        {
            // Defensive checks (v2.6.0)
            if (!IsInitialized)
            {
                Debug.LogWarning("EnhancedBossMechanics: Cannot update phase - system not initialized");
                return null;
            }

            if (string.IsNullOrEmpty(bossName))
            {
                Debug.LogWarning("EnhancedBossMechanics: Cannot update phase with null or empty boss name");
                return null;
            }

            if (!activeEncounters.ContainsKey(bossName) || !bossConfigurations.ContainsKey(bossName))
            {
                return null;
            }

            var encounterState = activeEncounters[bossName];
            var phases = bossConfigurations[bossName];
            
            // Check for phase transition
            foreach (var phase in phases)
            {
                if (currentHealthPercent <= phase.healthThresholdPercent && 
                    encounterState.currentPhase < phase.phase)
                {
                    encounterState.currentPhase = phase.phase;
                    encounterState.phaseTransitionCount++;
                    
                    Debug.Log($"\n⚡ PHASE TRANSITION ⚡");
                    Debug.Log($"🎭 {phase.phaseTransitionMessage}");
                    Debug.Log($"⚔️ Boss entering {phase.phase}!\n");
                    
                    // Trigger phase-specific effects
                    OnPhaseTransition(encounterState, phase);
                    
                    return phase;
                }
            }
            
            // Return current phase config
            return phases.Find(p => p.phase == encounterState.currentPhase);
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
        /// Execute a boss ability
        /// </summary>
        public string ExecuteBossAbility(string bossName, BossAbility ability, Character target)
        {
            // Defensive checks (v2.6.0)
            if (string.IsNullOrEmpty(bossName) || target == null)
            {
                return "Invalid boss ability execution";
            }

            if (!activeEncounters.ContainsKey(bossName))
            {
                return $"{bossName} is not in an active encounter";
            }

            var state = activeEncounters[bossName];
            
            switch (ability)
            {
                case BossAbility.LifeDrain:
                    return ExecuteLifeDrain(bossName, target, state);
                    
                case BossAbility.AreaOfEffect:
                    return ExecuteAreaOfEffect(bossName, state);
                    
                case BossAbility.StatusCurse:
                    return ExecuteStatusCurse(bossName, target);
                    
                case BossAbility.EnrageMode:
                    return ExecuteEnrage(bossName, state);
                    
                case BossAbility.Shield:
                    return ExecuteShield(state);
                    
                case BossAbility.UltimateAttack:
                    return ExecuteUltimate(bossName, target, state);
                    
                case BossAbility.Teleport:
                    return ExecuteTeleport(bossName);
                    
                case BossAbility.SummonMinions:
                    return "Boss summons reinforcements!";
                    
                case BossAbility.EnvironmentalHazard:
                    return ExecuteEnvironmentalHazard(state);
                    
                default:
                    return $"{bossName} uses {ability}!";
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
