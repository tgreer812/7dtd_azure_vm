# Web API ARM Templates

This directory contains Azure Resource Manager (ARM) templates for deploying the `ServerManagment.WebApi` application.

## Important Notice

**Do not modify these files directly.** Instead copy `private_parameters.template.json` to `private_parameters.json` and provide your environment specific values there.

## Template Files

- `template.json` - Main ARM template defining the App Service and related resources
- `parameters.json` - Example parameter values used for deployment
- `private_parameters.template.json` - Template for sensitive values (copy to `private_parameters.json`)
  - includes `adminPassword` and optional `gameServerHost`

## Deployment Process

Use the PowerShell script in `Deployment/Azure/Scripts/deploy_webapi.ps1` to deploy the Web API. The script expects `private_parameters.json` to exist in this directory.
If your game server uses a custom domain, provide it via the `-GameServerHost` parameter when running the script.

