# Getting Started with ACOTAR Fantasy RPG

Welcome to the ACOTAR Fantasy RPG development guide! This document will help you get up and running quickly.

## Quick Start (5 Minutes)

### Option 1: Using Docker (Recommended)

1. **Prerequisites**: Install [Docker Desktop](https://www.docker.com/products/docker-desktop/)

2. **Clone the repository**:
   ```bash
   git clone https://github.com/S3OPS/ACOTAR.git
   cd ACOTAR
   ```

3. **Set Unity License** (required for building):
   ```bash
   # Get your license from Unity
   # Then set the environment variable:
   
   # Linux/Mac:
   export UNITY_LICENSE='your-license-content'
   
   # Windows:
   set UNITY_LICENSE=your-license-content
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

### Option 2: Using Unity Editor (For Development)

1. **Prerequisites**:
   - [Unity Hub](https://unity.com/download)
   - Unity 2022.3.0f1 (install via Unity Hub)

2. **Clone and Open**:
   ```bash
   git clone https://github.com/S3OPS/ACOTAR.git
   ```
   - Open Unity Hub
   - Click "Add project from disk"
   - Select the ACOTAR folder
   - Open with Unity 2022.3.0f1

3. **Explore**:
   - Open `Assets/Scenes/MainScene.unity`
   - Press Play button
   - Check Console window for game output

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
- Or use Docker build (no Unity installation needed)

### "Docker build fails"
- Check Docker is running: `docker --version`
- Ensure Unity license is set: `echo $UNITY_LICENSE`
- Check logs in `Build/build.log`

### "Game doesn't start"
- Check Unity Console for errors
- Verify `GameManager` is in scene
- Ensure all scripts compile without errors

### "Missing references"
- Delete `Library/` folder
- Reopen project in Unity
- Let Unity reimport assets

## Resources

- **ACOTAR Wiki**: [fandom.com/wiki/A_Court_of_Thorns_and_Roses](https://acourtofthornsandroses.fandom.com/)
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
