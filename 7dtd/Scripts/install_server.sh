#!/bin/bash

STEAMCMD_DIR=/opt/steamcmd
SERVER_DIR=/7dtd
LOG_DIR=/log

LOG_FILE="$LOG_DIR/install_server_$(date +%Y%m%d_%H%M%S).log"

# Redirect all output to the log file
exec > $LOG_FILE 2>&1

echo "Installing server to $SERVER_DIR"
$STEAMCMD_DIR/steamcmd.sh +login anonymous +force_install_dir $SERVER_DIR +app_update 294420 validate +quit

# Delete the start script that comes by default when the server is installed
# The inital_setup.sh script puts a custom one there
rm -f $SERVER_DIR/startserver.sh

# Not sure if this will be used, but gives us a way to programatically check when the server is finished installing
echo "true" > $SERVER_DIR/finished_installing.txt

echo "Installation complete"