# ACOTAR Fantasy RPG - Enhancement Summary v2.6.10

**Version**: 2.6.10  
**Release Date**: February 21, 2026  
**Type**: Magic Combat & Synergy Feedback Update  
**Status**: Complete ✅

---

## 📋 Overview

Version 2.6.10 delivers on the "What's Next" items outlined in v2.6.9, completing four targeted enhancements: character-meet quest objective tracking, cascade combo feedback for magic attacks, named sound clip validation, and party synergy audio/visual feedback.

### Enhancement Philosophy

> "Every system should talk to every other system — automatically, with zero manual wiring."

This release focuses on closing the remaining feedback loops: magic attacks now feel as impactful as physical ones, party synergies announce themselves with sight and sound, and designers can instantly see which audio assets still need to be configured.

---

## 🎯 Systems Enhanced

### 1. Character-Meet Quest Objective Tracking 👥

**Why Enhanced**: `UnlockCharacter()` added characters to `metCharacters` and logged the event, but never advanced the associated quest objective. Players meeting Cassian or Tarquin saw no quest progress in their log.

#### Key Features:

1. **New `NotifyCharacterMeetObjective(string characterName)` Method**
   - Private method in `StoryManager`
   - Called at the end of `UnlockCharacter()` only when the character is being added for the first time
   - Gets `QuestManager` via `GameManager.Instance.GetComponent<QuestManager>()`

2. **Full Character → Objective Mapping**

| Character | Quest ID | Objective Index | Objective Text |
|---|---|---|---|
| `Tamlin`, `Lucien`, `Alis` | `main_003` | 1 | "Meet the court members: Lucien, Alis, and others" |
| `Cassian` | `book2_003` | 0 | "Meet Cassian, the General" |
| `Azriel` | `book2_003` | 1 | "Meet Azriel, the Spymaster" |
| `Mor` | `book2_003` | 2 | "Meet Mor, the Morrigan" |
| `Amren` | `book2_003` | 3 | "Meet Amren, the ancient one" |
| `Tarquin` | `book2_012` | 1 | "Meet High Lord Tarquin" |

3. **Null-Safe Design**
   - Guards on `GameManager.Instance` and `QuestManager` before any calls
   - `UpdateQuestObjectiveProgress` validates quest existence and active state internally

#### Code Changes:
```csharp
// StoryManager.cs — UnlockCharacter()
if (!metCharacters.Contains(characterName))
{
    metCharacters.Add(characterName);
    LoggingSystem.Instance?.Log(...);
    NotifyCharacterMeetObjective(characterName);  // NEW
}

// StoryManager.cs — NEW method
private void NotifyCharacterMeetObjective(string characterName)
{
    // ... null checks ...
    switch (characterName)
    {
        case "Tamlin": case "Lucien": case "Alis":
            questManager.UpdateQuestObjectiveProgress("main_003", 1); break;
        case "Cassian":
            questManager.UpdateQuestObjectiveProgress("book2_003", 0); break;
        // ... all 8 characters
    }
}
```

---

### 2. Cascade Combo Feedback for Magic Attacks ⚡

**Why Enhanced**: In v2.6.9, `CombatSystem.CalculateMagicAttack` already called `UpdateCombo()` and could set `lastAttackWasCascade = true`, but `CombatUI.OnEnemyTargeted()` only executed `PlayerPhysicalAttack()` and only checked the cascade flag for physical attacks. Magic cascades were silently lost.

#### Key Features:

1. **Pending Magic Ability State**
   - New `private MagicType? pendingMagicAbility = null` field in `CombatUI`
   - `OnMagicAbilitySelected()` now stores the chosen ability instead of just logging it

2. **End-to-End Magic Attack Flow**
   - `OnEnemyTargeted()` checks `pendingMagicAbility.HasValue` first
   - If set: clears it, calls `PlayerMagicAttack(enemy, ability)`, logs the cast, then checks cascade
   - If not set: falls through to the existing physical attack path (unchanged)

3. **Cascade Feedback on Magic**
   - Same `ScreenEffectsManager.Instance?.AlertPulse()` + `AudioManager.Instance?.PlayUISFXByName("combo_cascade")` pair as physical attacks

4. **Stale-State Protection**
   - `OnDefendClicked()` now clears `pendingMagicAbility = null` so switching to defend never causes a stale magic ability to fire on the next enemy click

