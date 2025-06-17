#!/bin/bash

STEAMCMD_DIR=/opt/steamcmd
SERVER_DIR=/7dtd
SERVER_SAVES_DIR=$SERVER_DIR/ServerSaves
LOG_DIR=/log
LOG_FILE="$LOG_DIR/install_server_$(date +%Y%m%d_%H%M%S).log"

# Redirect all output to the log file
mkdir -p "$LOG_DIR"       # Ensure the log directory exists
exec > $LOG_FILE 2>&1

echo "Installing server to $SERVER_DIR"
$STEAMCMD_DIR/steamcmd.sh +login anonymous +force_install_dir $SERVER_DIR +app_update 294420 validate +quit

# Delete the start script that comes by default when the server is installed
# The inital_setup.sh script puts a custom one there
rm -f $SERVER_DIR/startserver.sh

# Update the serverconfig.xml file so that the world saves go into the $SERVER_SAVES_DIR
echo "Updating serverconfig.xml with custom saves directory"
if [ -f "$SERVER_DIR/serverconfig.xml" ]; then
  # Check if the UserDataFolder property is commented out
  if grep -q "<!-- *<property name=\"UserDataFolder\"" "$SERVER_DIR/serverconfig.xml"; then
    # Uncomment and update the UserDataFolder property
    sed -i 's|<!-- *<property name="UserDataFolder".*-->|<property name="UserDataFolder" value="'"$SERVER_SAVES_DIR"'" />|' "$SERVER_DIR/serverconfig.xml"
  else
    # Update existing UserDataFolder property
    sed -i 's|<property name="UserDataFolder" value=".*"|<property name="UserDataFolder" value="'"$SERVER_SAVES_DIR"'"|' "$SERVER_DIR/serverconfig.xml"
  fi
  echo "Set UserDataFolder to $SERVER_SAVES_DIR in serverconfig.xml"
else
  echo "Warning: serverconfig.xml not found at $SERVER_DIR/serverconfig.xml"
fi

# Setup the cron job so the server runs automatically
echo "Setting up cron job for automatic server startup"
CRON_JOB="* * * * * /7dtd/cron_job.sh"
if ! crontab -l 2>/dev/null | grep -Fxq "$CRON_JOB"; then
    ( crontab -l 2>/dev/null; echo "$CRON_JOB" ) | crontab -
    echo "Added cron job: $CRON_JOB"
else
    echo "Cron job already exists: $CRON_JOB"
fi

# Not sure if this will be used, but gives us a way to programatically check when the server is finished installing
echo "true" > $SERVER_DIR/finished_installing.txt

echo "Installation complete"