# Getting Started with ACOTAR Fantasy RPG

Welcome to the ACOTAR Fantasy RPG development guide! This document will help you get up and running quickly.

## Quick Start (5 Minutes)

### Option 1: Using Unity Editor (Recommended)

**This is the easiest option and requires NO license setup!** Unity Personal is completely FREE.

1. **Prerequisites**: 
   - [Unity Hub](https://unity.com/download) (free download)
   - Create a free Unity account
   - Unity Personal license is automatically activated when you sign in

2. **Install Unity**:
   - Open Unity Hub
   - Go to "Installs" → "Install Editor"
   - Install Unity **2022.3.0f1** (or compatible LTS version)

3. **Clone the repository**:
   ```bash
   git clone https://github.com/S3OPS/ACOTAR.git
   cd ACOTAR
   ```

4. **Open in Unity**:
   - In Unity Hub, click "Add" (or the dropdown arrow next to "Open")
   - Select "Add project from disk"
   - Select the ACOTAR folder
   - Unity will automatically import assets when you first open the project (may take a few minutes)

5. **Play**:
   - Open `Assets/Scenes/MainScene.unity`
   - Press the Play button
   - Check the Console window (Window → General → Console) for game output

6. **Build** (optional):
   - Go to File → Build Settings
   - Select your platform
   - Click "Build" and choose a location

### Option 2: Using Docker (For CI/CD and Automated Builds)

Docker builds are useful for CI/CD pipelines but require additional license setup.

1. **Prerequisites**: Install [Docker Desktop](https://www.docker.com/products/docker-desktop/)

2. **Clone the repository**:
   ```bash
   git clone https://github.com/S3OPS/ACOTAR.git
   cd ACOTAR
   ```

3. **Set Unity License** (required for Docker/headless builds):
   
   For Docker builds, you need a Unity license file (`.ulf`). See the [Unity Manual Activation Guide](https://docs.unity3d.com/Manual/ManualActivationGuide.html) for how to obtain one.
   
   > **Note**: This step is only required for Docker builds. If you're using the Unity Editor directly on your computer, you don't need to set this variable - your license is handled automatically when you sign in.
   
   ```bash
   export UNITY_LICENSE='<contents-of-your-.ulf-file>'
   ```

4. **Build and Run**:
   ```bash
   # Build Docker image
   ./scripts/build-docker.sh     # Linux/Mac
   scripts\build-docker.bat      # Windows
   
   # Build the game
   ./scripts/build-unity.sh      # Linux/Mac
   scripts\build-unity.bat       # Windows
   ```

5. **Play**:
   - Find the executable in `Build/ACOTAR_RPG.exe`
   - Run it and check console output for game demo

## Unity Licensing - Important Information

### Is Unity Free?

**YES!** Unity Personal is completely **FREE** for:
- Individuals
- Companies/organizations with less than $100,000 USD in annual revenue

### Do I need the UNITY_LICENSE environment variable?

**Only for Docker/headless builds.** If you're using Unity Editor directly:
- Just download Unity Hub
- Create a free Unity account  
- Sign in - your Personal license activates automatically
- No environment variables needed!

### Why does Docker need special setup?

Docker runs Unity in "batch mode" (no GUI), which requires a license file to be provided via environment variable. This is a Unity requirement for headless/automated builds.

### How to get a license file for Docker?

1. Follow the [Unity Manual Activation Guide](https://docs.unity3d.com/Manual/ManualActivationGuide.html)
2. This generates a `.ulf` license file
3. Set the contents as `UNITY_LICENSE` environment variable

## What's Included?

### 🎮 Game Features

1. **Seven Courts System**
   - All seven High Fae courts (Spring, Summer, Autumn, Winter, Night, Dawn, Day)
   - Accurate descriptions and powers from the books

2. **Character System**
   - Multiple character classes (High Fae, Illyrian, Human, etc.)
   - Stat system (Health, Magic Power, Strength, Agility)
   - Transformation system (Human → High Fae)

3. **Magic System**
   - 12 different magic types from the books
   - Winnowing, Daemati, Shapeshifting, Elemental magic, etc.

4. **Quest System**
   - Main story quests following book plot
   - Side quests and court quests
   - Progressive storyline with objectives

5. **Location System**
   - Visit iconic locations: Velaris, Under the Mountain, Spring Manor
   - Travel between courts and territories
   - Lore-accurate descriptions

### 📁 Project Structure

```
ACOTAR/
├── Assets/
│   ├── Scripts/          # Core game code
│   │   ├── Character.cs      # Character classes and abilities
│   │   ├── GameManager.cs    # Main game controller
│   │   ├── LocationManager.cs # Location system
│   │   └── QuestManager.cs   # Quest system
│   └── Scenes/           # Unity scenes
├── Docker/              # Docker configuration
│   ├── Dockerfile
│   └── docker-compose.yml
├── scripts/             # Build automation
│   ├── build-docker.sh
│   ├── build-unity.sh
│   └── test-project.sh
├── LORE.md             # ACOTAR lore reference
├── DEVELOPMENT.md       # Developer guide
└── README.md           # Project overview
```

## Testing the Game

### Run Tests

```bash
./scripts/test-project.sh
```

This verifies:
- ✓ Unity project structure
- ✓ All required files exist
- ✓ Code syntax and structure
- ✓ Lore accuracy
- ✓ Docker configuration

### Manual Testing in Unity

1. Open project in Unity
2. Press Play
3. Open Window → General → Console
4. You should see:
   - Character creation messages
   - Location system initialization
   - Quest system startup
   - Available locations list

### Demo Story Progression

The game includes a demo that shows key story moments:

```csharp
// In Unity Console or attach to a button:
GameManager.Instance.DemoStoryProgression();
```

This demonstrates:
- Starting as human in mortal lands
- Journey to Spring Court
- Transformation to High Fae Under the Mountain
- Discovering Velaris and the Night Court
- Learning magical abilities

## Understanding the Code

### Core Concepts

1. **Character System** (`Character.cs`):
   ```csharp
   Character player = new Character("Feyre", CharacterClass.Human, Court.Spring);
   player.LearnAbility(MagicType.Shapeshifting);
   ```

2. **Location System** (`LocationManager.cs`):
   ```csharp
   Location velaris = locationManager.GetLocation("Velaris");
   List<Location> nightCourtPlaces = locationManager.GetLocationsByCourt(Court.Night);
   ```

3. **Quest System** (`QuestManager.cs`):
   ```csharp
   questManager.StartQuest("main_001");
   questManager.CompleteQuest("main_001"); // Grants XP
   ```

4. **Game Manager** (`GameManager.cs`):
   ```csharp
   GameManager.Instance.TravelTo("Velaris");
   GameManager.Instance.TransformToHighFae();
   GameManager.Instance.GrantAbility(MagicType.Daemati);
   ```

### ACOTAR Lore Reference

All game content is based on accurate lore from the books. See `LORE.md` for:
- Detailed court descriptions
- Character class information
- Magic system details
- Location descriptions
- Story timeline

## Next Steps

### For Players

1. **Explore the Demo**: Run the game and see the story progression
2. **Check Lore**: Read `LORE.md` to understand the world
3. **Provide Feedback**: Share your experience

### For Developers

1. **Read Code**: Explore `Assets/Scripts/`
2. **Check Development Guide**: Read `DEVELOPMENT.md` for architecture
3. **Add Content**: Follow guides to add new quests, locations, or abilities
4. **Test Changes**: Use `./scripts/test-project.sh`
5. **Build**: Use Docker scripts to create builds

### Adding Your Own Content

#### Add a New Quest

Edit `Assets/Scripts/QuestManager.cs`:

```csharp
Quest myQuest = new Quest(
    "my_quest_001",
    "My Custom Quest",
    "Description of what the player must do",
    QuestType.SideQuest
);
myQuest.objectives.Add("First objective");
myQuest.objectives.Add("Second objective");
myQuest.experienceReward = 250;
AddQuest(myQuest);
```

#### Add a New Location

Edit `Assets/Scripts/LocationManager.cs`:

```csharp
AddLocation(new Location(
    "My Location",
    "Description matching ACOTAR style",
    Court.Night,
    LocationType.City
));
```

#### Add a New Character Ability

Edit `Assets/Scripts/Character.cs`:

```csharp
// 1. Add to MagicType enum
public enum MagicType {
    // ... existing
    MyNewAbility
}

// 2. Grant to player
GameManager.Instance.GrantAbility(MagicType.MyNewAbility);
```

## Troubleshooting

### "Unity version not found"
- Install Unity 2022.3.0f1 via Unity Hub
- Or use Docker build (requires license setup - see above)

### "Docker build fails"
- Check Docker is running: `docker --version`
- Ensure Unity license is set: `echo $UNITY_LICENSE`
- Make sure you're using the contents of your `.ulf` file, not a license key
- Check logs in `Build/build.log`
- **Tip**: If you don't need CI/CD, just use Unity Editor directly - it's much simpler!

### "Unity license not working in Docker"
- The `UNITY_LICENSE` must contain the full contents of a `.ulf` license file
- See [Unity Manual Activation](https://docs.unity3d.com/Manual/ManualActivationGuide.html)
- **Alternative**: Use Unity Editor for local builds - no special license setup needed!

### "Game doesn't start"
- Check Unity Console for errors
- Verify `GameManager` is in scene
- Ensure all scripts compile without errors

### "Missing references"
- Delete `Library/` folder
- Reopen project in Unity
- Let Unity reimport assets

## Resources

- **ACOTAR Wiki**: [acourtofthornsandroses.fandom.com](https://acourtofthornsandroses.fandom.com/)
- **Unity Documentation**: [docs.unity3d.com](https://docs.unity3d.com/)
- **Docker Docs**: [docs.docker.com](https://docs.docker.com/)
- **Unity CI Images**: [game-ci/docker](https://github.com/game-ci/docker)

## Community

This is a fan project celebrating Sarah J. Maas's ACOTAR series!

- Report bugs via GitHub Issues
- Suggest features via GitHub Issues
- Contribute via Pull Requests
- Share your experience!

## Important Notes

### Lore Accuracy
This game strives for 100% lore accuracy. All content is verified against the ACOTAR books:
- A Court of Thorns and Roses (ACOTAR)
- A Court of Mist and Fury (ACOMAF)
- A Court of Wings and Ruin (ACOWAR)

### Fair Use
This is a non-commercial fan project. All rights to ACOTAR intellectual property belong to Sarah J. Maas and Bloomsbury Publishing.

---

## Ready to Start?

Choose your path:

1. **I want to play**: Use Docker to build and run
2. **I want to develop**: Open in Unity Editor
3. **I want to understand**: Read `LORE.md` and `DEVELOPMENT.md`

**Welcome to Prythian! 🌟**
