@echo off
REM ACOTAR Unity Project Build Script for Windows
REM Builds the Unity project for Windows using Docker

echo === Building ACOTAR Fantasy RPG ===
echo.

set BUILD_TARGET=Windows64
set BUILD_PATH=/workspace/Build/ACOTAR_RPG

if not "%1"=="" set BUILD_TARGET=%1

echo Build Target: %BUILD_TARGET%
echo Build Path: %BUILD_PATH%
echo.

REM Run Unity build inside Docker container
docker-compose run --rm unity-builder /opt/unity/Editor/Unity -quit -batchmode -nographics -projectPath /workspace -buildTarget %BUILD_TARGET% -buildWindows64Player "%BUILD_PATH%.exe" -logFile /workspace/Build/build.log

echo.
echo === Build Complete ===
echo Build output: Build\ACOTAR_RPG.exe
echo Build log: Build\build.log
