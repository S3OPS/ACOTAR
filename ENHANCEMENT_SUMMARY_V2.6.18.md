# ACOTAR Fantasy RPG - Enhancement Summary v2.6.18

**Version**: 2.6.18  
**Release Date**: February 26, 2026  
**Type**: Legendary Spell Particle Burst VFX & Spell Queue Animation Header Group  
**Status**: Complete ✅

---

## 📋 Overview

Version 2.6.18 delivers the two code-facing "What's Next" items from v2.6.17: a star/spark particle burst (`legendarySpellBurst`) now fires alongside the shimmer flash when a legendary ability is queued — completing the audio + animation + VFX triple-feedback loop for legendary spells — and the spell-queue animation constants in `GameConfig.UISettings` are gathered under a labelled `// --- Spell Queue Animation ---` comment group so designers can find every animation knob in one place.

### Enhancement Philosophy

> "Legendary power deserves a legendary entrance — the shimmer captures the eye, the sound hits the ear, and now the particle burst fills the space around the spell name with stars."

---

## 🎯 Systems Enhanced

### 1. Legendary Spell Particle Burst VFX ✨

**Why Added**: The shimmer animation (v2.6.16) and `"spell_legendary"` audio cue give a visual and auditory signal when a legendary ability is queued. A particle burst provides the missing spatial VFX layer — a brief star/spark emitter that fires around the `pendingSpellText` object and instantly communicates "something extraordinary is queued" to the player. It uses Unity's existing `ParticleSystem.Emit()` API to fire a one-shot burst of `LEGENDARY_PARTICLE_BURST_COUNT` particles without needing a separate coroutine.

#### Inspector Field Added (`CombatUI.cs`)

```csharp
[Header("Legendary VFX")]
public ParticleSystem legendarySpellBurst; // v2.6.18: Star/spark emitter that fires when a legendary ability is queued
```

#### Code Changes (`CombatUI.cs` — `UpdatePendingMagicIndicator` if-branch):

```csharp
// v2.6.16: Extra shimmer flash for legendary abilities
if (IsLegendaryAbility(pendingMagicAbility.Value))
{
    if (spellShimmerCoroutine != null) StopCoroutine(spellShimmerCoroutine);
    spellShimmerCoroutine = StartCoroutine(ShimmerPendingSpellText(GetSpellColor(pendingMagicAbility.Value)));
    AudioManager.Instance?.PlayUISFXByName("spell_legendary");

    // v2.6.18: Particle burst VFX counterpart to the shimmer
    if (legendarySpellBurst != null)
    {
        legendarySpellBurst.Emit(GameConfig.UISettings.LEGENDARY_PARTICLE_BURST_COUNT);
    }
}
```

#### Code Changes (`CombatUI.cs` — `UpdatePendingMagicIndicator` else-branch):

```csharp
if (legendarySpellBurst != null && legendarySpellBurst.isPlaying) legendarySpellBurst.Stop(); // v2.6.18: stop burst when spell cleared
```

#### Designer Setup Notes

- Attach a `ParticleSystem` component to the `pendingSpellText` GameObject (or a child of it) and wire it to the `legendarySpellBurst` Inspector slot
- Recommended particle settings: `Play On Awake = false`, `Stop Action = None`, short lifetime (0.4–0.8 s), star or sparkle shape, colours matching the spell colour for maximum lore accuracy
- Leave `legendarySpellBurst` unassigned to silently opt out of the burst — non-legendary abilities are unaffected regardless

---

### 2. Spell Queue Animation Header Group in `GameConfig.UISettings` 🔧

**Why Added**: The four spell-queue animation constants (`SPELL_FADE_IN_DURATION`, `SPELL_SCALE_START`, `SPELL_SHIMMER_HALF_DURATION`, `SPELL_SHIMMER_FLASH_COUNT`) and the new `LEGENDARY_PARTICLE_BURST_COUNT` were scattered in the `UISettings` block with no visual grouping. Adding a `// --- Spell Queue Animation ---` comment separator puts every animation knob for the spell-indicator under one labelled group, mirroring the approach suggested in v2.6.17 for a `[Header]` grouping (which is not applicable to a static class with `const` members).

#### Code Changes (`GameConfig.cs` — `UISettings`):

```csharp
// --- Spell Queue Animation ---                          // v2.6.18: grouped for Inspector discoverability
public const float SPELL_FADE_IN_DURATION = 0.3f;       // v2.6.15: Spell-queue indicator fade-in duration (seconds)
public const float SPELL_SCALE_START = 0.75f;           // v2.6.15: Spell-queue indicator punch-animation start scale
public const float SPELL_SHIMMER_HALF_DURATION = 0.06f; // v2.6.17: Seconds per half-cycle of the legendary shimmer flash
public const int   SPELL_SHIMMER_FLASH_COUNT = 2;       // v2.6.17: Number of full white↔spellColor cycles in the shimmer
public const int   LEGENDARY_PARTICLE_BURST_COUNT = 12; // v2.6.18: Number of particles emitted per legendary-ability burst
```

---

## 📊 Overall Impact

### Code Metrics
```
Files Modified:      2  (CombatUI.cs, GameConfig.cs)
Fields Added:        1  (legendarySpellBurst in CombatUI)
Constants Added:     1  (LEGENDARY_PARTICLE_BURST_COUNT in GameConfig.UISettings)
Lines Added:        ~15
Breaking Changes:    0
```

