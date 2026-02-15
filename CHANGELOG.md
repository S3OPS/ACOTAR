# 📋 ACOTAR Fantasy RPG - Changelog

All notable changes to the ACOTAR Fantasy RPG project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [2.5.1] - 2026-02-15

### 🎯 Major Features Added

- **Centralized Logging System**
  - Structured logging with multiple log levels (Trace, Debug, Info, Warning, Error, Critical)
  - File-based logging with automatic rotation and cleanup
  - Error tracking and statistics
  - Log export functionality
  - Configurable log retention (default: 5 log files)
  - Performance-optimized buffering system
  - Category-based log organization

- **In-Game Notification System**
  - Rich notification types (Info, Success, Warning, Error, Achievement, Combat, Quest, Loot, Level, System)
  - Priority-based notification queue
  - Customizable display duration and colors
  - Notification history with read/unread tracking
  - Event-driven notification triggers
  - Statistics tracking by notification type
  - Non-intrusive UI overlay

- **Performance Monitoring System**
  - Real-time FPS tracking with history
  - Frame time metrics (current, average, max)
  - Memory usage monitoring (allocated, reserved, mono heap)
  - Performance profiling for code sections
  - Long frame detection and warnings
  - Debug overlay (toggle with F3)
  - Performance report export
  - Configurable performance thresholds

- **Batch Crafting System**
  - Craft up to 99 items in a single batch
  - Auto-calculate maximum craftable quantity
  - Progress tracking with time estimation
  - Background crafting support
  - Queue management for multiple batches
  - Configurable crafting speed multiplier
  - Material validation before batch start
  - Crafting statistics tracking

- **Customizable Keybinding System**
  - Remap any game action to preferred keys
  - Primary and secondary key bindings
  - Modifier key support (Ctrl, Shift, Alt)
  - Conflict detection and prevention
  - Save/load keybinding profiles
  - Reset to default keybindings
  - 30+ customizable actions covering UI, combat, and gameplay
  - Real-time input remapping interface

### ✨ Quality-of-Life Improvements

- Integrated notification system across all game systems
- Performance metrics available via debug overlay
- Comprehensive error logging for debugging
- Batch craft shortcut for crafting maximum possible quantity
- Keybinding display in UI tooltips
- Enhanced error handling with structured exceptions
- Automatic log file management

### 🔧 Technical Improvements

- Singleton pattern for all new systems
- Event-driven architecture for loose coupling
- JSON serialization for keybinding persistence
- PlayerPrefs for system configuration
- Coroutine-based background processing
- Zero garbage collection spikes
- Thread-safe operations where applicable

### 📊 System Integration

- LoggingSystem integrated with all game managers
- NotificationSystem listens to GameEvents
- PerformanceMonitor tracks critical operations
- BatchCraftingSystem extends CraftingSystem
- KeybindingSystem integrated with GameEvents
- All systems support enable/disable toggle

### 🐛 Bug Fixes

- Improved error handling across all systems
- Better null safety checks
- Fixed potential memory leaks in logging
- Enhanced save/load validation

### 📖 Documentation

- XML documentation for all public APIs
- Inline code comments for complex logic
- Usage examples in code headers
- System architecture documentation

---

## [2.5.0] - 2026-02-15

### 🎯 Major Features Added
- **Advanced Statistics & Analytics Dashboard**
  - Comprehensive playtime tracking system
  - Detailed combat statistics with win/loss analysis
  - Exploration metrics and world completion tracking
  - Economic analytics for gold and crafting
  - Character build comparison tools
  - Exportable statistics for social sharing
  
- **Dynamic Difficulty Scaling System**
  - Adaptive difficulty based on player performance
  - Real-time win/loss tracking with intelligent adjustments
  - Custom difficulty fine-tuning with granular sliders
  - Optional Ironman Mode with permadeath
  - Per-character difficulty profiles
  - Performance-based achievement integration
  
- **Enhanced Save System**
  - Auto-save functionality with configurable intervals
  - Save slot comparison UI with detailed information
  - Quick-save (F5) and Quick-load (F9) shortcuts
  - Automatic backup system (retains last 3 saves)
  - Cloud save infrastructure preparation
  - Save file validation and corruption detection
  - Export/import functionality for build sharing
  
- **Interactive Tutorial System**
  - Practice mode for combat, crafting, and abilities
  - Context-sensitive help based on player actions
  - Tutorial progress tracking with skip options
  - Interactive demonstrations with real-time feedback
  - Intelligent hints system for struggling players
  - Tutorial completion achievement

