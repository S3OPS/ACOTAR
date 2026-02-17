# 🎵 Phase 9: Audio & Atmosphere - Planning Document

**Phase**: 9 of 13  
**Priority**: 🔥 **HIGH PRIORITY**  
**Status**: 📋 **PLANNED**  
**Estimated Time**: 3-4 hours  
**Focus**: Bring Prythian to life with sound

---

## 🎯 Overview

Phase 9 adds the critical audio layer that transforms the game from a visual experience into an immersive journey through Prythian. This phase implements sound effects, ambient soundscapes, and background music to enhance atmosphere and player engagement.

> **Philosophy**: "Audio is 50% of the experience. Great sound makes a good game unforgettable."

---

## ✅ Phase 9 Objectives

### 1. Sound Effects System 🔊

**Goal**: Implement comprehensive SFX for all player actions and events.

#### Audio Categories:

##### UI Sounds:
- [ ] **Menu Navigation**
  - Button click/hover sounds
  - Panel open/close sounds
  - Tab switch sounds
  - Notification pings

- [ ] **Inventory**
  - Item pickup sound
  - Item drop sound
  - Equipment equip sound
  - Consumable use sound
  - Crafting complete sound

- [ ] **Quest System**
  - Quest accepted sound
  - Quest completed sound (with fanfare)
  - Objective completed sound
  - Quest updated sound

##### Combat Sounds:
- [ ] **Player Actions**
  - Physical attack swing
  - Magic cast sounds (per ability type)
  - Defend/block sound
  - Dodge/evade sound
  - Critical hit sound (enhanced)
  - Flee attempt sound

- [ ] **Damage & Effects**
  - Player takes damage sound
  - Player death sound
  - Enemy hit sounds (varied)
  - Enemy death sounds (per type)
  - Heal effect sound

- [ ] **Combat Flow**
  - Turn start sound
  - Victory fanfare
  - Defeat sound
  - Loot drop sound

##### Character Sounds:
- [ ] **Character Events**
  - Level up fanfare (triumphant)
  - Ability learned sound
  - Transformation sound (human to High Fae)
  - Stat increase sounds

- [ ] **Companion System**
  - Companion joins sound
  - Loyalty increase sound
  - Loyalty decrease sound

##### World Sounds:
- [ ] **Travel & Exploration**
  - Winnowing sound (magical teleport)
  - Location arrival sound
  - Door open/close sounds
  - Footsteps (optional)

- [ ] **Events**
  - Save game sound
  - Load game sound
  - Time passage sound

**Estimated Time**: 1 hour

---

### 2. Ambient Soundscapes 🌙

**Goal**: Create immersive location-specific ambient audio.

#### Location Soundscapes:

##### Court-Specific Ambience:
- [ ] **Spring Court**
  - Birds chirping
  - Gentle breeze through leaves
  - Distant waterfall
  - Nature sounds

- [ ] **Summer Court (Adriata)**
  - Ocean waves
  - Seagulls
  - Gentle sea breeze
  - Distant boats

- [ ] **Autumn Court**
  - Rustling leaves
  - Crackling fire (distant)
  - Wind through trees
  - Forest ambience

- [ ] **Winter Court**
  - Howling wind
  - Snow crunching
  - Ice cracking
  - Cold atmosphere

- [ ] **Night Court (Velaris)**
  - City ambience (distant)
  - Gentle magical hum
  - Night sounds
  - Peaceful atmosphere

- [ ] **Night Court (Hewn City)**
  - Echoing cavern sounds
  - Dripping water
  - Distant voices
  - Ominous atmosphere

- [ ] **Dawn Court**
  - Morning birds
  - Gentle wind
  - Peaceful ambience
  - Healing atmosphere

- [ ] **Day Court**
  - Bright, warm atmosphere
  - Gentle energy hum
  - Library sounds (pages)
  - Scholarly ambience

##### Special Locations:
- [ ] **Under the Mountain**
  - Oppressive silence
  - Distant screams (subtle)
  - Dripping water echoes
  - Heavy atmosphere