### Systems Improved
```
✅ CombatUI — legendarySpellBurst particle field + emit/stop calls in UpdatePendingMagicIndicator
✅ GameConfig.UISettings — Spell Queue Animation comment group + LEGENDARY_PARTICLE_BURST_COUNT constant
```

---

## 🔧 Technical Details

### Full Spell Queue Audio + Visual + VFX Flow (v2.6.18)

```
Player clicks legendary ability button
  → OnMagicAbilitySelected(ability)
      → AudioManager.PlayUISFXByName("ability_select")          ← v2.6.13 (all abilities)
      → pendingMagicAbility = ability
      → UpdatePendingMagicIndicator()
          → pendingSpellText.color = GetSpellColor(ability)
          → StopCoroutine(spellFadeCoroutine) if running
          → spellFadeCoroutine = StartCoroutine(FadeInPendingSpellText())
              → scale punch (SPELL_SCALE_START → 1) + alpha (0 → 1)        ← v2.6.14/2.6.15
          → [LEGENDARY ONLY] StopCoroutine(spellShimmerCoroutine) if running
          → [LEGENDARY ONLY] spellShimmerCoroutine = StartCoroutine(ShimmerPendingSpellText())
              → 2× white↔spellColor flash (SPELL_SHIMMER_HALF_DURATION/cycle) ← v2.6.16/2.6.17
          → [LEGENDARY ONLY] AudioManager.PlayUISFXByName("spell_legendary")  ← v2.6.16
          → [LEGENDARY ONLY] legendarySpellBurst.Emit(LEGENDARY_PARTICLE_BURST_COUNT) ← NEW v2.6.18

Player cancels (Defend / flee / target reached)
  → pendingMagicAbility = null
  → UpdatePendingMagicIndicator() [else branch]
      → StopCoroutine + spellFadeCoroutine = null              ← v2.6.14
      → StopCoroutine + spellShimmerCoroutine = null           ← v2.6.16
      → legendarySpellBurst.Stop() if playing                  ← NEW v2.6.18
      → AudioManager.PlayUISFXByName("spell_clear")            ← v2.6.15
      → pendingSpellText cleared + reset                       ← v2.6.14
```

### Complete Spell-Queue Indicator Constants (v2.6.18)

All spell indicator animation parameters are now in `GameConfig.UISettings` under the `// --- Spell Queue Animation ---` group:

| Constant | Value | Added | Purpose |
|---|---|---|---|
| `SPELL_FADE_IN_DURATION` | `0.3f` | v2.6.15 | Duration of the fade-in animation |
| `SPELL_SCALE_START` | `0.75f` | v2.6.15 | Starting scale of the punch animation |
| `SPELL_SHIMMER_HALF_DURATION` | `0.06f` | v2.6.17 | Half-cycle duration of the legendary shimmer |
| `SPELL_SHIMMER_FLASH_COUNT` | `2` | v2.6.17 | Number of shimmer cycles |
| `LEGENDARY_PARTICLE_BURST_COUNT` | `12` | **v2.6.18** | Particles emitted per legendary burst |

---

## 🧪 Testing & Validation

### Backward Compatibility ✅
- `legendarySpellBurst` is null by default — all existing scenes and non-legendary abilities are completely unaffected
- `legendarySpellBurst.Emit()` and `legendarySpellBurst.Stop()` are guarded by null checks
- `LEGENDARY_PARTICLE_BURST_COUNT` is a `const` — identical compile-time constant, no runtime behaviour change unless the field is wired in the Inspector
- All existing tests pass with no changes required

### Existing tests pass ✅
- `scripts/test-project.sh` passes with no changes required

---

## 🚀 What's Next (v2.6.19)

1. Wire actual `AudioClip` assets for `"spell_legendary"`, `"synergy_trigger"`, `"ability_select"`, and `"spell_clear"` in the SoundLibrary Inspector — the last remaining item from the v2.6.17 "What's Next" list
2. Wire a star/sparkle `ParticleSystem` asset to `CombatUI.legendarySpellBurst` in the MainScene — making the VFX visible to players
3. Consider tinting `legendarySpellBurst` particle start-colour to match `GetSpellColor()` at the moment of queuing — per-ability particle colours would reinforce the existing colour-coded text

---

## 📝 Changelog Entry

### Added
- `CombatUI.legendarySpellBurst` — `ParticleSystem` inspector field under `[Header("Legendary VFX")]`; fires a one-shot burst when a legendary ability is queued
- `GameConfig.UISettings.LEGENDARY_PARTICLE_BURST_COUNT` — centralised constant (12) for the burst particle count

### Enhanced
- `CombatUI.UpdatePendingMagicIndicator()` — emits `legendarySpellBurst` when a legendary ability is queued
- `CombatUI.UpdatePendingMagicIndicator()` — stops `legendarySpellBurst` in the else-branch when spell is cleared
- `GameConfig.UISettings` — spell queue animation constants gathered under a `// --- Spell Queue Animation ---` comment group; `LEGENDARY_PARTICLE_BURST_COUNT` added to that group

### Technical
- ~15 lines of new/modified code across 2 files
- 0 breaking changes
- 100% backward compatible

---

*"To the stars who listen—and the dreams that are answered."*

**Version**: 2.6.18  
**Status**: ✅ **COMPLETE**  
**Compatibility**: ✅ **100% BACKWARDS COMPATIBLE**

---

**Enhancement Completed**: February 26, 2026  
**Total Impact**: ~15 lines across 2 files  
**Features Added**: 2 enhancements (legendary particle burst VFX, spell animation header group)  
**Wave**: Legendary Spell Particle Burst VFX & Spell Queue Animation Header Group (v2.6.18)
