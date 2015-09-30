#!/bin/bash

echo "Building Unity3d Game..."
UNITY3D_LOC="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
eval $UNITY3D_LOC -quit -batchmode -executeMethod ToBuild.BuildLinux
echo "Build Complete!"