if [ "$1" == "-clean" ]; then
	rm -r Executables/*;
	echo "Directory clean!"
	exit 0
fi
if [ "$1" == "win" ]; then
	echo "Starting Windows Unity builds..."
	"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\Snehil\Documents\Programming\Unity\game.unity.docker" -executeMethod ToBuild.BuildWindows
	"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\Snehil\Documents\Programming\Unity\game.unity.docker" -executeMethod ToBuild.BuildMac
	"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -projectPath "C:\Users\Snehil\Documents\Programming\Unity\game.unity.docker" -executeMethod ToBuild.BuildWindows_32  
else
	echo "Starting Unix Unity builds..."
	./BuildScript.sh -platform mac;
	./BuildScript.sh -platform win;
	./BuildScript.sh -platform win32;
fi

cp Assets/Resources/README.txt Executables/MacOS/;
cp Assets/Resources/README.txt Executables/Windows/;
cp Assets/Resources/README.txt Executables/Windows_32/;
cd Executables/;

if [ "$1" == "win" ]; then
	echo "Please zip folders... as names:"
	echo "Docker_Mac.zip"
	echo "Docker_Win.zip"
	echo "Docker_Win_32.zip"
else
	echo "Folders zipped!"
	zip -r Docker_Mac.zip MacOS/;
	zip -r Docker_Win.zip Windows/;
	zip -r Docker_Win_32.zip Windows_32/;
fi
echo "FINISHED ALL"