# ACOTAR Fantasy RPG - Development Guide

## Project Overview

This Unity-based Fantasy RPG recreates the world of Prythian from Sarah J. Maas's ACOTAR series with lore-accurate content and Docker-based builds.

## Development Setup

### Local Unity Development

1. **Prerequisites**
   - Unity 2022.3.0f1 or compatible version
   - Unity Hub (recommended)
   - Git

2. **Clone and Open**
   ```bash
   git clone https://github.com/S3OPS/ACOTAR.git
   cd ACOTAR
   ```
   - Open Unity Hub
   - Add project from disk
   - Open with Unity 2022.3.0f1

3. **Project Structure**
   - `Assets/Scripts/` - All game logic
   - `Assets/Scenes/` - Unity scenes
   - `ProjectSettings/` - Unity configuration
   - `Packages/` - Unity package dependencies

### Docker Development

1. **Prerequisites**
   - Docker Desktop
   - Docker Compose

2. **Build Docker Image**
   ```bash
   ./scripts/build-docker.sh      # Linux/Mac
   scripts\build-docker.bat       # Windows
   ```

3. **Interactive Development**
   ```bash
   docker-compose run --rm unity-builder /bin/bash
   ```

4. **Build the Game**
   ```bash
   ./scripts/build-unity.sh       # Linux/Mac
   scripts\build-unity.bat        # Windows
   ```

## Code Architecture

### Core Systems

#### 1. Character System (`Character.cs`)
- **Enums**: `Court`, `CharacterClass`, `MagicType`
- **Character Class**: Manages player/NPC attributes, abilities, and stats
- **Methods**:
  - `SetBaseStats()` - Initializes stats based on character class
  - `LearnAbility()` - Adds new magical abilities
  - `TakeDamage()` / `Heal()` - Health management
  - `IsAlive()` - Status check

#### 2. Location Manager (`LocationManager.cs`)
- **Location Class**: Represents places in Prythian
- **LocationType Enum**: Court, City, Village, Manor, etc.
- **Methods**:
  - `InitializeLocations()` - Sets up all game locations
  - `GetLocation()` - Retrieves specific location
  - `GetLocationsByCourt()` - Filters by court
  - `GetAllLocationNames()` - Returns all location names

#### 3. Quest Manager (`QuestManager.cs`)
- **Quest Class**: Manages story and side quests
- **QuestType Enum**: MainStory, SideQuest, CourtQuest, etc.
- **Methods**:
  - `InitializeQuests()` - Creates all quests
  - `StartQuest()` - Activates a quest
  - `CompleteQuest()` - Marks quest complete, grants rewards
  - `GetActiveQuests()` - Returns active quest list

#### 4. Game Manager (`GameManager.cs`)
- **Singleton Pattern**: Single instance manages entire game
- **Game State**: Tracks player progression, location, time
- **Methods**:
  - `InitializeGame()` - Sets up new game
  - `TransformToHighFae()` - Lore-accurate transformation
  - `GrantAbility()` - Teaches new magic
  - `TravelTo()` - Location changes
  - `ChangeCourtAllegiance()` - Court switching
  - `DemoStoryProgression()` - Demonstrates game features

### Design Patterns

1. **Singleton**: `GameManager` uses singleton for global access
2. **Manager Pattern**: Separate managers for distinct systems
3. **Data-Driven**: Characters, locations, and quests are data objects
4. **Namespace Organization**: All code in `ACOTAR` namespace

## Adding New Content

### Adding a New Court (Example)

```csharp
// In LocationManager.cs - InitializeLocations()
AddLocation(new Location(
    "New Court Name",
    "Description of the court",
    Court.NewCourt,  // Add to Court enum first
    LocationType.Court
));
```

### Adding a New Quest

```csharp
// In QuestManager.cs - InitializeQuests()
Quest newQuest = new Quest(
    "quest_id",
    "Quest Title",
    "Quest description",
    QuestType.SideQuest
);
newQuest.objectives.Add("Objective 1");
newQuest.objectives.Add("Objective 2");
newQuest.experienceReward = 200;
AddQuest(newQuest);
```

### Adding a New Magic Type

```csharp
// 1. Add to MagicType enum in Character.cs
public enum MagicType {
    // ... existing types
    NewMagicType
}

// 2. Grant to character
GameManager.Instance.GrantAbility(MagicType.NewMagicType);
```

### Adding a New Character Class