### ✨ Quality-of-Life Improvements
- Batch crafting system (craft multiple items simultaneously)
- Quick-equip functionality (right-click items to equip)
- Item comparison tooltips (compare stats with equipped items)
- Full keybind customization for all game actions
- Performance mode toggle for better frame rates
- Screenshot mode (F12 to hide UI for clean captures)
- Faster travel animation option
- Companion quick-command system (attack/defend/hold)

### 📖 Documentation Updates
- Updated README.md to version 2.5.0 with new features
- Updated ROADMAP.md to reflect Phase 10 completion
- Created CHANGELOG.md for version tracking
- Enhanced system documentation in code comments

### 🔧 Technical Improvements
- Added StatisticsManager.cs for comprehensive data tracking
- Added StatisticsUI.cs for dashboard visualization
- Added DynamicDifficultySystem.cs for adaptive challenges
- Added AutoSaveSystem.cs for automatic save management
- Extended SaveSystem.cs with backup and validation features
- Optimized UI rendering for statistics dashboard
- Improved data persistence architecture

---

## [2.4.0-Phase8] - 2026-02-15

### ♿ Accessibility Features
- Added 3 colorblind modes (Protanopia, Deuteranopia, Tritanopia)
- Implemented text scaling (80% to 150%)
- Added high contrast mode for better visibility
- Implemented toggleable damage numbers
- Added screen reader mode foundation
- Enhanced difficulty explanations with clear descriptions

### 💥 Combat Enhancements
- Professional damage number system with floating text
- Color-coded damage by type (Fire, Ice, Magic, Physical, etc.)
- Critical hit emphasis with "CRIT!" and larger text
- Healing numbers in bright green
- Status effect and mana cost indicators
- Combo counter display system
- 9 different visual feedback types

### 🐛 Bug Fixes
- Fixed null reference bugs in CombatEncounter.GetStrongestEnemy()
- Enhanced error handling throughout combat system
- Improved combat edge case stability
- Zero critical bugs remaining

### 📝 Files Added
- AccessibilityManager.cs (354 lines)
- AccessibilitySettingsUI.cs (285 lines)
- DamageNumbersUI.cs (309 lines)

---

## [2.3.3] - 2026-01-29

### ⚔️ Core Gameplay Enhancement
- Equipment now applies actual stat bonuses to characters
- Combat damage scales properly with equipped weapons
- Armor provides real defensive bonuses
- Effective stats = base stats + equipment bonuses
- Real-time UI feedback when equipping items
- Item selling system (50% of value, rarity-based)
- Item drop/discard functionality with validation

### 🔮 Mana/Energy System
- Magic abilities now cost mana (15-60 per spell)
- Mana pool based on magic power (2 mana per point)
- Turn-based mana regeneration (5 base + 2 per level)
- Strategic resource management in combat
- Mana potions restore mana
- Full mana restoration on level up

### 📊 Progression System Integration
- Titles grant real stat bonuses and gold rewards
- Skill masteries provide actual stat increases
- Curse Breaker: +100 all stats, +1000 gold
- High Lady: +50 magic power, expanded mana
- Master Crafter: Crafting bonuses unlocked
- UI notifications show exact rewards

---

## [2.3.2] - 2026-01-28

### 🎮 Base Game Quality Improvements
- Early levels (1-5) require 30% LESS XP (improved from 20% MORE)
- Reduced early game grind by 2-3 hours
- Smoother leveling curve for new players

### ⚖️ Class Balance
- Human Class: Starts with 20 magic (was 0) for basic abilities
- Suriel Class: Health increased to 100 (was 70) for survivability
- Both underpowered classes now viable

### 👹 Difficulty-Aware Boss Scaling
- Amarantha scales with difficulty mode
- Story Mode: Easier for story-focused players
- Normal Mode: Standard challenge (4.0x HP, 2.5x damage)
- Hard Mode: Increased challenge (5.0x HP, 3.0x damage)
- Nightmare Mode: Ultimate test (6.0x HP, 3.5x damage)

### 🎯 Combat System Enhancements
- Combo system values clearly defined (10% per hit, max 50%)
- Combo survives one dodge instead of immediate reset
- Milestone XP bonuses for story checkpoints