- [ ] **Human Lands**
  - Rural sounds
  - Farm animals (distant)
  - Village life
  - Simple, earthy sounds

- [ ] **The Wall**
  - Magical barrier hum
  - Eerie silence
  - Wind whistling
  - Foreboding atmosphere

**Estimated Time**: 1 hour

---

### 3. Background Music System 🎼

**Goal**: Implement dynamic music system with context-aware tracks.

#### Music Categories:

##### Menu Music:
- [ ] **Main Menu Theme**
  - Sweeping orchestral
  - Hints of all courts
  - Epic but welcoming
  - Loop seamlessly

##### Exploration Music:
- [ ] **Peaceful Exploration**
  - Spring Court theme (nature, hope)
  - Summer Court theme (ocean, freedom)
  - Night Court theme (mystery, power)
  - Velaris theme (beauty, home)

- [ ] **Tense Exploration**
  - Under the Mountain theme (oppressive)
  - Hewn City theme (dark, threatening)
  - Dangerous areas theme

##### Combat Music:
- [ ] **Standard Combat**
  - Fast-paced, energetic
  - Builds tension
  - Loop-friendly
  - Intensity layers

- [ ] **Boss Battles**
  - Epic orchestral
  - High intensity
  - Unique per major boss:
    - Middengard Wyrm theme
    - Naga theme
    - Amarantha final battle theme

##### Story Music:
- [ ] **Emotional Scenes**
  - Love theme (Feyre & Tamlin/Rhysand)
  - Tragedy theme (Clare Beddor)
  - Triumph theme (Breaking the curse)
  - Hope theme (Becoming High Fae)

- [ ] **Special Events**
  - Calanmai theme (primal, fire)
  - Starfall theme (magical, beautiful)
  - The Trials themes (suspenseful)

#### Music System Features:
- [ ] Dynamic volume adjustment
- [ ] Smooth crossfading between tracks
- [ ] Context-aware music switching
- [ ] Combat intensity system
- [ ] Music loops without jarring cuts
- [ ] Integration with Settings UI

**Estimated Time**: 1.5 hours

---

### 4. Audio Manager System 🎛️

**Goal**: Implement professional audio management system.

#### Core Features:
- [ ] **AudioManager.cs**
  - Centralized audio control
  - Volume management per category
  - 2D and 3D sound support
  - Audio pooling for efficiency

- [ ] **Audio Categories**
  ```csharp
  public enum AudioCategory
  {
      Master,
      Music,
      SFX,
      Ambient,
      UI,
      Voice // Future
  }
  ```

- [ ] **Audio Control**
  - Play sound at point
  - Play UI sound
  - Play ambient loop
  - Play music track
  - Stop all sounds
  - Pause/resume

- [ ] **Advanced Features**
  - Fade in/out
  - Crossfade between tracks
  - Pitch variation for variety
  - Priority system
  - Audio pooling

#### Integration Points:
- [ ] Settings UI (already has audio sliders)
- [ ] All game systems trigger audio
- [ ] Save audio settings
- [ ] Respect mute toggle

**Estimated Time**: 0.5 hours

---

## 📊 Phase 9 Deliverables

### Code:
1. **AudioManager.cs** - Central audio management
2. **MusicManager.cs** - Music system controller
3. **SoundEffects.cs** - SFX collection and player
4. **AmbienceManager.cs** - Ambient sound controller
5. **AudioEvents.cs** - Audio event integration

### Assets:
1. **Sound Effects Library** (~50 SFX files)
2. **Music Tracks** (~15 music tracks)
3. **Ambient Loops** (~10 ambient tracks)
4. **AudioMixer** - Unity mixer setup

### Documentation:
1. **Phase 9 completion report**
2. **Audio asset list**
3. **Music track guide**
4. **Audio integration guide**

---

## 🎵 Audio Specifications

### Technical Requirements:

