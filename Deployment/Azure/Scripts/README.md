# Deployment Scripts

This directory contains scripts for deploying Azure resources for the 7 Days to Die project.

## Scripts

- `deploy_vm.ps1` - Deploys the VM infrastructure using ARM templates in `../Arm/VM` and config in `../Arm/VM/private_parameters.json`
- `deploy_functions.ps1` - Deploys Azure Functions infrastructure using ARM templates in `../Arm/Functions` and config in `../Arm/Functions/private_parameters.json`
- `deploy.sh` - (If present) Bash script for deployment (not updated for new structure)

## Usage

1. Copy the relevant `private_parameters.template.json` to `private_parameters.json` in the appropriate `Arm/VM` or `Arm/Functions` directory.
2. Edit the values in your `private_parameters.json` file(s).
3. Run the appropriate deployment script from this directory:
   - For VM: `./deploy_vm.ps1`
   - For Functions: `./deploy_functions.ps1`

## Notes

- The `private_parameters.json` files are gitignored and should not be committed.
- Make sure you have the Azure CLI installed and are logged in before running these scripts.
