#!/bin/bash


while [[ $# > 1 ]]
do
	key="$1"

	case $key in
	    -platform)
		    BUILD_PLATFORM="$2"
		    shift # past argument
	    ;;
	    -unityPath)
		    UNITYLOC="$2"
		    shift # past argument
	    ;;
	    *)	
	            # unknown option
	    ;;
	esac
	shift # past argument or value
done

if [ "$UNITYLOC" == "" ]; then 
	UNITYLOC="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
fi


if [ "$BUILD_PLATFORM" == "win" ]; then   
	BUILD_METHOD="BuildWindows"
elif [ "$BUILD_PLATFORM" == "mac" ]; then   
	BUILD_METHOD="BuildMac"
elif [ "$BUILD_PLATFORM" == "linux" ]; then   
	BUILD_METHOD="BuildLinux" 
else
	BUILD_METHOD="BuildLinux"
fi

echo "Build for : "$BUILD_PLATFORM
echo "Unity Location : "$UNITYLOC

echo "... Building Unity3d Project"

DIR=`pwd`
BUILD="$UNITYLOC -quit -batchmode -projectPath $DIR -logFile $DIR'/Build.log' -executeMethod ToBuild.$BUILD_METHOD"
	
if eval $BUILD
	then echo "... Build Complete!"
else echo "... An Error Occurred."
fi
