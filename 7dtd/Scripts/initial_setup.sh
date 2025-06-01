#!/bin/bash

mkdir /log
mkdir /7dtd
mkdir /data

STEAMCMD_DIR=/opt/steamcmd
mkdir $STEAMCMD_DIR

# make log file variable that is 'initial_setup_<timestamp>.log'
log_file="/log/initial_setup_$(date +%Y%m%d_%H%M%S).log"

echo "Executing initial_setup script from directory: $(pwd)" >> $log_file

echo "Updating apt-get packages and installing basic tools" >> $log_file
apt-get update && apt-get install -y \
    wget \
    unzip \
    lib32gcc-s1 \
    && rm -rf /var/lib/apt/lists/*

echo "Pulling steamcmd" >> $log_file
wget wget https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz -O /tmp/steamcmd.tar.gz >> $log_file

echo "Unpacking steamcmd to $STEAMCMD_DIR" >> $log_file
tar -xvzf /tmp/steamcmd.tar.gz -C $STEAMCMD_DIR >> $log_file

echo "Removing tempfiles" >> $log_file
rm /tmp/steamcmd.tar.gz

# TODO: Update this to main once it's merged
echo "Pulling install_server.sh" >> $log_file
wget https://raw.githubusercontent.com/tgreer812/7dtd_azure_vm/refs/heads/setup/7dtd/Scripts/install_server.sh -O install_server.sh

echo "Running install_server.sh" >> $log_file
chmod +x ./install_server.sh
./install_server.sh >> $log_file


