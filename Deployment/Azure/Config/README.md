# Configuration Files

This directory contains configuration templates and settings for customizing your Azure deployment of the 7 Days to Die server.

## Getting Started

1. **Copy the template**: `cp global_config.template.json global_config.json`
2. **Edit the configuration**: Open `global_config.json` in your preferred text editor
3. **Configure required values**: Fill in all placeholder values marked with `<replace-with-...>`

## Configuration Files

- `global_config.template.json` - Template file with default values and placeholders
- `global_config.json` - Your customized configuration (created by copying the template)
- `.gitignore` - Ensures your personal configuration doesn't get committed to version control

## Required Configuration

The configuration file contains parameters for:

- **VM Admin Credentials**: Username and password for accessing the Azure VM
- **Azure Resource Settings**: VM size, location, and other Azure-specific parameters
- **Network Configuration**: Security group rules and network settings

## Security Notes

- **Never commit your `global_config.json` file** - It contains sensitive information like passwords
- The `.gitignore` file in this directory prevents accidental commits of your configuration
- Use strong passwords for the VM admin account
- Consider using Azure Key Vault for production deployments

## Example Configuration

After copying the template, your `global_config.json` should look similar to:

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        "vmAdminUsername": {
            "value": "your-admin-username"
        },
        "vmAdminPassword": {
            "value": "your-secure-password"
        }
    }
}
```

Replace the placeholder values with your actual configuration before running the deployment.