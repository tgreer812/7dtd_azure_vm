# Azure Deployment Scripts

This directory contains PowerShell scripts for deploying the 7 Days to Die server infrastructure to Microsoft Azure.

## Scripts

- `deploy.ps1`: Main deployment script that orchestrates the entire deployment process

## What the Deployment Script Does

The `deploy.ps1` script performs the following operations:

1. **Validation**: Checks for required configuration files and Azure CLI setup
2. **Resource Group Management**: Creates or updates the Azure resource group
3. **ARM Template Deployment**: Deploys the infrastructure using ARM templates
4. **Configuration Integration**: Merges your custom configuration with the deployment parameters
5. **Status Monitoring**: Provides deployment progress and completion status

## Prerequisites

Before running the deployment script, ensure you have:

- **Azure CLI**: Installed and authenticated (`az login`)
- **PowerShell**: Version 5.1 or higher (Core PowerShell 6+ recommended)
- **Azure Subscription**: With sufficient permissions to create resources
- **Configuration**: Properly configured `global_config.json` file in the Config directory

## Usage

1. Open PowerShell in the Scripts directory
2. Execute: `.\deploy.ps1`
3. Monitor the deployment progress in the console output
4. Wait for completion confirmation

## Deployment Details

- **Resource Group**: Creates a resource group named "tg-7d2d-dedicated"
- **Location**: Deploys to the region specified in your configuration
- **Cleanup**: Automatically handles resource cleanup if deployment fails
- **Logging**: Provides detailed output for troubleshooting

## Post-Deployment

After the script completes:
- The VM will be created and starting up
- The 7 Days to Die server installation begins automatically
- Server installation typically takes 5-10 minutes after VM creation
- You can connect to the server once installation is complete

Use the Azure Portal or Azure CLI to monitor the VM status and retrieve connection details.
