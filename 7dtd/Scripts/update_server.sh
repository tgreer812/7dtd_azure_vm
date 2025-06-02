#!/bin/bash

STEAMCMD_DIR="/opt/steamcmd"
SERVER_DIR="/7dtd"
LOG_DIR="/log"

# make log file variable that is 'initial_setup_<timestamp>.log'
LOG_FILE="$LOG_DIR/update_$(date +%Y%m%d_%H%M%S).log"

# redirect all output to log file
exec > $LOG_FILE 2>&1

$STEAMCMD_DIR/steamcmd.sh +login anonymous +force_install_dir "$SERVER_DIR" +app_update 294420 validate +quit

echo "Update complete"