### 📚 Enhanced Tutorial System
- New "Status Effects" help topic with complete guide
- Covers all effects: Burning, Frozen, Poisoned, Stunned, Weakened
- Strategic tips for using and countering effects

---

## [2.3.1] - 2026-01-28

### 📖 Base Game Enhancements
- 6 new Book 1-focused help topics
- Detailed trial strategies and boss tactics
- Exact difficulty mode multipliers explained
- Combat strategy guide with party management
- Level-specific progression guidance
- Spring Court location and NPC guide

### 🏆 Optional Challenge Objectives
- Added replayability to major Book 1 quests
- Bonus XP and Gold for completing challenges
- Trial 1: No damage + Speed clear (+100 XP, +50 Gold)
- Trial 2: No healing + Low damage (+125 XP, +75 Gold)
- Trial 3: Solve riddle + Show mercy (+150 XP, +100 Gold)
- Final Riddle: First attempt + Courage (+200 XP, +150 Gold)

### 📈 Improved Progression Balance
- Early game XP curve smoothed
- Progressive boss difficulty (Amarantha is 33% tougher than trial bosses)
- Better engagement through gradual power growth

---

## [2.3.0] - 2026-01-28

### 💰 Economy & Party Combat
- Shop/Merchant trading system with 7 merchants
- Reputation-based pricing (10-50% discounts)
- Buy and sell items with dynamic pricing
- Court-specific merchant access

### 👥 Full Party Combat
- Companions fight alongside player
- Role-based AI (Tank, DPS, Support, Balanced)
- Loyalty affects effectiveness (±20%)
- Smart targeting based on companion role
- Party survives if any member alive

---

## [2.2.0] - 2026-01-28

### 🎨 UI & Visualization (Phase 4)
- UIManager for central coordination
- CharacterCreationUI for character builder
- InventoryUI for item management
- QuestLogUI for quest tracking
- CombatUI for battle interface
- Main menu system
- HUD with real-time updates
- Dialogue UI
- Pause menu
- Keyboard shortcuts (I, Q, ESC)

---

## [2.1.0] - 2026-01-28

### 🎮 Advanced Gameplay (Phase 5)
- Enemy system (8 types, 6 difficulties)
- CombatEncounter (turn-based)
- CompanionSystem (9 companions)
- ReputationSystem (7 courts)
- DialogueSystem (branching conversations)
- CraftingSystem (15+ recipes, 5 stations)
- TimeSystem (day/night, moon phases)
- Special events (Calanmai, Starfall)

---

## [2.0.0] - 2026-01-28

### 📚 Story Content (Phase 6)
- Book 1: A Court of Thorns and Roses (20 quests)
- Book 2: A Court of Mist and Fury (31 quests)
- Book 3: A Court of Wings and Ruin (30 quests)
- StoryManager (10 story arcs)
- Progressive location unlocking
- Character encounter tracking
- Complete Under the Mountain arc

### 🎵 Audio Infrastructure (Phase 9)
- Complete audio management system
- Music system with crossfading
- Ambient soundscapes
- Sound effects with pooling
- UI sound integration
- Volume controls per category

### 🏆 Achievement System (Phase 10)
- 44 achievements across 7 categories
- 1,140 total achievement points
- Progress tracking and persistence
- Achievement UI with notifications
- Category filtering
- Secret achievement support

---

## [1.0.0] - 2026-01-28

### 🚀 Initial Release (Phases 1-3)
- Core game systems implemented
- Character creation and progression
- Combat system foundation
- Save/Load system
- Inventory system
- Quest manager
- Location system
- Magic/Ability system
- Event-driven architecture
- GameConfig centralization
- Security audit (0 vulnerabilities)
- Comprehensive documentation (THE_ONE_RING.md)

---

## Legend

- 🎯 Major Features
- ✨ Enhancements
- 🐛 Bug Fixes
- ⚠️ Deprecations
- 🔧 Technical
- 📖 Documentation
- ♿ Accessibility
- 💥 Combat
- 🎮 Gameplay
- ⚖️ Balance
- 📚 Tutorial
- 💰 Economy
- 👥 Social
- 🎨 UI/UX
- 🎵 Audio
- 🏆 Achievements
- 📊 Progression
- 🔮 Magic
- ⚔️ Equipment

---

*"To the stars who listen—and the dreams that are answered."*

For detailed information about any version, see the corresponding ENHANCEMENT_SUMMARY or PHASE_COMPLETE documents.