```csharp
// 1. Add to CharacterClass enum
public enum CharacterClass {
    // ... existing classes
    NewClass
}

// 2. Add stats in SetBaseStats() method
case CharacterClass.NewClass:
    maxHealth = 120;
    magicPower = 80;
    strength = 70;
    agility = 75;
    break;
```

## Building

### Local Unity Build

1. Open project in Unity
2. File → Build Settings
3. Select platform (Windows, Mac, Linux)
4. Click "Build" or "Build and Run"

### Docker Build

```bash
# Build Docker image
./scripts/build-docker.sh

# Build Unity project
./scripts/build-unity.sh

# Output in Build/ directory
```

### Build Configuration

Modify `scripts/build-unity.sh` for different targets:
```bash
# Change BUILD_TARGET variable
BUILD_TARGET=Windows64    # Windows
BUILD_TARGET=OSXUniversal # Mac
BUILD_TARGET=Linux64      # Linux
```

## Testing

### Manual Testing

1. Open Unity
2. Press Play in editor
3. Check Console for demo output
4. Verify:
   - Character creation
   - Location system
   - Quest system
   - Stat changes

### Demo Mode

The `GameManager` includes `DemoStoryProgression()` that showcases:
- Character creation as human
- Transformation to High Fae
- Travel between locations
- Quest progression
- Ability learning
- Court allegiance changes

Run demo:
```csharp
GameManager.Instance.DemoStoryProgression();
```

## Lore Accuracy

### Maintaining Accuracy

1. **Reference**: Always check `LORE.md` for accurate information
2. **Character Names**: Use exact spellings from books
3. **Locations**: Match descriptions to book details
4. **Timeline**: Follow book chronology
5. **Magic System**: Abilities must match book capabilities

### Verified Content

All current content verified against:
- ACOTAR (Book 1)
- ACOMAF (Book 2)
- ACOWAR (Book 3)

## Docker Details

### Dockerfile

- Base: `unityci/editor:2022.3.0f1-windows-mono`
- Working directory: `/workspace`
- Volumes for caching builds
- License via environment variable

### docker-compose.yml

- Service: `unity-builder`
- Volumes:
  - Source code: `.:/workspace`
  - Build cache: `build-cache:/workspace/Library`
  - Build output: `build-output:/workspace/Build`
- Environment variables for Unity license

### Build Scripts

- `build-docker.sh/.bat` - Builds Docker image
- `build-unity.sh/.bat` - Builds Unity project in container

## Troubleshooting

### Unity Won't Open Project

- Check Unity version (must be 2022.3.0f1 or compatible)
- Delete `Library/` folder and reopen
- Check `ProjectSettings/ProjectVersion.txt`

### Docker Build Fails

- Ensure Docker is running
- Check Unity license is set: `echo $UNITY_LICENSE`
- Review build logs in `Build/build.log`
- Ensure Docker has enough resources (4GB+ RAM)

### Missing Dependencies

- Check `Packages/manifest.json`
- In Unity: Window → Package Manager → Refresh

## Best Practices

1. **Commit Often**: Small, focused commits
2. **Test Locally**: Test in Unity editor before Docker build
3. **Lore Check**: Verify against `LORE.md`
4. **Comments**: Add comments for complex logic
5. **Namespace**: Keep all code in `ACOTAR` namespace
6. **Serializable**: Mark data classes with `[System.Serializable]`

## Future Enhancements

Potential additions:
- [ ] Combat system
- [ ] Inventory system
- [ ] Dialog system
- [ ] Save/Load system
- [ ] 3D environments
- [ ] Character customization
- [ ] Multiplayer support
- [ ] More quests from later books
- [ ] Audio/Music
- [ ] UI/HUD

## Resources

- [Unity Documentation](https://docs.unity3d.com/)
- [Unity CI Docker Images](https://github.com/game-ci/docker)
- [ACOTAR Wiki](https://acourtofthornsandroses.fandom.com/)
- [Docker Documentation](https://docs.docker.com/)

## Contributing

1. Fork the repository
2. Create feature branch: `git checkout -b feature/new-feature`
3. Make changes (keep lore-accurate!)
4. Test thoroughly
5. Commit: `git commit -m "Add new feature"`
6. Push: `git push origin feature/new-feature`
7. Create Pull Request

## Support

- GitHub Issues: Report bugs or request features
- Documentation: Check README.md and LORE.md
- Code Comments: Review inline documentation

---

**Remember**: This is a fan project celebrating the ACOTAR series. All additions should respect the source material and maintain lore accuracy!
