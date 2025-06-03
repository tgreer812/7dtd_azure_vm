# 7 Days to Die Server Scripts

This directory contains scripts for managing a 7 Days to Die dedicated server.

## Scripts Overview

### initial_setup.sh
- Sets up the environment for the 7 Days to Die server
- Creates necessary directories (`/7dtd`, `/opt/steamcmd`, `/log`)
- Downloads and installs SteamCMD
- Launches `install_server.sh` in the background so the Azure deployment can finish quickly
- Redirects all output to a timestamped log file in `/log`

### install_server.sh
- Downloads and installs the 7 Days to Die dedicated server using SteamCMD
- Removes the default startserver.sh script (replaced by our custom version)
- Updates the serverconfig.xml file to use a custom saves directory (`/7dtd/ServerSaves`)
- Creates a marker file (`finished_installing.txt`) when installation completes
- Redirects all output to a timestamped log file in `/log`

### start_server.sh
- Starts the 7 Days to Die dedicated server
- Meant to replace the default script provided by the server installation

### update_server.sh
- Updates the 7 Days to Die dedicated server to the latest version

## Usage

These scripts are designed to be run in sequence. The Azure deployment process will:
1. Run `initial_setup.sh` via the Custom Script Extension
2. `initial_setup.sh` will launch `install_server.sh` in the background
3. The Azure deployment will complete without waiting for the server installation to finish
4. `install_server.sh` will continue installing and configuring the server in the background

## Custom Configuration

- All scripts use shared environment variables for directory paths
- Output redirection is set up at the beginning of each script using `exec > $LOG_FILE 2>&1`
- The `UserDataFolder` property in serverconfig.xml is modified to point to `/7dtd/ServerSaves`
- The server installation creates a file named `finished_installing.txt` when complete

## Common Issues

- If the server fails to start, check the log files in `/log` directory
- If the serverconfig.xml file is missing, the server installation may not have completed
- Use `tail -f /log/install_server_*.log` to monitor the installation progress
