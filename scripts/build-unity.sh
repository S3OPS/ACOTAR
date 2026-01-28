#!/bin/bash

# ACOTAR Unity Project Build Script
# Builds the Unity project using Docker

set -e

echo "=== Building ACOTAR Fantasy RPG ==="
echo ""

BUILD_TARGET=${1:-StandaloneWindows64}
BUILD_PATH="/workspace/Build/ACOTAR_RPG"

# Determine build method based on target
case "$BUILD_TARGET" in
    StandaloneWindows64|Windows64)
        BUILD_METHOD="BuildWindows64Player"
        BUILD_FILE="$BUILD_PATH.exe"
        ;;
    StandaloneOSX|OSX|Mac)
        BUILD_METHOD="BuildOSXUniversalPlayer"
        BUILD_FILE="$BUILD_PATH.app"
        ;;
    StandaloneLinux64|Linux64)
        BUILD_METHOD="BuildLinux64Player"
        BUILD_FILE="$BUILD_PATH.x86_64"
        ;;
    *)
        echo "Unknown build target: $BUILD_TARGET"
        echo "Supported targets: StandaloneWindows64, StandaloneOSX, StandaloneLinux64"
        exit 1
        ;;
esac

echo "Build Target: $BUILD_TARGET"
echo "Build Method: $BUILD_METHOD"
echo "Build File: $BUILD_FILE"
echo ""

# Run Unity build inside Docker container
docker-compose run --rm unity-builder \
    /opt/unity/Editor/Unity \
    -quit \
    -batchmode \
    -nographics \
    -projectPath /workspace \
    -buildTarget $BUILD_TARGET \
    -$BUILD_METHOD "$BUILD_FILE" \
    -logFile /workspace/Build/build.log

echo ""
echo "=== Build Complete ==="
echo "Build output: $BUILD_FILE"
echo "Build log: Build/build.log"
