rm -r Executables/*;
./BuildScript.sh -platform mac;
./BuildScript.sh -platform win;
./BuildScript.sh -platform win32;
cp Assets/Resources/README.txt Executables/MacOS/;
cp Assets/Resources/README.txt Executables/Windows/;
cp Assets/Resources/README.txt Executables/Windows_32/;
cd Executables/;
zip -r Docker_Mac.zip MacOS/;
zip -r Docker_Win.zip Windows/;
zip -r Docker_Win_32.zip Windows_32/;
echo "FINISHED ALL"