# ARM Templates

This directory contains Azure Resource Manager (ARM) templates for deploying the 7 Days to Die server infrastructure on Microsoft Azure.

## Important Notice

**Do not modify files in this directory directly.** These ARM templates are designed to work with the configuration system. Instead, modify the `global_config.json` file in the `Config` directory to customize your deployment.

## Template Files

- `template.json` - Main ARM template that defines the Azure resources (VM, networking, storage, security groups)
- `parameters.json` - Parameter mappings that link the ARM template to the global configuration

## What Gets Deployed

The ARM template creates the following Azure resources:

- **Virtual Machine**: Linux VM configured for running the game server
- **Virtual Network**: Isolated network environment for the server
- **Network Security Group**: Firewall rules for game server ports
- **Public IP Address**: Static IP for external player connections
- **Storage Account**: Managed disk storage for the VM
- **Network Interface**: VM network connectivity

## Deployment Process

The ARM templates are automatically processed by the PowerShell deployment script. The templates use parameters from `global_config.json` to customize the deployment according to your specifications.

## Customization

To customize the deployment:
1. Navigate to the `Config` directory
2. Copy `global_config.template.json` to `global_config.json`
3. Edit the values in `global_config.json` to match your requirements
4. Run the deployment script from the `Scripts` directory