# ACOTAR Fantasy RPG

A lore-accurate Fantasy RPG based on Sarah J. Maas's *A Court of Thorns and Roses* series, built with Unity and Docker.

## 🌟 About

Experience the world of Prythian in this immersive RPG that faithfully recreates the courts, characters, and magic from the ACOTAR series. Play through key story moments, explore all seven courts, and develop your character from human to High Fae with powerful magical abilities.

## 🎮 Features

### Lore-Accurate Content
- **Seven High Fae Courts**: Spring, Summer, Autumn, Winter, Night, Dawn, and Day
- **Character Classes**: High Fae, Lesser Fae, Human, Illyrian, and special creatures
- **Magic System**: 12 different magic types including Winnowing, Daemati, Shapeshifting, and elemental powers
- **Story Quests**: Follow the main storyline from the books with accurate quest progression
- **Locations**: Visit iconic locations like Velaris, Under the Mountain, Spring Court Manor, and more

### Game Systems
- **Character Progression**: Transform from human to High Fae (Made by the Cauldron)
- **Court Allegiance**: Choose and change your court allegiance
- **Magic Abilities**: Learn and use various magical powers from the books
- **Quest System**: Main story quests, side quests, court quests, and companion quests
- **Location Management**: Travel between the various courts and territories of Prythian

### Technical Features
- Built with **Unity 2022.3.0f1**
- **Docker-based build system** for consistent, reproducible builds
- Cross-platform support (Windows, Mac, Linux)
- Modular, extensible codebase

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
│   ├── Scripts/              # Game scripts
│   │   ├── Character.cs      # Character system with classes and abilities
│   │   ├── LocationManager.cs # Location and court management
│   │   ├── QuestManager.cs   # Quest system
│   │   └── GameManager.cs    # Main game orchestration
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
└── README.md                 # This file
```

## 📖 Documentation

- **[LORE.md](LORE.md)**: Comprehensive lore reference covering all seven courts, character classes, magic systems, locations, and story events from the ACOTAR series

## 🎯 Game Systems

### Character System
The game features multiple character classes from the ACOTAR lore:
- **High Fae**: Powerful immortal beings with enhanced abilities
- **Illyrian**: Warrior race with wings and combat expertise
- **Lesser Fae**: Various magical creatures
- **Human**: Mortals that can be Made by the Cauldron

### Magic System
12 different magic types including:
- Shapeshifting
- Winnowing (teleportation)
- Daemati (mind reading)
- Elemental magic (Fire, Water, Ice, Wind, Light, Darkness)
- Healing
- Shield creation

### Quest System
- **Main Story Quests**: Follow Feyre's journey from the books
- **Side Quests**: Additional adventures in Prythian
- **Court Quests**: Serve the High Lords
- **Companion Quests**: Build relationships with key characters

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

## 🧪 Testing

The game includes a demo mode that showcases key features:
- Character creation and stats
- Story progression through major plot points
- Location system
- Quest management
- Magic abilities
- Character transformation

To see the demo, run the game and check the Unity console logs.

## 📦 Build Output

After building, you'll find:
- `Build/ACOTAR_RPG.exe` - The game executable (Windows)
- `Build/build.log` - Build logs for debugging

## 🤝 Contributing

This is a fan project based on the ACOTAR series. Contributions that maintain lore accuracy are welcome!

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