# 7 Days to Die Server Management

This directory contains scripts and tools for managing a 7 Days to Die dedicated server on Linux systems. These scripts are designed to automate the installation, configuration, and management of the game server.

## Purpose

The scripts in this directory handle the complete lifecycle of a 7 Days to Die dedicated server:

- **Environment Setup**: Prepare the Linux system with required dependencies
- **Server Installation**: Download and install the dedicated server via SteamCMD
- **Server Management**: Start, stop, and update the game server
- **Configuration**: Apply custom server settings and configurations

## Directory Structure

- `Scripts/` - Shell scripts for server installation and management

## Server Requirements

- Linux-based system (Ubuntu/Debian recommended)
- Minimum 4GB RAM (8GB+ recommended for better performance)
- 10GB+ free disk space for server files
- Network connectivity for Steam downloads and player connections
- Appropriate firewall configuration for game ports (typically 26900-26902)

## Usage

The scripts are typically executed in sequence during initial server setup:

1. `initial_setup.sh` - Prepares the system environment
2. `install_server.sh` - Downloads and installs the game server
3. `start_server.sh` - Starts the server with configured settings
4. `update_server.sh` - Updates the server to the latest version

These scripts are designed to be run on a fresh Linux system and will create the necessary directories and download all required components automatically.