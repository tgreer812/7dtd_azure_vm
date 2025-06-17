# Azure Functions ARM Templates

This directory contains Azure Resource Manager (ARM) templates for deploying Azure Functions infrastructure for the 7 Days to Die project.

## Important Notice

**Do not modify files in this directory directly.** These ARM templates are designed to work with the configuration system. Instead, modify the `private_parameters.json` file in this directory to customize your deployment.

## Template Files

- `template.json` - Main ARM template for Azure Functions resources
- `parameters.json` - Parameter mappings for the ARM template
- `private_parameters.template.json` - Template for sensitive/private parameters (copy to `private_parameters.json` and fill in values)
- `private_parameters.json` - Your customized private configuration (should be gitignored)

## What Gets Deployed

- **Azure Function App**: The main compute resource for running serverless functions
- **Storage Account**: For function app storage
- **Hosting Plan**: Defines the compute plan for the function app
- **User Assigned Identity**: For secure resource access

## Deployment Process

The ARM templates are processed by the PowerShell deployment script. The templates use parameters from `private_parameters.json` to customize the deployment according to your specifications.

## Customization

To customize the deployment:
1. Copy `private_parameters.template.json` to `private_parameters.json`
2. Edit the values in `private_parameters.json` to match your requirements
3. Run the deployment script from the `Scripts` directory