#### Code Changes:
```csharp
// CombatUI.cs
private MagicType? pendingMagicAbility = null;  // NEW field

// OnMagicAbilitySelected — now stores the ability
pendingMagicAbility = ability;  // NEW

// OnEnemyTargeted — new branch before physical attack
if (pendingMagicAbility.HasValue)
{
    MagicType ability = pendingMagicAbility.Value;
    pendingMagicAbility = null;
    currentEncounter.PlayerMagicAttack(enemy, ability);
    AddCombatLogEntry($"You cast {ability} at {enemy.characterName}!");
    if (CombatSystem.WasLastAttackCascade())
    {
        ScreenEffectsManager.Instance?.AlertPulse();
        AudioManager.Instance?.PlayUISFXByName("combo_cascade");
    }
}
else { /* existing physical attack code */ }

// OnDefendClicked — stale-state protection
pendingMagicAbility = null;  // NEW
```

---

### 3. Named Sound Clip Validation 🔊

**Why Enhanced**: Named sound clips (`"confirm_open"`, `"quest_start"`, etc.) were introduced in v2.6.9 but had no runtime indicator when a clip was missing from `SoundLibrary.uiSounds`. Silent failures were hard to diagnose.

#### Key Features:

1. **`ValidateExpectedSoundClips()` Method**
   - Called in `AudioManager.Start()`, after `LoadAudioSettings()` and `ApplyVolumeSettings()`
   - Iterates the 8 expected UI sound names and calls `soundLibrary.GetUISFXClip(name)`
   - Logs a `Debug.LogWarning` for every missing clip

2. **Expected Clips Documented in Code**
   ```
   "confirm_open"    — CombatUI / InventoryUI confirmation dialog opens
   "confirm_yes"     — Player accepts a confirmation dialog
   "confirm_no"      — Player cancels a confirmation dialog
   "quest_start"     — New quest started
   "quest_complete"  — Quest completed
   "quest_progress"  — Quest objective advanced
   "combo_cascade"   — Physical or magic cascade combo milestone
   "synergy_trigger" — Party synergy combo activated
   ```

3. **Null-Safe**
   - If `soundLibrary` is not assigned, logs a single warning and returns early

#### Code Changes:
```csharp
// AudioManager.cs — Start()
private void Start()
{
    LoadAudioSettings();
    ApplyVolumeSettings();
    ValidateExpectedSoundClips();  // NEW
}

// AudioManager.cs — NEW method
private void ValidateExpectedSoundClips()
{
    if (soundLibrary == null)
    {
        Debug.LogWarning("[AudioManager] SoundLibrary is not assigned...");
        return;
    }
    string[] expectedUISounds = { "confirm_open", "confirm_yes", ... };
    foreach (string clipName in expectedUISounds)
    {
        if (soundLibrary.GetUISFXClip(clipName) == null)
            Debug.LogWarning($"[AudioManager] Expected UI sound clip '{clipName}' is not registered...");
    }
}
```

---

### 4. Party Synergy Audio/Visual Feedback ✨

**Why Enhanced**: The cascade combo triggered `ScreenEffectsManager.AlertPulse()` and played `"combo_cascade"` for dramatic effect. Party synergies — which represent equally significant team moments — had no equivalent screen or audio feedback.

#### Key Features:

1. **Screen Pulse on Synergy**
   - `ScreenEffectsManager.Instance?.AlertPulse()` fires when `TriggerSynergy()` finds a matching active synergy
   - Same dramatic effect as the cascade combo

2. **Audio on Synergy**
   - `AudioManager.Instance?.PlayUISFXByName("synergy_trigger")` fires alongside the visual
   - `"synergy_trigger"` is now included in the `ValidateExpectedSoundClips()` list

3. **Trigger Location**
   - In `PartySynergySystem.TriggerSynergy()`, immediately after `activeSyn.timesTriggered++` and before achievement tracking

4. **Null-Safe**
   - Both calls use the `?.` null-conditional pattern — no effect when managers are absent

#### Code Changes:
```csharp
// PartySynergySystem.cs — TriggerSynergy()
activeSyn.timesTriggered++;

// v2.6.10: Multi-sense feedback for synergy combo — matching cascade style
ScreenEffectsManager.Instance?.AlertPulse();
AudioManager.Instance?.PlayUISFXByName("synergy_trigger");

// Track for achievements ...
```

---

## 📊 Overall Impact

### Code Metrics
```
Files Modified:    4
Methods Added:     2 (NotifyCharacterMeetObjective, ValidateExpectedSoundClips)
Lines Added:       ~75
New Fields:        1 (pendingMagicAbility in CombatUI)
Breaking Changes:  0
```

