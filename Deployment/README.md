# Deployment

This directory contains resources and instructions for deploying the 7 Days to Die server and related Azure resources.

## Azure Deployment Structure

- `Azure/Arm/VM` - ARM templates and private config for VM deployment
- `Azure/Arm/Functions` - ARM templates and private config for Azure Functions deployment
- `Azure/Scripts` - PowerShell scripts for deploying resources

## Deployment Steps

1. Copy the relevant `private_parameters.template.json` to `private_parameters.json` in either `Azure/Arm/VM` or `Azure/Arm/Functions`.
2. Edit the values in your `private_parameters.json` file(s).
3. Run the appropriate deployment script from the `Azure/Scripts` directory:
   - For VM: `./deploy_vm.ps1`
   - For Functions: `./deploy_functions.ps1`

## Security

- The `private_parameters.json` files are gitignored and should not be committed.
- Store secrets securely and do not share your private config files.

## Notes

- Make sure you have the Azure CLI installed and are logged in before running these scripts.
- See the `README.md` files in each subdirectory for more details.
