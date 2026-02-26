# ACOTAR Fantasy RPG - Enhancement Summary v2.6.16

**Version**: 2.6.16  
**Release Date**: February 26, 2026  
**Type**: Legendary Ability Shimmer Flash & `"spell_legendary"` Audio  
**Status**: Complete ✅

---

## 📋 Overview

Version 2.6.16 delivers the "What's Next" item from v2.6.15: a brief shimmer flash animation and a dedicated `"spell_legendary"` audio cue that fire whenever a player queues one of ACOTAR's rarest magic abilities (Daemati, DeathManifestation, Shadowsinger, TruthTelling, SpellCleaving, MatingBond, Seer). Common abilities continue to use only the existing scale-punch fade-in, keeping the legend tier visually distinct.

### Enhancement Philosophy

> "Legendary power deserves a legendary entrance — common spells queue quietly; ancient gifts announce themselves."

---

## 🎯 Systems Enhanced

### 1. `IsLegendaryAbility()` Static Helper 🔮

**Why Added**: There is now a clear semantic distinction between "ordinary Fae magic" (elemental manipulation, healing, shields) and "legendary gifts" (Cauldron-forged power, unique bonds, prophetic sight). Encoding this in a named helper keeps the distinction readable and easy to extend.

#### Legendary Ability Classification

| Magic Type | Rarity Rationale |
|---|---|
| `Daemati` | Rare mind magic of the Night Court inner circle |
| `DeathManifestation` | Nesta's Cauldron-forged power — one of a kind |
| `Shadowsinger` | Azriel's unique, unteachable shadow gift |
| `TruthTelling` | Mor's innate compulsion — an inborn force |
| `SpellCleaving` | Rarest offensive magic in Prythian |
| `MatingBond` | The bond itself wielded as magic |
| `Seer` | Prophetic visions — few are born with them |

#### Code Added (`CombatUI.cs` — `IsLegendaryAbility`):
```csharp
private static bool IsLegendaryAbility(MagicType ability)
{
    switch (ability)
    {
        case MagicType.Daemati:
        case MagicType.DeathManifestation:
        case MagicType.Shadowsinger:
        case MagicType.TruthTelling:
        case MagicType.SpellCleaving:
        case MagicType.MatingBond:
        case MagicType.Seer:
            return true;
        default:
            return false;
    }
}
```

---

### 2. `ShimmerPendingSpellText()` Coroutine ✨

**Why Added**: The scale-punch animation from v2.6.14 gives every queued spell a punchy entry. Legendary abilities need something *more* — a brief white flash that fades back to the spell colour twice, like a surge of raw power before the magic settles. This runs in parallel with `FadeInPendingSpellText()` using a separate coroutine handle (`spellShimmerCoroutine`), so neither animation interferes with the other.

#### Animation Sequence
```
Legendary ability queued
  → FadeInPendingSpellText()        ← existing (alpha 0→1, scale 0.75→1)
  → ShimmerPendingSpellText()       ← NEW (2× white↔spellColor flash, 60 ms each half-cycle)
      cycle 1: spellColor → white (60 ms)
               white → spellColor (60 ms)
      cycle 2: spellColor → white (60 ms)
               white → spellColor (60 ms)
      → pendingSpellText.color = spellColor (exact)
```

#### Code Added (`CombatUI.cs` — `ShimmerPendingSpellText`):
```csharp
private IEnumerator ShimmerPendingSpellText(Color spellColor)
{
    const float flashHalfDuration = 0.06f;
    const int flashCount = 2;
    for (int i = 0; i < flashCount; i++)
    {
        // flash to white, then back to spell colour
        ...
    }
    pendingSpellText.color = spellColor;
    spellShimmerCoroutine = null;
}
```

---

### 3. Legendary Shimmer Trigger in `UpdatePendingMagicIndicator()` 🎇

**Why Changed**: `UpdatePendingMagicIndicator()` is the single entry-point for all spell-queue indicator updates. Adding the legendary check here — after the existing fade-in launch — keeps the flow linear and avoids scattering the new behaviour across multiple call sites.

#### Code Changes (`CombatUI.cs` — `UpdatePendingMagicIndicator` if-branch):
```csharp
// v2.6.16: Extra shimmer flash for legendary abilities
if (IsLegendaryAbility(pendingMagicAbility.Value))
{
    if (spellShimmerCoroutine != null) StopCoroutine(spellShimmerCoroutine);
    spellShimmerCoroutine = StartCoroutine(ShimmerPendingSpellText(GetSpellColor(pendingMagicAbility.Value)));
    AudioManager.Instance?.PlayUISFXByName("spell_legendary");
}
```

#### Code Changes (`CombatUI.cs` — `UpdatePendingMagicIndicator` else-branch):
```csharp
if (spellShimmerCoroutine != null) { StopCoroutine(spellShimmerCoroutine); spellShimmerCoroutine = null; } // v2.6.16
```

---

### 4. `"spell_legendary"` Audio Cue 🔊

**Why Added**: `"ability_select"` fires for every queued ability; `"spell_legendary"` fires only for the rare tier, giving designers a dedicated slot to assign a more dramatic, weighty sound clip — e.g. a resonant chime, a Cauldron-like hum, or the whisper of shadows — that distinguishes legendary queue events from ordinary ones.

#### Code Changes (`AudioManager.ValidateExpectedSoundClips`):
```csharp
"spell_legendary", // v2.6.16: Player queues a legendary/rare magic ability
```

#### Code Changes (`SoundLibrary.EnsureDefaultUISoundEntries`):
```csharp
"spell_legendary", // v2.6.16: Player queues a legendary/rare magic ability
```

---

## 📊 Overall Impact

