#!/bin/bash

# Get the directory where this script is located
SCRIPT_DIR="$(dirname "${BASH_SOURCE[0]}")"
AZURE_DIR="$(realpath "$SCRIPT_DIR/..")"
ARM_DIR="$(realpath "$AZURE_DIR/Arm")"
CONFIG_DIR="$(realpath "$AZURE_DIR/Config")"
TEMPLATE_FILE="$(realpath "$ARM_DIR/template.json")"
GLOBAL_CONFIG_FILE="$(realpath "$CONFIG_DIR/global_config.json")"
PARAMETERS_FILE="@$(realpath "$ARM_DIR/parameters.json")"
RESOURCE_GROUP="tg-7d2d-dedicated"

echo "ARM Directory: $ARM_DIR"
echo "Template File: $TEMPLATE_FILE"
echo "Parameters File: $PARAMETERS_FILE"

if [ ! -f "$GLOBAL_CONFIG_FILE" ]; then
    echo "ERROR: A global configuration file named 'global_config.json' is expected at: $GLOBAL_CONFIG_FILE" >&2
    exit 1
fi

# Save a list of all existing resource groups
RES_GROUPS=$(az group list --query "[].name" -o tsv)

# Check if the resource group exists
if echo "$RES_GROUPS" | grep -q "^$RESOURCE_GROUP$"; then
    echo "Resource group '$RESOURCE_GROUP' already exists."
else
    echo "Resource group '$RESOURCE_GROUP' does not exist. Creating..."
    az group create --name "$RESOURCE_GROUP" --location "eastus"
fi

az deployment group create \
    --resource-group "$RESOURCE_GROUP" \
    --template-file "$TEMPLATE_FILE" \
    --parameters "$PARAMETERS_FILE" \
    --parameters "$GLOBAL_CONFIG_FILE"

echo "Resources have been provisioned. Server may still be installing in the background"
az network public-ip show --resource-group "$RESOURCE_GROUP" --name 7dtd-pip --query "{fqdn:dnsSettings.fqdn,address: ipAddress}"