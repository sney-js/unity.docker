#!/bin/bash -e
#Deployment script for Nagios configs.

me="$(basename "$(test -L "$0" && readlink "$0" || echo "$0")")"

ARCHIVENAME="SpaceDocker.tar"
COMPRESS="tar cvpf $ARCHIVENAME binaries/*"
SSH="ssh stardock@10.2.21.63"
vmhost="10.2.21.63"
sshKey=keyfile
configDir="binaries" #/Snehil.Docker.game_SpaceDocker_Game_BuildDocker_md5.checksum ?

while [ $# > 1 ]
do
    scriptKey="$1"
    shift

    case $scriptKey in
        -k|--ssh-key)
        sshKey="$1"
        shift
        ;;
         *)
        break
        ;;
    esac
done

if $COMPRESS; then
    echo "... compressed binary. Connecting to server..."
    echo "[$me] Cleaning staging dir..."
    ssh -o StrictHostKeyChecking=no -i $sshKey stardock@$vmhost "cd $configDir; rm -rf staging/*"
    echo "[$me] Deploying $archiveName to stardock@$vmhost:$configDir ..."
    scp -o StrictHostKeyChecking=no -i $sshKey $archiveName stardock@$vmhost:$configDir/staging
    #echo "[$me] Verifying configuration..."
    #ssh -o StrictHostKeyChecking=no -i $sshKey stardock@$vmhost "cd $configDir/staging; tar -xvf $archiveName"
    echo "[$me] Completed"
fi







# Defaults
#Please don't modify them directly. Use script keys instead. 
#Example: DeployScript.sh --ssh-key nagios_key --file nagios-config.tar --target-host ldnxvmon02.emea.akqa.local --config-dir /etc/nagios3





# me="$(basename "$(test -L "$0" && readlink "$0" || echo "$0")")"

# while [ $# > 1 ]
# do
# scriptKey="$1"
# shift

# case $scriptKey in
#     -k|--ssh-key)
#     sshKey="$1"
#     shift
#     ;;
#     -f|--file)
#     archiveName="$1"
#     shift
#     ;;
#     -t|--target-host)
#     vmhost="$1"
#     shift
#     ;;
#     -c|--config-dir)
#     configDir="$1"
#     shift
#     ;;
# 	-h|--help)
#     echo "[-k|--ssh-key] ssh_key - specify ssh key to use"
# 	echo "[-f|--file] file.tar - specify file to deploy"
# 	echo "[-t|--target-host] hostname - specify target nagios host deploy"
# 	echo "[-c|--config-dir] file.tar - specify nagios3 config dir. By default will be used /etc/nagios3 dir"
#     exit 0
#     ;;
#     *)
# 	break
#     ;;
# esac
# done

# # Hack to fix WARNING: UNPROTECTED PRIVATE KEY FILE! error
# chmod 600 $sshKey


# echo "[$me] Cleaning staging dir..."
# ssh -oStrictHostKeyChecking=no -i $sshKey nagios@$nagiosHost "cd $configDir; rm -rf staging/*"
# echo "[$me] Deploying $archiveName to nagios@$nagiosHost:$configDir ..."
# scp -oStrictHostKeyChecking=no -i $sshKey $archiveName nagios@$nagiosHost:$configDir/staging
# echo "[$me] Verifying configuration..."
# ssh -oStrictHostKeyChecking=no -i $sshKey nagios@$nagiosHost "cd $configDir/staging; tar -xvf $archiveName; rm conf.d/pnp4nagios.cfg; ln -s /etc/pnp4nagios/nagios.cfg conf.d/pnp4nagios.cfg; cd go; sh go_verify_config.sh"
# echo "[$me] Compleated"