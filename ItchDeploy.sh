if [ "$1" == "--clean" ]; then
	rm -r Executables/*;
	echo "Directory clean!"
	exit 0
fi

if [ "$1" != "--build" ]; then
	echo "parameters: --clean"
	echo "parameters: --build [win/mac] "
	exit 0
fi

if [ "$2" == "win" ]; then
	echo "Starting Windows Unity builds..."
	echo "Make sure Unity is not running..."
	"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\Snehil\Documents\Programming\Unity\game.unity.docker" -executeMethod ToBuild.BuildWindows
	echo "Windows 64 built"
	"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\Snehil\Documents\Programming\Unity\game.unity.docker" -executeMethod ToBuild.BuildMac
	echo "Mac built"
	"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\Snehil\Documents\Programming\Unity\game.unity.docker" -executeMethod ToBuild.BuildWindows_32  
	echo "Windows 32 built"
elif [ "$2" == "mac" ]; then
	echo "Starting Unix Unity builds..."
	./BuildScript.sh -platform mac;
	./BuildScript.sh -platform win;
	./BuildScript.sh -platform win32;
fi

cp Assets/Resources/README.txt Executables/MacOS/;
cp Assets/Resources/README.txt Executables/Windows/;
cp Assets/Resources/README.txt Executables/Windows_32/;

if [ "$2" == "win" ]; then
	echo "Please zip folders... as names:"
	echo "Docker_Mac.zip"
	echo "Docker_Win.zip"
	echo "Docker_Win_32.zip"
elif [ "$2" == "mac" ]; then
	cd Executables/;
	zip -r Docker_Mac.zip MacOS/
	zip -r Docker_Win.zip Windows/
	zip -r Docker_Win_32.zip Windows_32/
	echo "Folders zipped!"
fi

echo "FINISHED ALL"
