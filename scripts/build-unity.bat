@echo off
REM ACOTAR Unity Project Build Script for Windows
REM Builds the Unity project using Docker

echo === Building ACOTAR Fantasy RPG ===
echo.

set BUILD_TARGET=%1
if "%BUILD_TARGET%"=="" set BUILD_TARGET=StandaloneWindows64

set BUILD_PATH=/workspace/Build/ACOTAR_RPG

REM Determine build method based on target
if "%BUILD_TARGET%"=="StandaloneWindows64" (
    set BUILD_METHOD=BuildWindows64Player
    set BUILD_FILE=%BUILD_PATH%.exe
) else if "%BUILD_TARGET%"=="Windows64" (
    set BUILD_METHOD=BuildWindows64Player
    set BUILD_FILE=%BUILD_PATH%.exe
) else if "%BUILD_TARGET%"=="StandaloneLinux64" (
    set BUILD_METHOD=BuildLinux64Player
    set BUILD_FILE=%BUILD_PATH%.x86_64
) else if "%BUILD_TARGET%"=="Linux64" (
    set BUILD_METHOD=BuildLinux64Player
    set BUILD_FILE=%BUILD_PATH%.x86_64
) else (
    echo Unknown build target: %BUILD_TARGET%
    echo Supported targets: StandaloneWindows64, StandaloneLinux64
    exit /b 1
)

echo Build Target: %BUILD_TARGET%
echo Build Method: %BUILD_METHOD%
echo Build File: %BUILD_FILE%
echo.

REM Run Unity build inside Docker container
docker-compose run --rm unity-builder /opt/unity/Editor/Unity -quit -batchmode -nographics -projectPath /workspace -buildTarget %BUILD_TARGET% -%BUILD_METHOD% "%BUILD_FILE%" -logFile /workspace/Build/build.log

echo.
echo === Build Complete ===
echo Build output: %BUILD_FILE%
echo Build log: Build\build.log
