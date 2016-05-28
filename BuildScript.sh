#!/bin/bash

#----------------usage
if [ "$1" == "-help" ]; then
  echo "Usage: 
   -platform <win/win32/mac/linux/linux32> [default=64 bit for win/linux. mac=universal]
   -unity <path/to/unit/app>
   -logile <path/to/file.log> [if none provided, then print is shown as output]"
  exit 0
fi
#----------------extract arguments
while [[ $# > 1 ]]
do
	key="$1"

	case $key in
	    -platform)
		    BUILD_PLATFORM="$2"
		    shift # past argument
	    ;;
	    -unity)
		    UNITYLOC="$2"
		    shift # past argument
	    ;;
	    -logfile)
		    LOGLOC="$2"
		    shift # past argument
	    ;;
	    *) # unknown option
	    ;;
	esac
	shift # past argument or value
done
#----------------extract Unity App Location
if [ "$UNITYLOC" == "" ]; then 
	UNITYLOC="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
fi
#----------------extract platform
if [ "$BUILD_PLATFORM" == "win" ]; then   
	BUILD_METHOD="Windows"
elif [ "$BUILD_PLATFORM" == "mac" ]; then   
	BUILD_METHOD="Mac"
elif [ "$BUILD_PLATFORM" == "linux" ]; then   
	BUILD_METHOD="Linux"
elif [ "$BUILD_PLATFORM" == "win32" ]; then   
	BUILD_METHOD="Windows_32" 
elif [ "$BUILD_PLATFORM" == "linux32" ]; then   
	BUILD_METHOD="Linux_32"
else
	BUILD_METHOD="Mac"
fi
#----------------print info
echo "... Unity Location : "$UNITYLOC
echo "... Build Platform : "$BUILD_METHOD
echo "... Building Now ... " 
#----------------Run Build Command
DIR=`pwd`
UNITYLOC="/c/Program\ Files/Unity/Editor/Unity.exe"
BUILD="${UNITYLOC} -quit -batchmode -projectPath $DIR -logFile $LOGLOC -executeMethod ToBuild.Build$BUILD_METHOD"
	
if $BUILD; then 
	echo "... Build Complete!"
	exit 0
else 
	echo "... An Error Occurred."
	echo "    Command Used:"
	echo "'$BUILD'"
	exit 1
fi
