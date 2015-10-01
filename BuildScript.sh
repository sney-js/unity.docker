#!/bin/bash

echo "... Checking git"

LOCAL=$(git rev-parse @)
REMOTE=$(git rev-parse @{u})
BASE=$(git merge-base @ @{u})

if [ $LOCAL = $REMOTE ]; then
    echo "... GIT Up-to-date. New Build not required."
elif [ $LOCAL = $BASE ]; then
	
	echo "... New commit detected! Pulling..."
	GITFETCH="git fetch"
	GITRESET="eval git reset --hard origin/master"
	eval $GITFETCH
	eval $GITRESET  
	
	DIR=`pwd`
	UNITYLOC="/Applications/Unity/Unity.app/Contents/MacOS/Unity"


	echo "... Re-Building Unity3d Project"
	BUILD="$UNITYLOC -quit -batchmode -projectPath $DIR -logFile $DIR'/Build.log'  -executeMethod ToBuild.BuildLinux"
	if eval $BUILD
		then echo "... Build Complete!"
	else echo "... An Error Occurred."
	fi

fi
