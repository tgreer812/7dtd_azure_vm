#!/bin/bash

STEAMCMD_DIR=/opt/steamcmd
SERVER_DIR=/7dtd

echo "Installing server to $SERVER_DIR"
$STEAMCMD_DIR/steamcmd.sh +login anonymous +force_install_dir $SERVER_DIR +app_update 294420 validate +quit