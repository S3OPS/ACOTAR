#!/bin/bash

# ACOTAR Unity Build Script
# This script builds the Unity project inside a Docker container

set -e

echo "=== ACOTAR Fantasy RPG - Docker Build Script ==="
echo ""

# Check if Unity license is set
if [ -z "$UNITY_LICENSE" ]; then
    echo "Warning: UNITY_LICENSE environment variable is not set."
    echo "You'll need to provide a Unity license to build."
    echo "Set it using: export UNITY_LICENSE='<your-license-content>'"
fi

# Build Docker image
echo "Building Docker image..."
docker-compose build

echo ""
echo "=== Docker image built successfully ==="
echo ""
echo "To build the Unity project, run:"
echo "  ./scripts/build-unity.sh"
echo ""
echo "To run the development environment:"
echo "  docker-compose run --rm unity-builder /bin/bash"
