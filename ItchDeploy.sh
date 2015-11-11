rm -r Executables/*;
./BuildScript.sh -platform mac;
./BuildScript.sh -platform win;
./BuildScript.sh -platform win32;
cd Executables/;
cp Assets/Resources/README.txt Executables/;
zip -r Docker_Mac.zip MacOS/ README.txt;
zip -r Docker_Win.zip Windows/ README.txt;
zip -r Docker_Win_32.zip Windows_32/ README.txt;
echo "FINISHED ALL"