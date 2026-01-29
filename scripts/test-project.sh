#!/bin/bash

# Test script for ACOTAR RPG game systems
# Verifies the Unity project structure and code

set -e

echo "=== ACOTAR Fantasy RPG - Test Script ==="
echo ""

# Test 1: Check Unity project structure
echo "Test 1: Verifying Unity project structure..."
required_dirs=(
    "Assets"
    "Assets/Scripts"
    "Assets/Scenes"
    "ProjectSettings"
    "Packages"
    "scripts"
)

all_dirs_exist=true
for dir in "${required_dirs[@]}"; do
    if [ -d "$dir" ]; then
        echo "  ✓ $dir exists"
    else
        echo "  ✗ $dir missing"
        all_dirs_exist=false
    fi
done

if [ "$all_dirs_exist" = true ]; then
    echo "  ✓ All required directories exist"
else
    echo "  ✗ Some directories are missing"
    exit 1
fi
echo ""

# Test 2: Check required Unity files
echo "Test 2: Verifying Unity configuration files..."
required_files=(
    "ProjectSettings/ProjectVersion.txt"
    "ProjectSettings/ProjectSettings.asset"
    "Packages/manifest.json"
)

all_files_exist=true
for file in "${required_files[@]}"; do
    if [ -f "$file" ]; then
        echo "  ✓ $file exists"
    else
        echo "  ✗ $file missing"
        all_files_exist=false
    fi
done

if [ "$all_files_exist" = true ]; then
    echo "  ✓ All required Unity files exist"
else
    echo "  ✗ Some Unity files are missing"
    exit 1
fi
echo ""

# Test 3: Check game scripts
echo "Test 3: Verifying game scripts..."
game_scripts=(
    "Assets/Scripts/Character.cs"
    "Assets/Scripts/GameManager.cs"
    "Assets/Scripts/LocationManager.cs"
    "Assets/Scripts/QuestManager.cs"
)

all_scripts_exist=true
for script in "${game_scripts[@]}"; do
    if [ -f "$script" ]; then
        echo "  ✓ $script exists"
    else
        echo "  ✗ $script missing"
        all_scripts_exist=false
    fi
done

if [ "$all_scripts_exist" = true ]; then
    echo "  ✓ All game scripts exist"
else
    echo "  ✗ Some game scripts are missing"
    exit 1
fi
echo ""

# Test 4: Check Docker files
echo "Test 4: Verifying Docker configuration..."
docker_files=(
    "Dockerfile"
    "docker-compose.yml"
    ".dockerignore"
)

all_docker_files_exist=true
for file in "${docker_files[@]}"; do
    if [ -f "$file" ]; then
        echo "  ✓ $file exists"
    else
        echo "  ✗ $file missing"
        all_docker_files_exist=false
    fi
done

if [ "$all_docker_files_exist" = true ]; then
    echo "  ✓ All Docker files exist"
else
    echo "  ✗ Some Docker files are missing"
    exit 1
fi
echo ""

# Test 5: Check build scripts
echo "Test 5: Verifying build scripts..."
build_scripts=(
    "scripts/build-docker.sh"
    "scripts/build-docker.bat"
    "scripts/build-unity.sh"
    "scripts/build-unity.bat"
)

all_scripts_executable=true
for script in "${build_scripts[@]}"; do
    if [ -f "$script" ]; then
        echo "  ✓ $script exists"
        if [[ "$script" == *.sh ]]; then
            if [ -x "$script" ]; then
                echo "    ✓ $script is executable"
            else
                echo "    ✗ $script is not executable"
                all_scripts_executable=false
            fi
        fi
    else
        echo "  ✗ $script missing"
        all_scripts_executable=false
    fi
done

if [ "$all_scripts_executable" = true ]; then
    echo "  ✓ All build scripts exist and .sh files are executable"
else
    echo "  ✗ Some build scripts are missing or not executable"
    exit 1
fi
echo ""

# Test 6: Check documentation
echo "Test 6: Verifying documentation..."
doc_files=(
    "README.md"
    "LORE.md"
    "SETUP.md"
    "THE_ONE_RING.md"
)

all_docs_exist=true
for doc in "${doc_files[@]}"; do
    if [ -f "$doc" ]; then
        lines=$(wc -l < "$doc")
        echo "  ✓ $doc exists ($lines lines)"
    else
        echo "  ✗ $doc missing"
        all_docs_exist=false
    fi
done

if [ "$all_docs_exist" = true ]; then
    echo "  ✓ All documentation files exist"
else
    echo "  ✗ Some documentation files are missing"
    exit 1
fi
echo ""

# Test 7: Verify code syntax (basic check)
echo "Test 7: Checking C# code syntax..."
syntax_ok=true
for script in "${game_scripts[@]}"; do
    # Check for namespace
    if grep -q "namespace ACOTAR" "$script"; then
        echo "  ✓ $script has correct namespace"
    else
        echo "  ✗ $script missing namespace"
        syntax_ok=false
    fi
    
    # Check for using statements
    if grep -q "using UnityEngine;" "$script"; then
        echo "  ✓ $script imports UnityEngine"
    else
        echo "  ⚠ $script might not need UnityEngine (or missing import)"
    fi
done

if [ "$syntax_ok" = true ]; then
    echo "  ✓ Code structure looks good"
fi
echo ""

# Test 8: Check ACOTAR lore accuracy
echo "Test 8: Verifying ACOTAR lore accuracy..."
lore_keywords=(
    "Velaris"
    "Rhysand"
    "Tamlin"
    "Spring Court"
    "Night Court"
    "Illyrian"
    "Cauldron"
)

lore_ok=true
for keyword in "${lore_keywords[@]}"; do
    if grep -r -q "$keyword" Assets/Scripts/ LORE.md; then
        echo "  ✓ Found reference to '$keyword'"
    else
        echo "  ✗ Missing reference to '$keyword'"
        lore_ok=false
    fi
done

if [ "$lore_ok" = true ]; then
    echo "  ✓ Lore references are present"
fi
echo ""

# Final summary
echo "==================================="
echo "    TEST SUMMARY"
echo "==================================="
if [ "$all_dirs_exist" = true ] && \
   [ "$all_files_exist" = true ] && \
   [ "$all_scripts_exist" = true ] && \
   [ "$all_docker_files_exist" = true ] && \
   [ "$all_scripts_executable" = true ] && \
   [ "$all_docs_exist" = true ] && \
   [ "$syntax_ok" = true ] && \
   [ "$lore_ok" = true ]; then
    echo "✓ ALL TESTS PASSED"
    echo ""
    echo "The ACOTAR Fantasy RPG project is properly set up!"
    echo "Next steps:"
    echo "  1. Open in Unity 2022.3.0f1"
    echo "  2. Or build with Docker: ./scripts/build-docker.sh"
    exit 0
else
    echo "✗ SOME TESTS FAILED"
    echo ""
    echo "Please review the output above for issues."
    exit 1
fi
