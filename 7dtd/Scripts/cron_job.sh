#!/bin/bash

LOG_FILE="/log/server_monitor.log"

# Check if the server is running
if ! pgrep -f 7DaysToDieServer.x86_64 > /dev/null; then
    echo "$(date): Server not running, starting it now..." >> "$LOG_FILE"
    /7dtd/start_server.sh &
else
    echo "$(date): Server is already running." >> "$LOG_FILE"
fi

