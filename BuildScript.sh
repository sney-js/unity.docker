#!/bin/bash

echo "... Checking git"
DIR=`pwd`
UNITYLOC="/Applications/Unity/Unity.app/Contents/MacOS/Unity"

GITFETCH="git fetch"
GITRESET="eval git reset --hard origin/master"
eval $GITFETCH
eval $GITRESET

echo "... Building Unity3d Project"
BUILD="$UNITYLOC -quit -batchmode -projectPath $DIR -logFile $DIR'/Build.log'  -executeMethod ToBuild.BuildLinux"
if eval $BUILD
	then echo "... Build Complete!"
else echo "... An Error Occurred."
fi
