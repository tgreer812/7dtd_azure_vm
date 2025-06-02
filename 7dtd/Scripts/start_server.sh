#!/bin/bash

STEAMCMD_DIR="/opt/steamcmd"
SERVER_DIR="/7dtd"
SERVER_EXEC="$SERVER_DIR/7DaysToDieServer.x86_64"
LOG_DIR="/log"

# Generate timestamp for the log file
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
SERVER_LOG_FILE="$LOG_DIR/server_log_${TIMESTAMP}.txt"

echo "Server output will be logged to $SERVER_LOG_FILE" >> $SERVER_LOG_FILE

# Set LD_LIBRARY_PATH to include the directory containing steamclient.so
export LD_LIBRARY_PATH="$STEAMCMD_DIR/linux64:$LD_LIBRARY_PATH"

# Use exec to replace the shell process with the server process
# -logfile: Specifies the log file path.
# -quit, -batchmode, -nographics: Standard dedicated server flags.
$SERVER_EXEC \
    -logfile "$SERVER_LOG_FILE" \
    -quit \
    -batchmode \
    -nographics \
    -dedicated &