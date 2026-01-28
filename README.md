# ACOTAR Fantasy RPG

A lore-accurate Fantasy RPG based on Sarah J. Maas's *A Court of Thorns and Roses* series, built with Unity and Docker.

## 🌟 About

Experience the world of Prythian in this immersive RPG that faithfully recreates the courts, characters, and magic from the ACOTAR series. Play through the complete Book 1 storyline with a full user interface, explore all seven courts, recruit legendary companions, and develop your character from human to High Fae with powerful magical abilities. Now with complete UI/UX for character creation, inventory management, quest tracking, and turn-based combat!

## 🎮 Features

### Lore-Accurate Content
- **Seven High Fae Courts**: Spring, Summer, Autumn, Winter, Night, Dawn, and Day
- **Character Classes**: High Fae, Lesser Fae, Human, Illyrian, Attor, and Suriel
- **Magic System**: 12 different magic types including Winnowing, Daemati, Shapeshifting, and elemental powers
- **Story Quests**: Complete Book 1 storyline with 20+ main and side quests
- **Locations**: Visit 13+ iconic locations like Velaris, Under the Mountain, Spring Court Manor, and more
- **Legendary Companions**: Recruit and adventure with 9 companions including Rhysand, Cassian, Azriel, and more

### Advanced Gameplay Systems
- **Turn-based Combat**: Strategic battles with 5 AI behavior patterns and full combat UI
- **Enemy System**: 8 enemy types from ACOTAR lore with difficulty scaling
- **Companion System**: Party of up to 3 companions with loyalty mechanics
- **Reputation System**: Track standing with all 7 courts, affecting prices and access
- **Dialogue System**: Branching conversations with consequences and visual interface
- **Crafting System**: 15+ recipes across 5 crafting stations
- **Time System**: Day/night cycle with moon phases affecting magic power
- **Inventory System**: Complete item management with grid UI, drag-drop ready, and equipment slots

### User Interface & Visualization
- **Main Menu System**: Title screen with New Game, Continue, Load, Settings
- **Character Creation UI**: Visual class and court selection with stat preview
- **HUD (Heads-Up Display)**: Real-time health, magic, XP, location, and gold tracking
- **Inventory UI**: Grid-based item display with tooltips, equipment slots, and rarity colors
- **Quest Log UI**: Comprehensive quest tracking with objectives, filters, and details
- **Dialogue UI**: Character portraits and branching choice buttons
- **Combat UI**: Turn-based interface with action buttons, enemy panels, and combat log
- **Pause Menu**: In-game menu with settings and save/load options
- **Keyboard Shortcuts**: Quick access (I for Inventory, Q for Quest Log, ESC for Pause)

### Story Content
- **Complete Book 1 Arc**: From mortal lands to Under the Mountain
- **The Three Trials**: Face the Middengard Wyrm, Naga, and impossible choices
- **Character Transformation**: Experience being Made by the Cauldron
- **Story Progression**: Unlock locations and characters as you progress
- **Side Quests**: Learning to read, painting, friendships, and more
- **Multiple Story Arcs**: Foundation for Books 1-3 content

### Technical Features
- Built with **Unity 2022.3.0f1**
- **Docker-based build system** for consistent, reproducible builds
- Cross-platform support (Windows, Mac, Linux)
- Modular, extensible codebase with 25 game systems
- Complete save/load functionality
- Full UI/UX implementation with 8 interactive panels
- Keyboard shortcut system
- Event-driven architecture
- 0 security vulnerabilities (CodeQL verified)

## 🚀 Getting Started

### Prerequisites

