# Deployment Resources

This directory contains resources and scripts for deploying the 7 Days to Die server to cloud environments. The deployment automation handles infrastructure provisioning, server installation, and initial configuration.

## Current Support

- **Microsoft Azure**: Complete deployment solution using ARM templates and PowerShell scripts
- **Future Platforms**: Designed to be extensible for other cloud providers

## Deployment Features

- **Infrastructure as Code**: Automated resource provisioning using ARM templates
- **Configuration Management**: Template-based configuration system for easy customization
- **Automated Installation**: Server software is automatically installed and configured during deployment
- **Security**: Network security groups and proper firewall configuration included

## Directory Structure

- `Azure/` - Microsoft Azure deployment resources including ARM templates, configuration files, and PowerShell scripts

## Deployment Process

1. **Configure**: Set up your deployment parameters in the configuration files
2. **Deploy**: Run the deployment scripts to create Azure resources
3. **Wait**: The server installation happens automatically in the background
4. **Connect**: Once complete, players can connect to your dedicated server

The entire process typically takes 10-15 minutes from start to finish, depending on Azure resource provisioning time and download speeds.