#### Sound Effects:
- **Format**: WAV or OGG
- **Sample Rate**: 44.1 kHz
- **Bit Depth**: 16-bit
- **Duration**: 0.1s - 3s typically
- **Size**: Keep under 100KB each

#### Music:
- **Format**: OGG Vorbis (compressed)
- **Sample Rate**: 44.1 kHz
- **Bit Rate**: 128-192 kbps
- **Duration**: 1-3 minutes (loopable)
- **Size**: Aim for under 5MB each

#### Ambient:
- **Format**: OGG Vorbis
- **Sample Rate**: 44.1 kHz
- **Bit Rate**: 96-128 kbps
- **Duration**: 30s - 2 minutes (seamless loop)
- **Size**: 1-3MB each

---

## 🔧 Implementation Code

### AudioManager Structure:
```csharp
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource ambientSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;
    
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    
    // Play methods
    public void PlayMusic(AudioClip clip, bool loop = true)
    public void PlayAmbient(AudioClip clip)
    public void PlaySFX(AudioClip clip, float volume = 1f)
    public void PlayUI(AudioClip clip)
    
    // Control methods
    public void SetVolume(AudioCategory category, float volume)
    public void CrossfadeMusic(AudioClip newClip, float duration)
    public void StopAll()
    public void PauseAll()
    public void ResumeAll()
}
```

### Sound Effect Triggers:
```csharp
// In CombatUI.cs
void OnAttackButtonClicked()
{
    AudioManager.Instance.PlaySFX(attackSwingSound);
    // ... existing code
}

// In InventorySystem.cs
public bool AddItem(string itemId, int quantity)
{
    bool success = // ... existing code
    if (success)
    {
        AudioManager.Instance.PlaySFX(itemPickupSound);
    }
    return success;
}

// In QuestManager.cs
public void CompleteQuest(string questId)
{
    // ... existing code
    AudioManager.Instance.PlaySFX(questCompleteSound);
}
```

---

## 🎼 Music Track List

### Suggested Track Names:
1. **MainMenuTheme.ogg** - Title screen
2. **VelarisExplore.ogg** - Peaceful Night Court
3. **SpringCourtTheme.ogg** - Nature and hope
4. **UnderTheMountain.ogg** - Oppressive darkness
5. **CombatStandard.ogg** - Regular battles
6. **BossAmarantha.ogg** - Final boss epic
7. **TrialSuspense.ogg** - The three trials
8. **Calanmai.ogg** - Fire Night ritual
9. **Starfall.ogg** - Magical celestial event
10. **LoveTheme.ogg** - Romantic moments
11. **TragedyTheme.ogg** - Sad moments
12. **TriumphTheme.ogg** - Victory moments
13. **HewnCityDark.ogg** - Court of Nightmares
14. **SummerCourtWaves.ogg** - Adriata peaceful
15. **WinterCourtIce.ogg** - Cold atmosphere

---

## 🔊 SFX List (50+ sounds)

### UI (10 sounds):
1. ButtonClick.wav
2. ButtonHover.wav
3. PanelOpen.wav
4. PanelClose.wav
5. TabSwitch.wav
6. Notification.wav
7. ErrorBeep.wav
8. ConfirmSound.wav
9. CancelSound.wav
10. MenuScroll.wav

### Combat (15 sounds):
11. SwordSwing.wav
12. MagicCast.wav
13. CriticalHit.wav
14. PlayerHurt.wav
15. PlayerDeath.wav
16. EnemyHit.wav
17. EnemyDeath.wav
18. Defend.wav
19. Dodge.wav
20. VictoryFanfare.wav
21. DefeatSound.wav
22. FleeAttempt.wav
23. LootDrop.wav
24. TurnStart.wav
25. HealEffect.wav

### Character (10 sounds):
26. LevelUp.wav
27. AbilityLearned.wav
28. Transformation.wav
29. CompanionJoin.wav
30. LoyaltyUp.wav
31. LoyaltyDown.wav
32. StatIncrease.wav
33. XPGain.wav
34. GoldPickup.wav
35. EquipItem.wav

