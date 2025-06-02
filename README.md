# 7 Days to Die Azure VM

This project provides a complete solution for deploying and managing a 7 Days to Die dedicated server on Microsoft Azure Virtual Machines. It includes ARM (Azure Resource Manager) templates for infrastructure deployment and shell scripts for server management.

## Overview

7 Days to Die is a survival horror video game that supports dedicated server hosting. This repository automates the entire process of setting up a Linux-based dedicated server on Azure, from VM creation to server installation and configuration.

## Features

- **Automated Azure Infrastructure**: ARM templates to deploy VM, networking, and security resources
- **Server Installation Scripts**: Automated installation of SteamCMD and 7 Days to Die dedicated server
- **Server Management Tools**: Scripts for starting, stopping, and updating the game server
- **Configuration Management**: Templated configuration files for easy customization

## Repository Structure

- `7dtd/` - Server management scripts and tools for the 7 Days to Die dedicated server
- `Deployment/` - Cloud deployment resources and automation scripts
  - `Azure/` - Microsoft Azure-specific deployment templates and scripts

## Prerequisites

- Azure subscription with appropriate permissions to create resources
- Azure CLI installed and configured
- PowerShell (for deployment scripts)
- Basic knowledge of Azure Resource Manager templates

## Quick Start

1. Clone this repository
2. Navigate to `Deployment/Azure/Config/`
3. Copy `global_config.template.json` to `global_config.json` and configure your settings
4. Run the deployment script from `Deployment/Azure/Scripts/deploy.ps1`
5. Wait for the deployment to complete and the server to install automatically

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.