### Code Metrics
```
Files Modified:      2  (CombatUI.cs, AudioManager.cs)
Fields Added:        1  (spellShimmerCoroutine in CombatUI)
Methods Added:       2  (IsLegendaryAbility, ShimmerPendingSpellText in CombatUI)
Sound Entries Added: 1  ("spell_legendary" in ValidateExpectedSoundClips + EnsureDefaultUISoundEntries)
PlayUISFXByName calls added: 1 ("spell_legendary" in UpdatePendingMagicIndicator)
Lines Added:        ~55
Breaking Changes:    0
```

### Systems Improved
```
✅ CombatUI.UpdatePendingMagicIndicator — triggers shimmer + "spell_legendary" audio for legendary abilities
✅ CombatUI.UpdatePendingMagicIndicator — cancels shimmer coroutine when spell is cleared
✅ CombatUI.IsLegendaryAbility — new helper classifying 7 legendary MagicType values
✅ CombatUI.ShimmerPendingSpellText — new coroutine: 2-cycle white↔spellColor flash
✅ AudioManager.ValidateExpectedSoundClips — "spell_legendary" added to expected-clip list
✅ SoundLibrary.EnsureDefaultUISoundEntries — "spell_legendary" slot pre-populated in Inspector
```

---

## 🔧 Technical Details

### Full Spell Queue Audio + Visual Flow (v2.6.16)

```
Player clicks ability button
  → OnMagicAbilitySelected(ability)
      → AudioManager.PlayUISFXByName("ability_select")   ← v2.6.13 (all abilities)
      → pendingMagicAbility = ability
      → UpdatePendingMagicIndicator()
          → pendingSpellText.color = GetSpellColor(ability)
          → StopCoroutine(spellFadeCoroutine) if running
          → spellFadeCoroutine = StartCoroutine(FadeInPendingSpellText())
              → scale punch (SPELL_SCALE_START → 1) + alpha (0 → 1)  ← v2.6.14/v2.6.15
          → [LEGENDARY ONLY] StopCoroutine(spellShimmerCoroutine) if running
          → [LEGENDARY ONLY] spellShimmerCoroutine = StartCoroutine(ShimmerPendingSpellText())
              → 2× white↔spellColor flash (60 ms/half-cycle)         ← NEW v2.6.16
          → [LEGENDARY ONLY] AudioManager.PlayUISFXByName("spell_legendary")  ← NEW v2.6.16

Player cancels (Defend / flee / target reached)
  → pendingMagicAbility = null
  → UpdatePendingMagicIndicator() [else branch]
      → StopCoroutine + spellFadeCoroutine = null     ← v2.6.14
      → StopCoroutine + spellShimmerCoroutine = null  ← NEW v2.6.16
      → AudioManager.PlayUISFXByName("spell_clear")   ← v2.6.15
      → pendingSpellText cleared + reset              ← v2.6.14
```

---

## 🧪 Testing & Validation

### Backward Compatibility ✅
- Non-legendary abilities are completely unaffected — `IsLegendaryAbility` returns false; no shimmer, no `"spell_legendary"` audio
- `ShimmerPendingSpellText` uses a null-guard on `pendingSpellText` and terminates immediately if not present
- `AudioManager.Instance?.PlayUISFXByName("spell_legendary")` — null-conditional is safe if `AudioManager` is absent; `PlayUISFXByName` logs a warning when the clip is unassigned but does not throw
- All existing tests pass with no changes required

### Existing tests pass ✅
- `scripts/test-project.sh` passes with no changes required

---

## 🚀 What's Next (v2.6.17)

1. Wire actual `AudioClip` assets for `"spell_legendary"` (and `"synergy_trigger"`, `"ability_select"`, `"spell_clear"`) in the SoundLibrary Inspector
2. Consider adding a particle burst (star/spark emitter) around the spell indicator text for legendary abilities — a VFX counterpart to the shimmer
3. Consider migrating `GameConfig.UISettings` spell-animation constants (`SPELL_FADE_IN_DURATION`, `SPELL_SCALE_START`) and the new shimmer timing constants to a `BalanceConfig` ScriptableObject for full runtime tuning without recompilation

---

## 📝 Changelog Entry

### Added
- `CombatUI.IsLegendaryAbility(MagicType)` — classifies 7 legendary ACOTAR magic types (Daemati, DeathManifestation, Shadowsinger, TruthTelling, SpellCleaving, MatingBond, Seer)
- `CombatUI.ShimmerPendingSpellText(Color)` — 2-cycle white↔spellColor flash coroutine for legendary spell-queue indicator
- `CombatUI.spellShimmerCoroutine` — coroutine handle so shimmer can be cleanly cancelled when the spell is cleared
- `"spell_legendary"` to `AudioManager.ValidateExpectedSoundClips()` expected-clip list
- `"spell_legendary"` to `SoundLibrary.EnsureDefaultUISoundEntries()` Inspector slot

### Enhanced
- `CombatUI.UpdatePendingMagicIndicator()` — fires shimmer animation and `"spell_legendary"` audio when a legendary ability is queued
- `CombatUI.UpdatePendingMagicIndicator()` — cancels `spellShimmerCoroutine` in the else-branch (spell cleared)

### Technical
- ~55 lines of new/modified code across 2 files
- 0 breaking changes
- 100% backward compatible

---

*"To the stars who listen—and the dreams that are answered."*

**Version**: 2.6.16  
**Status**: ✅ **COMPLETE**  
**Compatibility**: ✅ **100% BACKWARDS COMPATIBLE**

---

**Enhancement Completed**: February 26, 2026  
**Total Impact**: ~55 lines across 2 files  
**Features Added**: 2 enhancements (legendary shimmer flash, `"spell_legendary"` audio)  
**Wave**: Legendary Ability Shimmer Flash & `"spell_legendary"` Audio (v2.6.16)
