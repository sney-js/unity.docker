rm -r Executables/*;
./BuildScript.sh -platform mac;
./BuildScript.sh -platform win;
./BuildScript.sh -platform win32;
zip -r Executables/Docker_Win.zip Executables/Windows;
zip -r Executables/Docker_Winx86.zip Executables/Windows_32;
echo "FINISHED ALL"