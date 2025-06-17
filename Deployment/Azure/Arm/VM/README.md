# ARM Templates

This directory contains Azure Resource Manager (ARM) templates for deploying the 7 Days to Die server infrastructure on Microsoft Azure.

## Important Notice

**Do not modify files in this directory directly.** These ARM templates are designed to work with the configuration system. Instead, modify the `private_parameters.json` file in this directory to customize your deployment.

## Template Files

- `template.json` - Main ARM template that defines the Azure resources (VM, networking, storage, security groups)
- `parameters.json` - Parameter mappings that link the ARM template to the deployment
- `private_parameters.template.json` - Template for sensitive/private parameters (copy to `private_parameters.json` and fill in values)
- `private_parameters.json` - Your customized private configuration (should be gitignored)

## What Gets Deployed

The ARM template creates the following Azure resources:

- **Virtual Machine**: Linux VM configured for running the game server
- **Virtual Network**: Isolated network environment for the server
- **Network Security Group**: Firewall rules for game server ports
- **Public IP Address**: Static IP for external player connections
- **Storage Account**: Managed disk storage for the VM
- **Network Interface**: VM network connectivity

## Deployment Process

The ARM templates are automatically processed by the PowerShell deployment script. The templates use parameters from `private_parameters.json` to customize the deployment according to your specifications.

## Customization

To customize the deployment:
1. Copy `private_parameters.template.json` to `private_parameters.json`
2. Edit the values in `private_parameters.json` to match your requirements
3. Run the deployment script from the `Scripts` directory