@echo off
REM ACOTAR Unity Build Script for Windows
REM This script builds the Unity project inside a Docker container

echo === ACOTAR Fantasy RPG - Docker Build Script ===
echo.

REM Check if Unity license is set
if "%UNITY_LICENSE%"=="" (
    echo Warning: UNITY_LICENSE environment variable is not set.
    echo You'll need to provide a Unity license to build.
    echo Set it using: set UNITY_LICENSE=^<your-license-content^>
)

REM Build Docker image
echo Building Docker image...
docker-compose build

echo.
echo === Docker image built successfully ===
echo.
echo To build the Unity project, run:
echo   scripts\build-unity.bat
echo.
echo To run the development environment:
echo   docker-compose run --rm unity-builder /bin/bash
