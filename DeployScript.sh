#!/bin/bash -e

me="$(basename "$(test -L "$0" && readlink "$0" || echo "$0")")"
ARCHIVENAME="SpaceDocker.tar"

#example - ./DeployScript.sh -k stardock_keys -b Executables/MacOS/ -c /home/stardock/Documents/SpaceDocker/
#go example - ./DeployScript.sh -k stardock_keys -b binaries/ -c binaries/

# SSH="ssh stardock@10.2.21.63"
vmhost="10.2.21.63"
sshKey="stardock_keys"
configDir="binaries" #/Snehil.Docker.game_SpaceDocker_Game_BuildDocker_md5.checksum ?
binarydir="binaries/*"

while [[ $# > 1 ]]
do
    scriptKey="$1"

    case $scriptKey in
        -t|--target-host)
            vmhost="$2"
            shift
        ;;
        -k|--ssh-key)
            sshKey="$2"
            shift
        ;;
        -c|--config-dir)
            configDir="$2"
            shift
        ;;
        -b|--binary-dir)
            binarydir="$2"
            shift
        ;;
         *)
        ;;
    esac
    shift
done

chmod 600 $sshKey

binarydir="$binarydir""*"
echo "[$me] Binary Dir: $binarydir"

echo "[$me] Compressing $configDir"
COMPRESS="tar cvpf $ARCHIVENAME $binarydir"
if $COMPRESS; then
    echo "[$me] Cleaning $vmhost dir..."
    ssh -o StrictHostKeyChecking=no -i $sshKey stardock@$vmhost "mkdir $configDir; cd $configDir; rm -rf *"
    echo "[$me] Deploying $ARCHIVENAME to stardock@$vmhost:$configDir ..."
    scp -oStrictHostKeyChecking=no -i $sshKey $ARCHIVENAME stardock@$vmhost:$configDir
    echo "[$me] Extracting..."
    ssh -oStrictHostKeyChecking=no -i $sshKey stardock@$vmhost "cd $configDir; tar -xvf $ARCHIVENAME; ls -l $configDir"
    echo "[$me] Completed"
    exit 0
else
    echo "[$me] Could not Archive folder."
    exit 1
fi