### Quest & Inventory (10 sounds):
36. QuestAccepted.wav
37. QuestCompleted.wav
38. ObjectiveComplete.wav
39. ItemPickup.wav
40. ItemDrop.wav
41. ItemUse.wav
42. CraftingComplete.wav
43. SaveGame.wav
44. LoadGame.wav
45. BookOpen.wav

### World (10 sounds):
46. Winnowing.wav (teleport)
47. DoorOpen.wav
48. DoorClose.wav
49. Footstep1.wav
50. Footstep2.wav
51. LocationArrive.wav
52. TimePass.wav
53. MagicCharge.wav
54. BarrierHum.wav
55. WindWhoosh.wav

---

## 🌍 Ambient Loops (10+ tracks)

1. **SpringForest.ogg** - Birds, breeze, nature
2. **SummerOcean.ogg** - Waves, gulls, water
3. **AutumnWind.ogg** - Leaves, wind, fire
4. **WinterStorm.ogg** - Wind, snow, cold
5. **VelarisNight.ogg** - City, magic, peaceful
6. **HewnCityCavern.ogg** - Drips, echoes, dark
7. **UnderMountain.ogg** - Oppressive, eerie
8. **HumanVillage.ogg** - Rural, simple, earthy
9. **TheWall.ogg** - Barrier hum, wind, eerie
10. **DawnHealing.ogg** - Peaceful, warm, calm
11. **DayLibrary.ogg** - Pages, whispers, scholarly

---

## 📋 Task Checklist

### Week 1 (Audio System):
- [ ] Create AudioManager.cs
- [ ] Create MusicManager.cs
- [ ] Create SoundEffects.cs
- [ ] Setup AudioMixer in Unity
- [ ] Integrate with Settings UI
- [ ] Test volume controls

### Week 2 (Content Creation):
- [ ] Source/create UI sounds
- [ ] Source/create combat sounds
- [ ] Source/create character sounds
- [ ] Source/create ambient loops
- [ ] Source/create music tracks

### Week 3 (Integration):
- [ ] Add SFX to all UI interactions
- [ ] Add SFX to combat system
- [ ] Add SFX to character events
- [ ] Add ambient to all locations
- [ ] Add music to all contexts
- [ ] Test full audio experience

---

## 🎯 Success Metrics

### Quality Metrics:
- ✅ All major actions have sound feedback
- ✅ Ambient sound in all locations
- ✅ Music transitions smoothly
- ✅ No audio bugs or glitches

### Immersion Metrics:
- ✅ Audio enhances atmosphere
- ✅ Sound helps navigation
- ✅ Music matches mood
- ✅ Ambient sounds feel natural

### Technical Metrics:
- ✅ Audio performance acceptable (< 5% CPU)
- ✅ Memory usage reasonable (< 50MB)
- ✅ No audio pops or clicks
- ✅ Volume controls work perfectly

---

## 🔄 Integration with Other Phases

### Builds on:
- **Phase 7**: Settings UI (audio sliders ready)
- All game systems (trigger audio events)

### Prepares for:
- **Phase 10**: Complete sensory experience
- **Phase 13**: Professional release quality

---

## 📝 Audio Asset Sources

### Options for Audio:
1. **Free/Royalty-Free Sources**:
   - Freesound.org
   - OpenGameArt.org
   - Incompetech (music)
   - YouTube Audio Library

2. **Paid Assets**:
   - Unity Asset Store
   - AudioJungle
   - Epidemic Sound

3. **Custom Creation**:
   - Hire composer (music)
   - Hire sound designer (SFX)
   - Use audio tools (Audacity, etc.)

### Licensing Requirements:
- Ensure commercial use allowed
- Credit creators as needed
- Keep license documentation

---

*"Great audio turns a game into an experience."*

**Phase 9 Priority**: 🔥 **HIGH**  
**Dependencies**: Phase 7 Settings UI complete  
**Estimated Duration**: 3-4 hours  
**Success Criteria**: Immersive audio experience across all game systems
