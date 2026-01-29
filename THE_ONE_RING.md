# 🌟 The One Ring - ACOTAR RPG Technical Documentation

> *"One Ring to rule them all, One Ring to find them, One Ring to bring them all, and in the darkness bind them."*
> 
> This is the master documentation that binds all knowledge about the ACOTAR Fantasy RPG project together.

---

## 📚 Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture & Design](#architecture--design)
3. [Core Systems](#core-systems)
4. [Performance Optimizations](#performance-optimizations)
5. [Code Quality & Security](#code-quality--security)
6. [Development Roadmap](#development-roadmap)
7. [Developer Onboarding](#developer-onboarding)
8. [API Reference](#api-reference)

---

## 🎮 Project Overview

### What is ACOTAR RPG?

ACOTAR Fantasy RPG is a fan-made, lore-accurate fantasy role-playing game based on Sarah J. Maas's *"A Court of Thorns and Roses"* series. The game recreates the magical world of Prythian with its seven High Fae Courts, diverse character classes, and rich magic system.

### Key Features

- **7 High Fae Courts**: Spring, Summer, Autumn, Winter, Night, Dawn, and Day
- **6 Character Classes**: High Fae, Lesser Fae, Human, Illyrian, Attor, and Suriel
- **12 Magic Types**: Including Winnowing, Shapeshifting, Daemati, and elemental powers
- **Quest System**: Main story quests, side quests, court quests, and companion quests
- **Location System**: Explore iconic locations like Velaris, Under the Mountain, and the Spring Court Manor
- **Character Progression**: XP-based leveling system with stat growth
- **Combat System**: Turn-based combat with physical and magical attacks
- **Inventory System**: Item collection, equipment, and consumables
- **Save/Load System**: Persistent game state across sessions

### Technology Stack

- **Engine**: Unity 2022.3.0f1
- **Language**: C# (.NET Standard 2.1)
- **Architecture**: Component-based with Singleton pattern for managers
- **Serialization**: JSON for save data
- **Build System**: Docker-based cross-platform builds
- **Version Control**: Git/GitHub

---

## 🏗️ Architecture & Design

### Design Patterns

#### 1. Singleton Pattern
The `GameManager` uses the Singleton pattern to ensure only one instance exists throughout the game lifecycle.

```csharp
public static GameManager Instance { get; private set; }

private void Awake()
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
```

#### 2. Manager Pattern
Core game functionality is organized into specialized managers:
- **GameManager**: Orchestrates overall game flow
- **LocationManager**: Handles all location-related operations
- **QuestManager**: Manages quest progression

#### 3. Event-Driven Architecture
`GameEvents` provides a centralized event system for loose coupling:
- Character events (level up, transformation, damage)
- Location events (travel)
- Court events (allegiance changes)
- Quest events (started, completed)

#### 4. Modular Component Design
Game logic is broken into focused, reusable modules:
- **CharacterStats**: Encapsulates all stat-related logic
- **AbilitySystem**: Manages magic abilities
- **CombatSystem**: Handles combat calculations
- **InventorySystem**: Manages items and equipment
- **SaveSystem**: Handles game state persistence

### Project Structure

```
ACOTAR/
├── Assets/
│   └── Scripts/               # 25 game systems
│       ├── Character.cs       # Character system (refactored)
│       ├── CharacterStats.cs  # Modular stat management
│       ├── AbilitySystem.cs   # Magic ability system
│       ├── GameConfig.cs      # Centralized configuration
│       ├── GameEvents.cs      # Event-driven architecture
│       ├── GameManager.cs     # Main orchestration
│       ├── LocationManager.cs # Location management (optimized)
│       ├── QuestManager.cs    # Quest system
│       ├── StoryManager.cs    # Story progression tracking
│       ├── Book1Quests.cs     # Complete Book 1 content
│       ├── Enemy.cs            # Enemy system with 8 types
│       ├── CombatEncounter.cs # Turn-based combat manager
│       ├── CombatSystem.cs    # Combat calculations
│       ├── CompanionSystem.cs # 9 companions with loyalty
│       ├── ReputationSystem.cs # 7-court reputation
│       ├── DialogueSystem.cs  # Branching conversations
│       ├── CraftingSystem.cs  # 15+ recipes
│       ├── TimeSystem.cs      # Day/night cycle
│       ├── InventorySystem.cs # Item management
│       ├── SaveSystem.cs      # Save/load functionality
│       ├── UIManager.cs       # NEW: Central UI coordination
│       ├── CharacterCreationUI.cs # NEW: Character creation interface
│       ├── InventoryUI.cs     # NEW: Inventory grid UI
│       ├── QuestLogUI.cs      # NEW: Quest tracking UI
│       └── CombatUI.cs        # NEW: Combat interface
├── ProjectSettings/           # Unity configuration
├── Packages/                  # Unity packages
├── scripts/                   # Build automation
├── Documentation/
│   ├── THE_ONE_RING.md       # This file
│   ├── PHASE5_COMPLETE.md    # Phase 5 report
│   ├── PHASE6_FOUNDATION.md  # Phase 6 report
│   ├── PROJECT_SUMMARY.md    # Overall summary
│   ├── LORE.md               # ACOTAR lore reference
│   ├── SETUP.md              # Complete setup guide
│   └── DLC_GUIDE.md          # DLC content guide
└── README.md                  # Project overview
```
│   └── Scripts/
│       ├── Character.cs          # Character definition and management
│       ├── CharacterStats.cs     # Stat system (health, magic, etc.)
│       ├── AbilitySystem.cs      # Magic ability management
│       ├── GameManager.cs        # Core game orchestration
│       ├── GameConfig.cs         # Centralized configuration
│       ├── GameEvents.cs         # Event system for game state
│       ├── LocationManager.cs    # Location system
│       ├── QuestManager.cs       # Quest system
│       ├── CombatSystem.cs       # Combat mechanics
│       ├── InventorySystem.cs    # Item management
│       └── SaveSystem.cs         # Save/load functionality
├── ProjectSettings/              # Unity project configuration
├── Packages/                     # Unity packages and dependencies
└── scripts/                      # Build and deployment scripts
```

---

## ⚙️ Core Systems

### 1. Character System

#### Character Classes
Each class has unique base statistics optimized for different playstyles:

| Class | Health | Magic | Strength | Agility | Special |
|-------|--------|-------|----------|---------|---------|
| **High Fae** | 150 | 100 | 80 | 70 | Balanced magic user |
| **Illyrian** | 180 | 60 | 120 | 90 | Warrior class |
| **Lesser Fae** | 100 | 60 | 60 | 80 | Agile spellcaster |
| **Human** | 80 | 0 | 50 | 60 | Can be transformed |
| **Attor** | 120 | 40 | 90 | 100 | Fast attacker |
| **Suriel** | 70 | 150 | 30 | 40 | Prophetic powers |

#### Stat System
**CharacterStats** module handles:
- Health management (damage, healing)
- Experience and leveling
- Stat growth on level up
- XP requirements calculation

**Level Up Progression**:
- XP Required: `level * 100`
- Health: `+10` per level
- Magic Power: `+5` per level
- Strength: `+3` per level
- Agility: `+3` per level

### 2. Magic System

#### Ability Types
- **Shapeshifting**: Transform appearance
- **Winnowing**: Teleportation
- **Elemental Magic**: Fire, Water, Wind, Ice manipulation
- **Light/Darkness**: Manipulation of light and shadows
- **Healing**: Restore health
- **Shield Creation**: Defensive barriers
- **Daemati**: Mind reading and manipulation
- **Seer**: Prophetic visions

#### AbilitySystem Features
- Validates abilities against character class
- Prevents duplicate ability learning
- Supports ability removal (for curse effects)
- Track ability count and ownership

### 3. Location System

#### Location Types
- Court (High Fae territories)
- City (Major settlements)
- Village (Small communities)
- Manor (Noble residences)
- Mountain Range
- Forest
- Under the Mountain (Special location)
- Human Lands

#### Optimizations
- **Court-based caching**: Locations by court are cached for fast lookup
- **Name list caching**: All location names cached to avoid repeated dictionary key enumeration
- **Dictionary-based storage**: O(1) lookup time for locations by name

### 4. Quest System

#### Quest Types
- **Main Story**: Primary narrative quests
- **Side Quest**: Optional content
- **Court Quest**: Court-specific missions
- **Companion Quest**: Character relationship quests
- **Exploration Quest**: Discovery-based objectives

#### Quest Features
- Automatic XP rewards on completion
- Quest chaining (next quest triggers automatically)
- Active/Completed quest tracking
- Multiple objectives per quest
- Integration with event system

### 5. Combat System

#### Combat Actions
- **Physical Attack**: Strength-based damage
- **Magic Attack**: Magic power-based with ability check
- **Defend**: Reduces incoming damage by 50%
- **Use Ability**: Context-specific magic use
- **Flee**: Agility-based escape attempt

#### Combat Mechanics
- **Critical Hits**: 15% chance, 2x damage multiplier
- **Dodge**: Based on agility (1% per agility point)
- **Damage Variance**: 80-120% of base damage
- **Type-specific Multipliers**: Elemental magic deals 1.5x, Daemati 2x

### 6. Inventory System

#### Item Types
- **Weapon**: Equipped for combat bonuses
- **Armor**: Protection and stat bonuses
- **Consumable**: One-time use items (potions, etc.)
- **Quest Item**: Story-critical items
- **Crafting**: Materials for item creation
- **Magical**: Enchanted items with special effects

#### Item Rarity
Common → Uncommon → Rare → Epic → Legendary → Artifact

#### Features
- Stack management for consumables (up to 99)
- Quest item protection
- Equipment slots (weapon, armor)
- Item database with predefined items
- 50 slot inventory capacity

### 7. Save System

#### Saved Data
- Character state (name, class, court, stats)
- Abilities learned
- Current location and game time
- Story progress flags
- Quest completion status

#### Features
- JSON serialization for cross-platform compatibility
- Platform-specific save paths
- Save validation and error handling
- Save file existence check
- Delete save functionality

### 8. Story Progression System

#### Story Arc System
The **StoryManager** tracks progress through ACOTAR Books 1-3:

**Book 1 Arcs**:
- HumanLands → SpringCourt → UnderTheMountain → Aftermath

**Book 2 Arcs**:
- NightCourt → WarPreparations → Hybern

**Book 3 Arcs**:
- Alliance → War → Resolution

#### Features
- **Progressive Unlocking**: Locations and characters unlock as story advances
- **Arc Completion Tracking**: Know which parts of the story are complete
- **Content Gating**: New quests and areas unlock based on story progress
- **Character Encounters**: Track which major characters have been met

#### Book 1 Quest Content
**The Complete Under the Mountain Arc** (20+ quests):

**Main Story Quests**:
1. Beyond the Wall (Human lands)
2. The Spring Court's Beast (Capture by Tamlin)
3. Life at the Manor (Learning the curse)
4. Calanmai (Fire Night ritual)
5. Under the Mountain (Arrival)
6. First Trial: The Worm
7. Nights Under the Mountain (Rhysand)
8. Second Trial: The Naga
9. The Cost of Defiance (Clare Beddor)
10. Third Trial: Hearts of Stone
11. The Final Riddle
12. Breaking the Curse
13. Return to Spring
14. Nightmares and Walls
15. A Bargain Kept (Bridge to Book 2)

**Side Quests**:
- The Suriel's Wisdom
- Summer Court Alliance
- The Book of Breathings
- Letters and Words (learning to read)
- Canvas and Color (painting)
- A Servant's Wisdom (Alis)
- Memory of Starlight (Rhysand's gift)
- The Bone Carver's Gift
- The Court of Nightmares

#### Quest Rewards
- Experience points (100-1500 XP per quest)
- Item rewards
- Character unlocks
- Location access
- Story progression

### 9. UI & Visualization Systems

#### UIManager
**Central UI coordination system** managing all game interfaces:

**Features**:
- Panel management (8 panels: MainMenu, HUD, Inventory, QuestLog, Dialogue, Combat, PauseMenu, CharacterCreation)
- HUD updates (health, magic, XP, location, gold)
- Dialogue display with choice buttons
- Combat log with auto-scroll
- Pause system (freezes game time)
- Notification popups
- Keyboard shortcuts (I, Q, ESC)

#### Character Creation UI
**Visual character builder**:
- Class selection dropdown (6 classes)
- Court allegiance dropdown (8 courts)
- Real-time stat preview
- Detailed class/court descriptions
- Randomize button
- Name validation (2-20 characters)

**Class Stats Display**:
| Class | Health | Magic | Strength | Agility |
|-------|--------|-------|----------|---------|
| High Fae | 150 | 100 | 80 | 70 |
| Illyrian | 180 | 60 | 120 | 90 |
| Lesser Fae | 100 | 60 | 60 | 80 |
| Human | 80 | 0 | 50 | 60 |
| Attor | 120 | 40 | 90 | 100 |
| Suriel | 70 | 150 | 30 | 40 |

#### Inventory UI
**Complete item management interface**:
- Grid-based display with item slots
- Item details panel (name, description, stats, value)
- Equipment slots (weapon, armor)
- Rarity color coding (Common→Artifact)
- Actions: Use, Equip, Drop
- Sort and filter options
- Quest item protection

**Rarity Colors**:
- Common: White
- Uncommon: Green
- Rare: Blue
- Epic: Purple
- Legendary: Orange
- Artifact: Red

#### Quest Log UI
**Comprehensive quest tracking**:
- Scrollable quest list with status icons (○ active, ✓ complete)
- Detailed quest view (title, type, description, objectives, rewards)
- Objective checkboxes (☐ pending, ☑ complete)
- Type filters (Main/Side/Court/Companion)
- Status toggles (Active/Completed)
- Quest tracking for HUD
- Color-coded by type (Gold/Cyan/Purple/Pink)

#### Combat UI
**Turn-based battle interface**:
- Player health/magic bars with text values
- Multiple enemy panels with health bars
- Action buttons (Attack, Magic, Defend, Item, Flee)
- Expandable magic ability panel
- Scrolling combat log (10 lines visible)
- Turn indicator
- Target selection (click enemies)
- Victory screen with rewards
- Defeat screen

#### Keyboard Controls
- **I Key**: Toggle Inventory
- **Q Key**: Toggle Quest Log
- **ESC Key**: Pause Menu

---

## 🚀 Performance Optimizations

### 1. Centralized Configuration (GameConfig)
- All magic numbers moved to constants
- Eliminates hard-coded values
- Single source of truth for game balance
- Easy to tune without code changes

### 2. Location Caching
- Court-based location lookups cached
- O(1) instead of O(n) for repeat queries
- Location name list cached
- Cache invalidation on location addition

### 3. Modular Stat Calculations
- Stats initialized once per character class
- No repeated calculations during gameplay
- Property accessors provide clean interface
- Internal optimization hidden from API

### 4. Event-Driven Updates
- Reduces tight coupling between systems
- Systems subscribe only to relevant events
- No polling or update loops needed
- Clean separation of concerns

### 5. Dictionary-Based Storage
- O(1) lookup for locations by name
- O(1) lookup for quests by ID
- O(1) lookup for items by ID
- Minimal memory overhead

---

## 🔒 Code Quality & Security

### Security Audit Results
✅ **CodeQL Analysis**: 0 vulnerabilities found
- No SQL injection risks
- No XSS vulnerabilities
- No unsafe deserialization
- No hardcoded credentials
- No buffer overflow risks

### Code Quality Measures

#### 1. Input Validation
- Damage/heal amounts validated (no negative values)
- Experience gains validated
- File I/O wrapped in try-catch blocks
- Null checks on all manager references

#### 2. Error Handling
- Graceful degradation on missing components
- Debug warnings for invalid operations
- Safe save/load with exception handling
- Validation before state changes

#### 3. XML Documentation
- All public APIs documented
- Parameter descriptions
- Return value documentation
- Usage examples where appropriate

#### 4. Naming Conventions
- Clear, descriptive names
- Consistent C# conventions
- No magic numbers
- Self-documenting code

---

## 🗺️ Development Roadmap

### ✅ Completed (Current Release)

#### Phase 1: Optimization & Refactoring
- [x] Create modular CharacterStats system
- [x] Create modular AbilitySystem
- [x] Implement GameConfig for centralized configuration
- [x] Add location caching for performance
- [x] Remove code duplication

#### Phase 2: Enhancement & Features
- [x] Event-driven architecture (GameEvents)
- [x] Save/Load system
- [x] Combat system foundation
- [x] Inventory system foundation
- [x] Enhanced error handling

#### Phase 3: Quality & Documentation
- [x] Security audit (CodeQL)
- [x] Comprehensive documentation (The One Ring)
- [x] API reference
- [x] Developer onboarding guide

### ✅ Recently Completed Phases

#### Phase 4: UI & Visualization ✅ COMPLETE (January 2026)
- [x] Main menu system with title screen
- [x] Character creation UI with class/court selection
- [x] HUD with health, magic, XP, location displays
- [x] Inventory UI with grid, equipment, drag-drop ready
- [x] Quest log interface with filters and tracking
- [x] Dialogue UI with character portraits and choices
- [x] Combat UI with turn-based actions and enemy panels
- [x] Pause menu with settings access
- [x] Keyboard shortcuts (I, Q, ESC)
- [x] Color-coded rarity and quest types

#### Phase 5: Advanced Gameplay ✅ COMPLETE (January 2026)
- [x] Full combat encounters with turn-based system
- [x] Enemy AI system (5 behavior patterns)
- [x] Dialogue system (branching conversations)
- [x] Crafting system (15+ recipes, 5 stations)
- [x] Companion system (9 companions, loyalty mechanics)
- [x] Relationship/reputation system (7 courts)
- [x] Day/night cycle with moon phases
- [x] Special events (Calanmai, Starfall)

#### Phase 6: Story Content ✅ CORE COMPLETE (January 2026)
- [x] Complete Book 1 storyline (20+ quests)
- [x] Story progression system with arc tracking
- [x] Under the Mountain trials (Worm, Naga, Three Fae)
- [x] Side quest expansions (reading, painting, friendships)
- [x] Story-driven character unlocking
- [x] Location progression system

### 🎯 HIGH PRIORITY: Base Game Polish

> **Focus**: Complete the core game experience before adding new content. A polished Book 1 experience is more valuable than incomplete Books 1-3.

#### Phase 7: Core UI Completion ✅ COMPLETE (January 2026)
- [x] **Save/Load menu interface** - 5 save slot management
- [x] **Settings menu** (audio, graphics, accessibility, controls) - Complete customization
- [x] **Map system visualization** - Visual navigation of Prythian
- [x] **Tutorial/Help system** - 18+ comprehensive help topics
- [x] **Loading screens with lore tips** - 36 rotating ACOTAR facts

#### Phase 8: Base Game Quality 🔥 HIGH PRIORITY - NEXT
- [ ] **Combat balancing pass** - Ensure all classes are viable and fun
- [ ] **Quest flow polish** - Smooth transitions between story beats
- [ ] **UI/UX improvements** - Based on playtesting feedback
- [ ] **Bug fixes and stability** - Address any gameplay issues
- [ ] **Performance optimization** - Smooth gameplay on target hardware
- [ ] **Accessibility features** - Text size, colorblind modes, key remapping

#### Phase 9: Audio & Atmosphere 🔥 HIGH PRIORITY
- [ ] **Sound effects** - Combat, UI, environment
- [ ] **Ambient soundscapes** - Court-specific atmosphere
- [ ] **Background music** - Menu, exploration, combat themes
- [ ] **Audio settings** - Volume controls, mute options

#### Phase 10: Book 1 Final Polish
- [ ] **Book 1 playtesting** - Full playthrough validation
- [ ] **Ending polish** - Satisfying conclusion to Book 1 arc
- [ ] **Achievement integration** - Milestones for Book 1 content

### 🔮 Future Content (After Base Game Polish)

#### Phase 11: Extended Story Content
- [ ] Book 2 content (A Court of Mist and Fury)
- [ ] Book 3 content (A Court of Wings and Ruin)
- [ ] Court-specific storylines
- [ ] Companion personal quest arcs
- [ ] Multiple endings based on choices

#### Phase 12: Multiplayer (Future)
- [ ] Co-op quest system
- [ ] Shared world exploration
- [ ] PvP arena (optional)
- [ ] Trading system
- [ ] Social hub (Velaris)

#### Phase 13: Final Polish & Release
- [ ] Voice acting (stretch goal)
- [ ] Achievements system completion
- [ ] Localization (multiple languages)
- [ ] Platform-specific builds
- [ ] Public beta testing
- [ ] Official release

---

## 👨‍💻 Developer Onboarding

### Prerequisites
- Unity 2022.3.0f1 or later
- C# development environment (VS Code, Rider, or Visual Studio)
- Git for version control
- Basic knowledge of Unity and C#
- Familiarity with ACOTAR lore (recommended)

### Getting Started

#### 1. Clone the Repository
```bash
git clone https://github.com/S3OPS/ACOTAR.git
cd ACOTAR
```

#### 2. Open in Unity
1. Launch Unity Hub
2. Click "Open Project"
3. Navigate to the cloned ACOTAR directory
4. Unity will import the project (may take a few minutes)

#### 3. Explore the Codebase
Start with these key files:
1. `GameManager.cs` - Entry point and game orchestration
2. `Character.cs` - Character system overview
3. `GameConfig.cs` - Configuration and constants
4. `GameEvents.cs` - Event system reference

#### 4. Run the Demo
1. In Unity, open the main scene
2. Press Play
3. Check the Console for demo output

### Development Guidelines

#### Code Style
- Follow standard C# naming conventions
- Use XML documentation for public APIs
- Keep methods focused and single-purpose
- Prefer composition over inheritance
- Use events for inter-system communication

#### Adding New Features
1. Check if it fits existing systems
2. Consider event integration
3. Add to GameConfig if it has constants
4. Write XML documentation
5. Update this documentation

#### Testing
- Test character transformations
- Verify stat calculations
- Check save/load functionality
- Validate quest chains
- Test combat mechanics

### Common Tasks

#### Adding a New Location
```csharp
// In LocationManager.InitializeLocations()
AddLocation(new Location(
    "Your Location Name",
    "Detailed description",
    Court.Night,  // Ruling court
    LocationType.City
));
```

#### Adding a New Quest
```csharp
// In QuestManager.InitializeQuests()
Quest newQuest = new Quest(
    "quest_id_001",
    "Quest Title",
    "Quest description",
    QuestType.SideQuest
);
newQuest.objectives.Add("Complete objective 1");
newQuest.experienceReward = 250;
AddQuest(newQuest);
```

#### Adding a New Item
```csharp
// In InventorySystem.InitializeItemDatabase()
AddItemToDatabase(new Item(
    "item_id",
    "Item Name",
    "Item description",
    ItemType.Weapon,
    ItemRarity.Rare
) { strengthBonus = 15 });
```

---

## 📖 API Reference

### GameManager

#### Properties
- `Instance`: Singleton instance
- `playerCharacter`: Current player character
- `currentLocation`: Current location name
- `gameTime`: In-game days passed
- `locationManager`: Reference to LocationManager
- `questManager`: Reference to QuestManager

#### Methods
- `TransformToHighFae()`: Transform player to High Fae
- `GrantAbility(MagicType)`: Give player a magic ability
- `TravelTo(string)`: Move to a new location
- `ChangeCourtAllegiance(Court)`: Change court loyalty
- `ShowCharacterStats()`: Display character information
- `DemoStoryProgression()`: Run story demo

### Character

#### Properties
- `name`: Character name
- `characterClass`: CharacterClass enum
- `allegiance`: Court enum
- `health`, `maxHealth`: Current and max HP
- `magicPower`, `strength`, `agility`: Core stats
- `level`, `experience`: Progression
- `abilities`: List of MagicType
- `isFae`: Boolean flag
- `isMadeByTheCauldron`: Boolean flag

#### Methods
- `LearnAbility(MagicType)`: Learn a new ability
- `HasAbility(MagicType)`: Check ability ownership
- `TakeDamage(int)`: Apply damage
- `Heal(int)`: Restore health
- `IsAlive()`: Check if alive
- `GainExperience(int)`: Add XP and handle level ups
- `GetXPRequiredForNextLevel()`: Get XP needed

### GameConfig

#### Constants
- `BASE_XP_PER_LEVEL`: 100
- `STAT_INCREASE_PER_LEVEL_*`: Level up bonuses
- `ClassStats.*`: Base stats for each class
- `DEFAULT_*`: Default game settings

### GameEvents

#### Events
- `OnCharacterCreated(Character)`
- `OnCharacterLevelUp(Character, int)`
- `OnCharacterTransformed(Character, CharacterClass)`
- `OnAbilityLearned(Character, MagicType)`
- `OnLocationChanged(string, string)`
- `OnCourtAllegianceChanged(Character, Court)`
- `OnQuestStarted(Quest)`
- `OnQuestCompleted(Quest)`

#### Usage
```csharp
// Subscribe
GameEvents.OnCharacterLevelUp += OnLevelUpHandler;

// Handler
void OnLevelUpHandler(Character character, int newLevel)
{
    Debug.Log($"{character.name} reached level {newLevel}!");
}
```

### SaveSystem

#### Methods
- `SaveGame()`: Save current state
- `LoadGame()`: Load saved state
- `SaveExists()`: Check for save file
- `DeleteSave()`: Remove save file

### CombatSystem

#### Methods
- `CalculatePhysicalAttack(Character, Character)`: Returns CombatResult
- `CalculateMagicAttack(Character, Character, MagicType)`: Returns CombatResult
- `ApplyDefense(int)`: Returns reduced damage
- `AttemptFlee(Character, Character)`: Returns success boolean

### InventorySystem

#### Methods
- `AddItem(string, int)`: Add item by ID
- `RemoveItem(string, int)`: Remove item
- `HasItem(string, int)`: Check item existence
- `GetAllItems()`: Get all inventory slots
- `GetItemCount()`: Get slot count
- `GetItemFromDatabase(string)`: Get item definition

---

## 📞 Support & Contributing

### Getting Help
- Read through this documentation
- Check existing code examples
- Look at Unity console for error messages
- Review THE_ONE_RING.md for technical details
- Check LORE.md for world-building reference

### Contributing
This is a fan project. Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

### Community
- Respect ACOTAR source material and lore
- Keep changes lore-accurate when possible
- Maintain code quality standards
- Document your changes
- Be respectful of other contributors

---

## 📜 License & Credits

### Source Material
Based on "A Court of Thorns and Roses" series by Sarah J. Maas.
This is a fan-made project and is not officially affiliated with or endorsed by Sarah J. Maas or Bloomsbury Publishing.

### Project License
See LICENSE file for details.

### Credits
- **Original Creator**: ACOTAR development team
- **Optimization & Refactoring**: AI-assisted development (2026)
- **ACOTAR Series**: Sarah J. Maas
- **Game Engine**: Unity Technologies
- **Community**: All contributors and testers

---

## 🎯 Quick Reference

### Important Files
- 📄 `THE_ONE_RING.md` - This document
- 📄 `README.md` - Project overview
- 📄 `SETUP.md` - Complete setup and installation guide
- 📄 `LORE.md` - ACOTAR lore reference
- 📄 `DLC_GUIDE.md` - DLC content information

### Key Concepts
- **Courts**: The seven political divisions of Prythian
- **Made by the Cauldron**: Transformation from human to High Fae
- **Winnowing**: Fae teleportation magic
- **The Wall**: Barrier between human and Fae lands
- **Under the Mountain**: Location of Amarantha's curse

### Development Workflow
1. Plan feature
2. Update GameConfig if needed
3. Implement in appropriate manager/system
4. Add events if applicable
5. Test functionality
6. Document changes
7. Update this file

---

*"To the stars who listen—and the dreams that are answered."*

**Last Updated**: January 29, 2026
**Version**: 1.2.0 (Phase 7 Complete - Core UI Systems)
**Maintainer**: ACOTAR Development Team
