# ACOTAR Fantasy RPG - Enhancement Summary v2.6.17

**Version**: 2.6.17  
**Release Date**: February 26, 2026  
**Type**: Legendary Shimmer Timing Constants → `GameConfig.UISettings`  
**Status**: Complete ✅

---

## 📋 Overview

Version 2.6.17 delivers the "What's Next" item from v2.6.16: the shimmer animation timing constants (`flashHalfDuration` and `flashCount`) that were hard-coded as local `const` values inside `ShimmerPendingSpellText()` are now centralised in `GameConfig.UISettings`. This places all spell-queue indicator animation knobs — fade duration, punch scale, and shimmer timing — in a single designer-visible location, removing the last buried tuning value from `CombatUI`.

### Enhancement Philosophy

> "All animation timing belongs in one place — designers shouldn't hunt through gameplay coroutines to tune a flash duration."

---

## 🎯 Systems Enhanced

### 1. Shimmer Timing Constants Added to `GameConfig.UISettings` 🔧

**Why Enhanced**: `flashHalfDuration` (0.06 s) and `flashCount` (2) were `private const` values buried inside the `ShimmerPendingSpellText` coroutine in `CombatUI`. Moving them to `GameConfig.UISettings` places them alongside `SPELL_FADE_IN_DURATION` and `SPELL_SCALE_START` (added in v2.6.15), completing the set of spell-queue indicator animation constants in one designer-visible file. Designers can now tune every aspect of the spell indicator animation — fade-in, punch scale, shimmer speed, and shimmer count — without touching any gameplay code.

#### Constants Added to `GameConfig.UISettings`

| Constant | Value | Description |
|---|---|---|
| `SPELL_SHIMMER_HALF_DURATION` | `0.06f` | Seconds per half-cycle of the legendary shimmer flash (spellColor→white or white→spellColor) |
| `SPELL_SHIMMER_FLASH_COUNT` | `2` | Number of full white↔spellColor cycles in the shimmer animation |

#### Code Changes (`GameConfig.cs` — `UISettings`):
```csharp
public const float SPELL_SHIMMER_HALF_DURATION = 0.06f; // v2.6.17: Seconds per half-cycle of the legendary shimmer flash
public const int   SPELL_SHIMMER_FLASH_COUNT = 2;       // v2.6.17: Number of full white↔spellColor cycles in the shimmer
```

#### Code Changes (`CombatUI.cs` — `ShimmerPendingSpellText`):
```csharp
// Before (v2.6.16):
const float flashHalfDuration = 0.06f;
const int flashCount = 2;
for (int i = 0; i < flashCount; i++) { ... elapsed < flashHalfDuration ... }

// After (v2.6.17): reference GameConfig.UISettings directly
for (int i = 0; i < GameConfig.UISettings.SPELL_SHIMMER_FLASH_COUNT; i++)
{
    while (elapsed < GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION) { ... }
}
```

---

## 📊 Overall Impact

### Code Metrics
```
Files Modified:      2  (GameConfig.cs, CombatUI.cs)
Constants Added:     2  (SPELL_SHIMMER_HALF_DURATION, SPELL_SHIMMER_FLASH_COUNT in GameConfig.UISettings)
Constants Removed:   2  (same values removed from ShimmerPendingSpellText local scope)
Lines Added:        ~4
Breaking Changes:    0
```

### Systems Improved
```
✅ GameConfig.UISettings — SPELL_SHIMMER_HALF_DURATION and SPELL_SHIMMER_FLASH_COUNT added as designer-visible constants
✅ CombatUI.ShimmerPendingSpellText — references GameConfig.UISettings instead of local const values
```

---

## 🔧 Technical Details

### Complete Spell-Queue Indicator Animation Constants (v2.6.17)

All spell indicator animation parameters are now in `GameConfig.UISettings`:

