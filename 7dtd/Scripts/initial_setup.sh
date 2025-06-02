#!/bin/bash

STEAMCMD_DIR=/opt/steamcmd
# TODO: Update this to main once it's merged
GITHUB_SCRIPTS_URI=https://raw.githubusercontent.com/tgreer812/7dtd_azure_vm/refs/heads/setup/7dtd/Scripts
SERVER_DIR=/7dtd
LOG_DIR=/log

mkdir $LOG_DIR
mkdir $SERVER_DIR
mkdir $STEAMCMD_DIR

# make log file variable that is 'initial_setup_<timestamp>.log'
LOG_FILE="$LOG_DIR/initial_setup_$(date +%Y%m%d_%H%M%S).log"

# Redirect all output to the log file
exec > $LOG_FILE 2>&1

echo "Executing initialasdf_setup script from directory: $(pwd)"

echo "Updating apt-get packages and installing basic tools"
apt-get update && apt-get install -y \
    wget \
    unzip \
    lib32gcc-s1 \
    && rm -rf /var/lib/apt/lists/*

echo "Pulling steamcmd" >> $LOG_FILE
wget https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz -O /tmp/steamcmd.tar.gz

echo "Unpacking steamcmd to $STEAMCMD_DIR"
tar -xvzf /tmp/steamcmd.tar.gz -C $STEAMCMD_DIR

echo "Removing tempfiles"
rm /tmp/steamcmd.tar.gz

echo "Pulling install_server.sh"
wget $GITHUB_SCRIPTS_URI/install_server.sh -O install_server.sh

# Pull down the convenience script files
echo "Pulling down update script"
wget $GITHUB_SCRIPTS_URI/update_server.sh -O $SERVER_DIR/update_server.sh
chmod +x $SERVER_DIR/update_server.sh

echo "Pulling down start server script"
wget $GITHUB_SCRIPTS_URI/start_server.sh -O $SERVER_DIR/start_server.sh
chmod +x $SERVER_DIR/start_server.sh

# Install the server (runs in the background so the script can complete quickly)
echo "Running install_server.sh"
chmod +x ./install_server.sh
./install_server.sh &