- **Docker** and **Docker Compose** installed
- **Unity License** (for building)
- Git

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/S3OPS/ACOTAR.git
   cd ACOTAR
   ```

2. **Set up Unity License** (required for building)
   ```bash
   # Linux/Mac
   export UNITY_LICENSE='<your-unity-license-content>'
   
   # Windows
   set UNITY_LICENSE=<your-unity-license-content>
   ```

3. **Build Docker image**
   ```bash
   # Linux/Mac
   ./scripts/build-docker.sh
   
   # Windows
   scripts\build-docker.bat
   ```

4. **Build the game**
   ```bash
   # Linux/Mac
   ./scripts/build-unity.sh
   
   # Windows
   scripts\build-unity.bat
   ```

5. **Run the game**
   The built executable will be in `Build/ACOTAR_RPG.exe`

## 🏗️ Project Structure

```
ACOTAR/
├── Assets/                    # Unity assets and game content
│   ├── Scenes/               # Unity scenes
│   ├── Scripts/              # Game scripts (25 systems)
│   │   ├── Character.cs      # Character system with classes and abilities
│   │   ├── CharacterStats.cs # Modular stat management
│   │   ├── AbilitySystem.cs  # Magic ability system
│   │   ├── LocationManager.cs # Location and court management
│   │   ├── QuestManager.cs   # Quest system with Book 1 content
│   │   ├── StoryManager.cs   # Story progression tracking
│   │   ├── GameManager.cs    # Main game orchestration
│   │   ├── Enemy.cs          # Enemy system with 8 creature types
│   │   ├── CombatEncounter.cs # Turn-based combat manager
│   │   ├── CompanionSystem.cs # 9 companions with loyalty
│   │   ├── ReputationSystem.cs # 7-court reputation tracking
│   │   ├── DialogueSystem.cs  # Branching conversations
│   │   ├── CraftingSystem.cs  # 15+ recipes
│   │   ├── TimeSystem.cs      # Day/night cycle
│   │   ├── InventorySystem.cs # Item management
│   │   ├── SaveSystem.cs      # Save/load functionality
│   │   ├── CombatSystem.cs    # Combat calculations
│   │   ├── GameConfig.cs      # Centralized configuration
│   │   ├── GameEvents.cs      # Event-driven architecture
│   │   ├── Book1Quests.cs     # Complete Book 1 quest content
│   │   ├── UIManager.cs       # Central UI coordination (NEW)
│   │   ├── CharacterCreationUI.cs # Character creation interface (NEW)
│   │   ├── InventoryUI.cs     # Inventory grid and management UI (NEW)
│   │   ├── QuestLogUI.cs      # Quest tracking interface (NEW)
│   │   └── CombatUI.cs        # Combat interface and display (NEW)
│   ├── Prefabs/              # Reusable game objects
│   ├── Materials/            # Visual materials
│   └── Resources/            # Game resources
├── ProjectSettings/          # Unity project configuration
├── Packages/                 # Unity packages
├── scripts/                  # Build scripts
│   ├── build-docker.sh       # Docker image build (Linux/Mac)
│   ├── build-docker.bat      # Docker image build (Windows)
│   ├── build-unity.sh        # Unity project build (Linux/Mac)
│   └── build-unity.bat       # Unity project build (Windows)
├── Dockerfile                # Docker configuration
├── docker-compose.yml        # Docker Compose configuration
├── LORE.md                   # Detailed ACOTAR lore reference
├── THE_ONE_RING.md          # Complete technical documentation
├── PHASE5_COMPLETE.md       # Phase 5 completion report
└── README.md                 # This file
```

## 📖 Documentation

- **[THE_ONE_RING.md](THE_ONE_RING.md)**: Complete technical documentation covering architecture, systems, API reference, and development roadmap
- **[LORE.md](LORE.md)**: Comprehensive lore reference covering all seven courts, character classes, magic systems, locations, and story events from the ACOTAR series
- **[PHASE5_COMPLETE.md](PHASE5_COMPLETE.md)**: Detailed completion report for Phase 5 advanced gameplay systems
- **[DEVELOPMENT.md](DEVELOPMENT.md)**: Development guide and best practices
- **[GETTING_STARTED.md](GETTING_STARTED.md)**: Quick start guide for new developers

## 🎯 Game Systems

### Story System
Complete Book 1 storyline with:
- **20+ Quests**: Main story and side quests
- **The Three Trials**: Face the Middengard Wyrm, Naga, and impossible choices
- **Character Development**: Transform from human to High Fae
- **Story Progression**: Unlock content as you advance through arcs
- **Bridge to Book 2**: Seamless transition with Rhysand's bargain

### Combat System
Turn-based strategic combat featuring:
- **8 Enemy Types**: From Bogge to Amarantha
- **5 AI Behaviors**: Aggressive, Defensive, Tactical, Berserker, Balanced
- **Player Actions**: Physical attacks, magic, defend, items, flee
- **Critical Hits**: 15% chance with 2x damage
- **Loot System**: Randomized drops and XP rewards

### Companion System
Adventure with legendary characters:
- **9 Companions**: Rhysand, Cassian, Azriel, Mor, Amren, Lucien, Tamlin, Nesta, Elain
- **Party Management**: Up to 3 active companions
- **Loyalty System**: 0-100 scale affecting combat effectiveness (80% to 120%)
- **Unique Roles**: Tank, DPS, Support, Balanced

### Reputation System
Your standing with the seven courts:
- **7 Reputation Levels**: Hostile to Exalted
- **Dynamic Pricing**: 50% discount to 50% penalty
- **Court Access**: Unlock exclusive areas and missions
- **Rivalry Mechanics**: Gaining with one court may hurt another

### Crafting System
Create powerful items:
- **15+ Recipes**: Weapons, potions, armor, magical items
- **5 Crafting Stations**: Workbench, Forge, Alchemy, Enchanting, Cooking
- **Material System**: Gather resources from exploration and combat
- **Level Requirements**: Progressive unlocking of advanced recipes

### Dialogue System
Meaningful conversations:
- **Branching Trees**: Multiple conversation paths
- **Choice Consequences**: Affect reputation and story
- **4+ NPC Dialogues**: Rhysand, Tamlin, Lucien, Suriel, and more
- **Quest Integration**: Trigger quests through dialogue

### Time & Environment
Living world simulation:
- **Day/Night Cycle**: 8 time periods from dawn to late night
- **Moon Phases**: Affect magic power by ±30%
- **Special Events**: Calanmai (Fire Night), Starfall
- **Calendar System**: Track in-game days, months, years

### Character System
The game features multiple character classes from the ACOTAR lore:
- **High Fae**: Powerful immortal beings with enhanced abilities
- **Illyrian**: Warrior race with wings and combat expertise
- **Lesser Fae**: Various magical creatures
- **Human**: Mortals that can be Made by the Cauldron
- **Attor**: Flying monsters serving Amarantha
- **Suriel**: Prophetic creatures with ancient knowledge

### Magic System
12 different magic types including:
- **Shapeshifting**: Transform appearance and form
- **Winnowing**: Teleportation across distances
- **Daemati**: Mind reading and manipulation
- **Darkness Manipulation**: Control shadows and darkness
- **Light Manipulation**: Wield light and truth
- **Elemental Magic**: Fire, Water, Ice, Wind manipulation
- **Healing**: Restore health and wounds
- **Shield Creation**: Protective barriers
- **Seer**: Prophetic visions of the future

### Quest System & Story
- **Main Story Quests**: Complete Book 1 arc (20+ quests)
- **The Three Trials**: Worm, Naga, and Hearts of Stone
- **Side Quests**: Learning to read, painting, friendships
- **Court Quests**: Serve the High Lords of each court
- **Companion Quests**: Personal storylines for each companion
- **Story Arcs**: Progressive unlocking through 10 defined arcs

### Locations
Visit all major locations from the books:
- Velaris (The City of Starlight)
- Under the Mountain
- Spring Court Manor
- Hewn City (Court of Nightmares)
- Illyrian Mountains
- Summer Court (Adriata)
- And more...

## 🛠️ Development

### Opening in Unity

1. Open Unity Hub
2. Click "Add project from disk"
3. Select the ACOTAR directory
4. Open with Unity 2022.3.0f1 or compatible version

### Docker Development Environment

Start an interactive development container:
```bash
docker-compose run --rm unity-builder /bin/bash
```

### Building Without Docker

If you have Unity installed locally:
1. Open the project in Unity
2. Go to File → Build Settings
3. Select your target platform
4. Click "Build"

## 🧪 Testing & Demo

The game includes comprehensive demo modes that showcase:

### Phase 5 Systems Demo
- Character creation and stat management
- Combat encounters with AI enemies
- Companion recruitment and party formation
- Reputation system with all 7 courts
- Dialogue system interactions
- Crafting weapons and potions
- Time progression and celestial events
- Full inventory display

### Story Demo
- Complete Book 1 quest progression
- Character transformation (human to High Fae)
- Location unlocking through story arcs
- Under the Mountain trials sequence
- Companion interactions
- Court allegiance changes

To see the demos, run the game and check the Unity console logs, or call:
```csharp
GameManager.Instance.DemoStoryProgression();
GameManager.Instance.DemoPhase5Systems();
```

## 📦 Build Output

After building, you'll find:
- `Build/ACOTAR_RPG.exe` - The game executable (Windows)
- `Build/build.log` - Build logs for debugging

## 🤝 Contributing

This is a fan project based on the ACOTAR series. Contributions are welcome!

### Development Phases
- ✅ **Phase 1-3**: Optimization, Enhancement, Documentation (Complete)
- ✅ **Phase 4**: UI & Visualization (Complete)
- ✅ **Phase 5**: Advanced Gameplay Systems (Complete)
- ✅ **Phase 6**: Story Content (In Progress - Book 1 Complete)
- 🔜 **Phase 7**: Multiplayer Features
- 🔜 **Phase 8**: Polish & Release

### Areas for Contribution
- Book 2 and Book 3 quest content
- Additional dialogue trees
- Court-specific storylines
- Companion personal quests
- UI design and implementation
- Sound and music
- Testing and bug reports

### Guidelines
- Maintain lore accuracy to the books
- Follow existing code patterns and architecture
- Add XML documentation for public APIs
- Test changes with demo systems
- Update documentation as needed

## 📊 Project Statistics

- **Total Lines of Code**: 8,000+
- **Game Systems**: 20 complete systems
- **Character Classes**: 6 classes
- **Magic Types**: 12 abilities
- **Locations**: 13+ unique places
- **Quests**: 30+ main and side quests
- **Enemies**: 8 creature types
- **Companions**: 9 legendary characters
- **Crafting Recipes**: 15+ items
- **Story Arcs**: 10 defined arcs (Books 1-3)
- **Security**: 0 vulnerabilities (CodeQL verified)

## 📜 License

This is a fan-made project based on the ACOTAR series by Sarah J. Maas. All rights to the ACOTAR intellectual property belong to Sarah J. Maas and Bloomsbury Publishing.

## 🙏 Credits

- **ACOTAR Series**: Sarah J. Maas
- **Unity Engine**: Unity Technologies
- **Docker Images**: [Unity CI](https://github.com/game-ci/docker)

## 📧 Support

For issues and questions:
- Open an issue on GitHub
- Check the LORE.md for accurate lore information
- Review the code comments for implementation details

---

⭐ **Experience the magic of Prythian!** ⭐