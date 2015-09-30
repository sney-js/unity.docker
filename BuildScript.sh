#!/bin/bash

echo "Building Unity3d Project..."
DIR=`pwd`
UNITY3D_LOC="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
USER="ssnehil@hotmail.com"
PASS="Unity6558"

eval $UNITY3D_LOC -quit -batchmode -projectPath $DIR -username $USER -password $PASS -executeMethod ToBuild.BuildMac

echo "Build Complete!"