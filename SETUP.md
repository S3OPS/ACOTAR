# ACOTAR Fantasy RPG - Complete Setup Guide

This guide walks you through setting up, building, and running the ACOTAR Fantasy RPG from start to finish. Follow the steps in sequential order.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Step 1: Clone the Repository](#step-1-clone-the-repository)
3. [Step 2: Choose Your Setup Method](#step-2-choose-your-setup-method)
4. [Step 3A: Unity Editor Setup (Recommended)](#step-3a-unity-editor-setup-recommended)
5. [Step 3B: Docker Setup (For CI/CD)](#step-3b-docker-setup-for-cicd)
6. [Step 4: Build the Game](#step-4-build-the-game)
7. [Step 5: Run and Test](#step-5-run-and-test)
8. [Troubleshooting](#troubleshooting)
9. [Next Steps](#next-steps)

---

## Prerequisites

Before you begin, ensure you have the following installed on your system:

### Required for All Users
- **Git** - Version control system for cloning the repository
  - Download: [git-scm.com](https://git-scm.com/downloads)
  - Verify installation: `git --version`

### Choose One Build Method

**Option A: Unity Editor (Recommended for most users)**
- Unity Hub (free)
- Unity 2022.3.0f1 or compatible LTS version
- Unity Personal license is FREE for individuals and organizations with less than $100,000 in annual revenue

**Option B: Docker (For automated/CI/CD builds)**
- Docker Desktop
- Docker Compose (included with Docker Desktop)
- Unity license file (.ulf) for batch mode operation

> **💡 Which should I choose?** 
> - Use **Unity Editor** if you want to develop, modify, or easily build the game locally
> - Use **Docker** if you're setting up CI/CD pipelines or need reproducible headless builds

---

## Step 1: Clone the Repository

Open your terminal or command prompt and run:

```bash
git clone https://github.com/S3OPS/ACOTAR.git
cd ACOTAR
```

Verify the project structure:
```bash
ls -la  # Linux/Mac
dir     # Windows
```

You should see directories like `Assets/`, `ProjectSettings/`, `Packages/`, and files like `README.md`, `Dockerfile`, etc.

---

## Step 2: Choose Your Setup Method

Based on your needs, follow either:
- **[Step 3A](#step-3a-unity-editor-setup-recommended)** for Unity Editor setup (easier, recommended)
- **[Step 3B](#step-3b-docker-setup-for-cicd)** for Docker setup (CI/CD, automated builds)

---

## Step 3A: Unity Editor Setup (Recommended)

This is the easiest method and requires no special license configuration.

### 3A.1: Install Unity Hub

1. Download Unity Hub from [unity.com/download](https://unity.com/download)
2. Install Unity Hub for your operating system
3. Launch Unity Hub

### 3A.2: Create Unity Account and Activate License

1. In Unity Hub, click **Sign In** (or create a new account if you don't have one)
2. Sign in with your Unity ID
3. Unity Personal license is **automatically activated** when you sign in
   - No manual setup required
   - Completely FREE for qualifying users

### 3A.3: Install Unity Editor

1. In Unity Hub, go to **Installs** tab
2. Click **Install Editor** (or **Add** button)
3. Select **Unity 2022.3.0f1** (LTS version)
   - If not available, choose the closest 2022.3.x LTS version
4. In the modules selection:
   - Required: **Build Support** for your target platform(s):
     - Windows Build Support (IL2CPP and/or Mono)
     - Mac Build Support (if on Mac)
     - Linux Build Support (if building for Linux)
5. Click **Install** and wait for the installation to complete

### 3A.4: Open Project in Unity

1. In Unity Hub, go to **Projects** tab
2. Click **Add** → **Add project from disk**
3. Navigate to and select the `ACOTAR` folder you cloned
4. Click **Select Folder** or **Open**
5. Unity Hub will add the project to your list
6. Click on the project to open it with Unity 2022.3.0f1

### 3A.5: Wait for Initial Import

- Unity will import all assets on first open (this may take 5-10 minutes)
- Watch the progress bar at the bottom of the Unity Editor
- Don't interrupt this process

### 3A.6: Verify Project Loaded

Once import is complete:
1. In Unity, check the **Project** panel (bottom) - you should see the `Assets` folder
2. In the **Hierarchy** panel (left), you should see scene objects
3. Open `Assets/Scenes/MainScene.unity` if not already open

**✅ Unity Editor Setup Complete!** Proceed to [Step 4](#step-4-build-the-game).

---

## Step 3B: Docker Setup (For CI/CD)

Use this method for automated builds in CI/CD pipelines.

### 3B.1: Install Docker

1. Download Docker Desktop from [docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop/)
2. Install Docker Desktop for your operating system
3. Launch Docker Desktop and wait for it to start
4. Verify installation:
   ```bash
   docker --version
   docker-compose --version
   ```

### 3B.2: Obtain Unity License File

Docker builds require a Unity license file (`.ulf`) for batch/headless mode operation.

#### For Unity Personal License (Free):

1. Follow Unity's [Manual Activation Guide](https://docs.unity3d.com/Manual/ManualActivationGuide.html)
2. This process involves:
   - Generating a license request file (`.alf`)
   - Uploading it to Unity's license portal
   - Downloading the license file (`.ulf`)

#### For Unity Pro/Plus License:

1. Log into your Unity account
2. Navigate to your license management page
3. Download your license file (`.ulf`)

### 3B.3: Set Unity License Environment Variable

You need to set the Unity license as an environment variable for Docker to use.

**Linux/Mac:**
```bash
export UNITY_LICENSE=$(cat /path/to/your/Unity_v2022.x.ulf)
```

**Windows (PowerShell):**
```powershell
$env:UNITY_LICENSE = Get-Content -Path "C:\path\to\your\Unity_v2022.x.ulf" -Raw
```

**Windows (Command Prompt):**
```cmd
set UNITY_LICENSE=<paste-entire-contents-of-ulf-file-here>
```

> **⚠️ Important:** The environment variable must contain the entire contents of the `.ulf` file, not just the file path.

Verify the variable is set:
```bash
echo $UNITY_LICENSE     # Linux/Mac
echo %UNITY_LICENSE%    # Windows CMD
$env:UNITY_LICENSE      # Windows PowerShell
```

### 3B.4: Build Docker Image

Navigate to the project directory and build the Docker image:

**Linux/Mac:**
```bash
./scripts/build-docker.sh
```

**Windows:**
```cmd
scripts\build-docker.bat
```

This command will:
- Build a Docker image with Unity 2022.3.0f1
- Configure the Unity license
- Set up the build environment

Wait for the build to complete (first time may take 10-20 minutes to download the Unity Docker image).

### 3B.5: Verify Docker Setup

Test that the container can start:

```bash
docker-compose run --rm unity-builder /bin/bash
```

If successful, you'll see a bash prompt inside the container. Type `exit` to leave.

**✅ Docker Setup Complete!** Proceed to [Step 4](#step-4-build-the-game).

---

## Step 4: Build the Game

### Method A: Using Unity Editor

1. **Open the project** in Unity (if not already open)
2. Go to **File** → **Build Settings**
3. Select your target platform:
   - **PC, Mac & Linux Standalone**
   - Then choose: Windows, Mac, or Linux
4. Click **Add Open Scenes** if MainScene isn't listed
5. Click **Build** (or **Build and Run**)
6. Choose a build output location (e.g., `Build/` folder)
7. Wait for the build to complete (5-15 minutes depending on your system)

The executable will be created at your chosen location:
- Windows: `ACOTAR_RPG.exe`
- Mac: `ACOTAR_RPG.app`
- Linux: `ACOTAR_RPG.x86_64`

### Method B: Using Docker

Build the game using the Docker container:

**Linux/Mac:**
```bash
./scripts/build-unity.sh
```

**Windows:**
```cmd
scripts\build-unity.bat
```

**For different platforms:**
```bash
# Windows build (default)
./scripts/build-unity.sh StandaloneWindows64

# Mac build
./scripts/build-unity.sh StandaloneOSX

# Linux build
./scripts/build-unity.sh StandaloneLinux64
```

The build output will be in:
- Executable: `Build/ACOTAR_RPG.exe` (or `.app` / `.x86_64`)
- Build log: `Build/build.log`

---

## Step 5: Run and Test

### Run the Built Game

**Windows:**
```bash
./Build/ACOTAR_RPG.exe
```

**Mac:**
```bash
open ./Build/ACOTAR_RPG.app
```

**Linux:**
```bash
chmod +x ./Build/ACOTAR_RPG.x86_64
./Build/ACOTAR_RPG.x86_64
```

### Test in Unity Editor (Recommended for Development)

1. Open the project in Unity Editor
2. Open `Assets/Scenes/MainScene.unity`
3. Click the **Play** button (▶️) at the top
4. Open **Window** → **General** → **Console** to see game output
5. You should see:
   - Character system initialization
   - Location system loading
   - Quest system startup
   - Game demo messages

### Run Demo Systems

In the Unity Console, you can test the game systems:

```csharp
// In Unity Editor, create a script or use the Console
GameManager.Instance.DemoStoryProgression();
GameManager.Instance.DemoPhase5Systems();
```

This will demonstrate:
- Character creation and transformation
- Location travel system
- Quest progression
- Combat system
- Companion recruitment
- Crafting system
- And more!

---

## Troubleshooting

### Unity Editor Issues

#### "Unity version not found" or "Project version mismatch"
**Solution:**
- Ensure you installed Unity 2022.3.0f1 (or compatible 2022.3.x LTS)
- In Unity Hub, check the **Installs** tab for the correct version
- Try opening with the closest compatible version

#### "Project won't open" or "Corrupt project"
**Solution:**
1. Close Unity and Unity Hub
2. Delete the `Library/` folder in the project directory
3. Reopen Unity Hub and open the project again
4. Unity will reimport all assets (this takes time)

#### "Missing references" or "Script compilation errors"
**Solution:**
1. In Unity Editor: **Window** → **Package Manager**
2. Click the **Refresh** button (circular arrow icon)
3. Wait for packages to update
4. If errors persist, check the Console for specific missing packages

#### "Unity Personal license not activating"
**Solution:**
- Ensure you're signed into Unity Hub with your Unity ID
- Go to **Preferences** → **Licenses** in Unity Hub
- Click **Activate New License** → **Unity Personal** → **I don't use Unity in a professional capacity**

### Docker Issues

#### "Docker build fails with license error"
**Solution:**
- Verify `UNITY_LICENSE` environment variable is set: `echo $UNITY_LICENSE`
- Ensure it contains the **full contents** of your `.ulf` file, not a file path
- Try setting it again or check for formatting issues
- Make sure there are no extra quotes or escape characters

#### "Unity license not working in Docker"
**Solution:**
1. Verify your `.ulf` file is valid:
   - It should be XML format
   - Generated for Unity 2022.3.x
2. Regenerate the license if needed using [Manual Activation](https://docs.unity3d.com/Manual/ManualActivationGuide.html)
3. Set the environment variable again with the new license

#### "Docker container won't start"
**Solution:**
- Ensure Docker Desktop is running
- Check Docker has enough resources allocated:
  - Recommended: 4GB+ RAM, 50GB+ disk space
  - In Docker Desktop: Settings → Resources
- Try: `docker system prune` to clean up (⚠️ removes unused images)

#### "Build fails in Docker but works in Unity Editor"
**Solution:**
- Check build logs: `cat Build/build.log`
- Common issues:
  - Missing build target modules in Docker image
  - License expiration or restrictions
  - Platform-specific build requirements

### General Issues

#### "Game doesn't start" or "Black screen"
**Solution:**
1. Check the logs:
   - Unity: Editor Console window
   - Built game: Look for `output_log.txt` or `Player.log` in the game's data folder
2. Verify GameManager is present in the scene
3. Ensure all scripts compiled without errors

#### "Build is very slow"
**Solution:**
- First-time builds are always slower (10-20 minutes)
- Subsequent builds are faster due to caching
- Close other applications to free up resources
- SSD storage significantly improves build times

### Getting Help

If you're still experiencing issues:

1. **Check Build Logs:**
   - Unity: Check the Console window for errors
   - Docker: Check `Build/build.log`

2. **Search Existing Issues:**
   - GitHub: [github.com/S3OPS/ACOTAR/issues](https://github.com/S3OPS/ACOTAR/issues)

3. **Create a New Issue:**
   - Include: Operating system, Unity version, error messages, build logs
   - Describe steps to reproduce the issue

4. **Documentation:**
   - [Unity Documentation](https://docs.unity3d.com/)
   - [Docker Documentation](https://docs.docker.com/)
   - [Unity CI Docker Images](https://github.com/game-ci/docker)

---

## Next Steps

### For Players

1. **Play the Game:** Run the built executable and experience the ACOTAR story
2. **Explore Content:** The base game includes the complete Book 1 storyline
3. **Check DLC:** See `DLC_GUIDE.md` for information on Books 2 and 3 content

### For Developers

1. **Read the Lore:** Review `LORE.md` for accurate ACOTAR universe information
2. **Study the Code:** Explore `Assets/Scripts/` to understand the architecture
3. **Technical Docs:** Read `THE_ONE_RING.md` for complete technical documentation
4. **Make Changes:** 
   - Add new quests, locations, or abilities
   - Test changes using the Unity Editor Play mode
   - Run demo systems to verify functionality

### Adding Custom Content

#### Add a New Quest

Edit `Assets/Scripts/QuestManager.cs` or `Assets/Scripts/Book1Quests.cs`:

```csharp
Quest customQuest = new Quest(
    "custom_quest_001",
    "My Custom Quest",
    "Description of the quest objective",
    QuestType.SideQuest
);
customQuest.objectives.Add("Find the hidden treasure");
customQuest.objectives.Add("Return to quest giver");
customQuest.experienceReward = 300;
AddQuest(customQuest);
```

#### Add a New Location

Edit `Assets/Scripts/LocationManager.cs`:

```csharp
AddLocation(new Location(
    "Custom Location",
    "A beautifully detailed description matching ACOTAR style",
    Court.Night,  // Choose appropriate court
    LocationType.City
));
```

#### Test Your Changes

1. Save your scripts in Unity
2. Press **Play** to test in the editor
3. Use the Console to verify your additions
4. Build and test the standalone version

---

## Project Structure Reference

```
ACOTAR/
├── Assets/                    # Unity assets
│   ├── Scenes/               # Game scenes
│   │   └── MainScene.unity
│   ├── Scripts/              # C# game code
│   │   ├── GameManager.cs
│   │   ├── Character.cs
│   │   ├── QuestManager.cs
│   │   └── ... (32 systems)
│   ├── Prefabs/              # Reusable game objects
│   └── Resources/            # Game resources
├── ProjectSettings/          # Unity configuration
├── Packages/                 # Unity packages
├── scripts/                  # Build automation
│   ├── build-docker.sh
│   ├── build-unity.sh
│   └── test-project.sh
├── Build/                    # Build output (created during build)
├── Dockerfile                # Docker configuration
├── docker-compose.yml        # Docker Compose setup
├── SETUP.md                  # This file
├── README.md                 # Project overview
├── LORE.md                   # ACOTAR lore reference
├── THE_ONE_RING.md          # Technical documentation
└── DLC_GUIDE.md             # DLC information
```

---

## Important Notes

### Unity Licensing

- **Unity Personal is FREE** for individuals and organizations earning less than $100,000/year
- Unity Editor automatically activates your license when you sign in
- Docker builds require a separate `.ulf` license file for batch mode
- You do NOT need a Unity Pro license to build or play this game

### Lore Accuracy

This project strives for 100% accuracy to the ACOTAR book series:
- All content verified against the books
- Character names, locations, and events match canon
- Magic system reflects book descriptions
- Timeline follows book chronology

### Fair Use

This is a non-commercial fan project. All rights to ACOTAR intellectual property belong to Sarah J. Maas and Bloomsbury Publishing.

---

## Additional Resources

- **ACOTAR Wiki:** [acourtofthornsandroses.fandom.com](https://acourtofthornsandroses.fandom.com/)
- **Unity Learn:** [learn.unity.com](https://learn.unity.com/)
- **Unity Documentation:** [docs.unity3d.com](https://docs.unity3d.com/)
- **Docker Documentation:** [docs.docker.com](https://docs.docker.com/)
- **Project Repository:** [github.com/S3OPS/ACOTAR](https://github.com/S3OPS/ACOTAR)

---

**🌟 Welcome to Prythian! Enjoy building and playing the ACOTAR Fantasy RPG! 🌟**
