#!/bin/bash

# ACOTAR Unity Project Build Script
# Builds the Unity project for Windows using Docker

set -e

echo "=== Building ACOTAR Fantasy RPG ==="
echo ""

BUILD_TARGET=${1:-Windows64}
BUILD_PATH="/workspace/Build/ACOTAR_RPG"

echo "Build Target: $BUILD_TARGET"
echo "Build Path: $BUILD_PATH"
echo ""

# Run Unity build inside Docker container
docker-compose run --rm unity-builder \
    /opt/unity/Editor/Unity \
    -quit \
    -batchmode \
    -nographics \
    -projectPath /workspace \
    -buildTarget $BUILD_TARGET \
    -buildWindows64Player "$BUILD_PATH.exe" \
    -logFile /workspace/Build/build.log

echo ""
echo "=== Build Complete ==="
echo "Build output: Build/ACOTAR_RPG.exe"
echo "Build log: Build/build.log"