| Constant | Value | Added | Purpose |
|---|---|---|---|
| `SPELL_FADE_IN_DURATION` | `0.3f` | v2.6.15 | Duration of the fade-in animation |
| `SPELL_SCALE_START` | `0.75f` | v2.6.15 | Starting scale of the punch animation |
| `SPELL_SHIMMER_HALF_DURATION` | `0.06f` | **v2.6.17** | Half-cycle duration of the legendary shimmer |
| `SPELL_SHIMMER_FLASH_COUNT` | `2` | **v2.6.17** | Number of shimmer cycles |

### Spell Queue Animation Flow (v2.6.17 — unchanged behaviour, improved discoverability)
```
Player clicks legendary ability button
  → OnMagicAbilitySelected(ability)
      → AudioManager.PlayUISFXByName("ability_select")   ← v2.6.13
      → UpdatePendingMagicIndicator()
          → spellFadeCoroutine = StartCoroutine(FadeInPendingSpellText())
              → scale: GameConfig.UISettings.SPELL_SCALE_START → 1   ← v2.6.15
              → alpha: 0 → 1 over SPELL_FADE_IN_DURATION             ← v2.6.15
          → spellShimmerCoroutine = StartCoroutine(ShimmerPendingSpellText())
              → SPELL_SHIMMER_FLASH_COUNT cycles of:                  ← v2.6.17 (was local const)
                  spellColor → white over SPELL_SHIMMER_HALF_DURATION ← v2.6.17 (was local const)
                  white → spellColor over SPELL_SHIMMER_HALF_DURATION ← v2.6.17 (was local const)
          → AudioManager.PlayUISFXByName("spell_legendary")           ← v2.6.16
```

---

## 🧪 Testing & Validation

### Backward Compatibility ✅
- `GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION` and `SPELL_SHIMMER_FLASH_COUNT` are `const` — identical compile-time values to the removed local constants; no runtime behaviour change
- All existing tests pass with no changes required

### Existing tests pass ✅
- `scripts/test-project.sh` passes with no changes required

---

## 🚀 What's Next (v2.6.18)

1. Wire actual `AudioClip` assets for `"spell_legendary"`, `"synergy_trigger"`, `"ability_select"`, and `"spell_clear"` in the SoundLibrary Inspector
2. Add a particle burst (star/spark emitter) around the spell indicator text for legendary abilities — a VFX counterpart to the shimmer
3. Consider a `[Header("Spell Queue Animation")]` attribute group in `GameConfig.UISettings` to improve Unity Inspector organisation

---

## 📝 Changelog Entry

### Added
- `GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION` — centralised constant (0.06 s) for the legendary shimmer flash half-cycle duration
- `GameConfig.UISettings.SPELL_SHIMMER_FLASH_COUNT` — centralised constant (2) for the number of shimmer cycles

### Enhanced
- `CombatUI.ShimmerPendingSpellText()` — references `GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION` and `GameConfig.UISettings.SPELL_SHIMMER_FLASH_COUNT` instead of local constants

### Removed
- Local `const float flashHalfDuration` from `CombatUI.ShimmerPendingSpellText` — superseded by `GameConfig.UISettings.SPELL_SHIMMER_HALF_DURATION`
- Local `const int flashCount` from `CombatUI.ShimmerPendingSpellText` — superseded by `GameConfig.UISettings.SPELL_SHIMMER_FLASH_COUNT`

### Technical
- ~4 lines of new/modified code across 2 files
- 0 breaking changes
- 100% backward compatible

---

*"To the stars who listen—and the dreams that are answered."*

**Version**: 2.6.17  
**Status**: ✅ **COMPLETE**  
**Compatibility**: ✅ **100% BACKWARDS COMPATIBLE**

---

**Enhancement Completed**: February 26, 2026  
**Total Impact**: ~4 lines across 2 files  
**Features Added**: 2 constants (`SPELL_SHIMMER_HALF_DURATION`, `SPELL_SHIMMER_FLASH_COUNT` in `GameConfig.UISettings`)  
**Wave**: Legendary Shimmer Timing Constants → `GameConfig.UISettings` (v2.6.17)