### Systems Improved
```
✅ StoryManager:        Auto quest-objective tracking on character meet
✅ CombatUI:            Magic attack fully wired to enemy targeting + cascade feedback
✅ AudioManager:        Designer-facing validation for all named UI sound clips
✅ PartySynergySystem:  Screen pulse + audio on every synergy trigger
```

---

## 🔧 Technical Details

### Null-Safe Pattern (used throughout)
```csharp
ScreenEffectsManager.Instance?.AlertPulse();
AudioManager.Instance?.PlayUISFXByName("synergy_trigger");
```

### Magic Attack Flow (v2.6.10)
```
1. Player clicks Magic button       → OnMagicClicked() → DisplayMagicAbilities()
2. Player selects ability           → OnMagicAbilitySelected() → pendingMagicAbility = ability
3. Player clicks enemy panel        → OnEnemyTargeted()
4.   pendingMagicAbility.HasValue?  → YES: PlayerMagicAttack(enemy, ability)
                                           check WasLastAttackCascade()
                                     NO:  PlayerPhysicalAttack(enemy)
                                           check WasLastAttackCascade()
```

### Character-Meet Flow (v2.6.10)
```
UnlockCharacter("Cassian")
  → metCharacters.Add("Cassian")
  → NotifyCharacterMeetObjective("Cassian")
    → questManager.UpdateQuestObjectiveProgress("book2_003", 0)
      → NotificationSystem.ShowQuest("Quest Progress: The Inner Circle", "✓ Meet Cassian, the General (1/4)")
      → AudioManager.PlayUISFXByName("quest_progress")
```

---

## 🧪 Testing & Validation

### Backward Compatibility ✅
- `pendingMagicAbility` defaults to `null` — physical attack behavior completely unchanged when no magic ability is pending
- `NotifyCharacterMeetObjective` null-checks `GameManager.Instance` and `QuestManager`
- `ValidateExpectedSoundClips` only logs warnings — no gameplay effect
- `TriggerSynergy` audio/visual calls null-checked — safe without managers present

### All existing tests pass ✅
- `scripts/test-project.sh` passes with no changes required

---

## 🚀 What's Next (v2.6.11)

1. Extend `NotifyCharacterMeetObjective` to cover Book 3 characters (Thesan, Helion, Beron, etc.) once their quest objectives are defined
2. Add `"synergy_trigger"` audio asset to the default SoundLibrary configuration
3. Show the synergy name in a brief on-screen notification (e.g., `"⚡ Cassian + Azriel Synergy!"`) alongside the screen pulse
4. Add `pendingMagicAbility` indicator to the CombatUI HUD so players know a spell is queued

---

## 📝 Changelog Entry

### Added
- `NotifyCharacterMeetObjective()` in `StoryManager` — automatically calls `UpdateQuestObjectiveProgress()` when key characters are first met
- `pendingMagicAbility` field in `CombatUI` — stores selected magic ability until an enemy target is chosen
- `ValidateExpectedSoundClips()` in `AudioManager` — logs warnings for any of the 8 expected named UI clips missing from `SoundLibrary`
- `ScreenEffectsManager.AlertPulse()` + `AudioManager.PlayUISFXByName("synergy_trigger")` in `PartySynergySystem.TriggerSynergy()`

### Enhanced
- `StoryManager.UnlockCharacter()`: calls `NotifyCharacterMeetObjective()` on first character meet
- `CombatUI.OnMagicAbilitySelected()`: stores selected ability in `pendingMagicAbility` for deferred execution
- `CombatUI.OnEnemyTargeted()`: executes magic or physical attack based on `pendingMagicAbility` state; cascade feedback applies to both
- `CombatUI.OnDefendClicked()`: clears `pendingMagicAbility` to prevent stale magic selection
- `AudioManager.Start()`: calls `ValidateExpectedSoundClips()` after audio settings are loaded

### Technical
- ~75 lines of new code
- 4 files modified
- 0 breaking changes
- 100% backward compatible

---

*"To the stars who listen—and the dreams that are answered."*

**Version**: 2.6.10  
**Status**: ✅ **COMPLETE**  
**Compatibility**: ✅ **100% BACKWARDS COMPATIBLE**

---

**Enhancement Completed**: February 21, 2026  
**Total Impact**: ~75 lines of polish & integration code  
**Features Added**: 4 enhancements (Character Tracking, Magic Cascade, Sound Validation, Synergy Feedback)  
**Wave**: Magic Combat & Synergy Feedback (v2.6.10)
