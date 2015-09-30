#!/bin/bash

echo "Building Unity3d Project..."
DIR=`pwd`
UNITY3D_LOC="/Applications/Unity/Unity.app/Contents/MacOS/Unity"

eval $UNITY3D_LOC -quit -batchmode -projectPath $DIR -executeMethod ToBuild.BuildLinux

echo "Build Complete!"