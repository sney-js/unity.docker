#!/bin/bash

echo "Building Unity3d Project..."
DIR=`pwd`
UNITYLOC="/Applications/Unity/Unity.app/Contents/MacOS/Unity"

BUILD="$UNITYLOC -quit -batchmode -projectPath $DIR -logFile $DIR"/Build.log"  -executeMethod ToBuild.BuildLinux"
if eval $BUILD
	then echo "Build Complete!"
else echo "An Error Occurred."
fi
