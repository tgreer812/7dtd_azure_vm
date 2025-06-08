# 7 Days to Die Server Management Backend

Backend solution for managing a 7 Days to Die game server running on Azure VM.

## Solution Structure

- **ServerManagement.Core**: Core domain models and interfaces
- **ServerManagement.Azure**: Azure-specific implementations for VM and game server management
- **ServerManagment.WebApi**: ASP.NET Core Web API exposing the HTTP endpoints
  (replaces the deprecated **ServerManagement.Functions** project)
- **Scripts**: PowerShell deployment scripts

## Configuration

The solution uses environment-based configuration. See [ServerManagment.WebApi](ServerManagment.WebApi) for detailed configuration instructions.

## Local Development

1. Ensure prerequisites are installed:
   - .NET 8 SDK
   - Azure CLI

2. Set up local configuration:
   - Copy `ServerManagment.WebApi/appsettings.Development.json.example` to `appsettings.Development.json` in the same folder
   - (Optional) copy `ServerManagment.WebApi/appsettings.json.example` to `appsettings.json` for production values
   - Update the files with your Azure and game server details

3. Run the Web API:
   ```bash
   cd ServerManagment.WebApi
   dotnet run
   ```

## Deployment

Use the provided PowerShell script to deploy to Azure:

```powershell
cd Scripts
.\publish_functions.ps1 -FunctionAppName "your-function-app" -ResourceGroupName "your-rg"
```

## Architecture

The solution follows clean architecture principles:

1. **Core Layer** (`ServerManagement.Core`): 
   - Defines interfaces and domain models
   - No external dependencies

2. **Infrastructure Layer** (`ServerManagement.Azure`):
   - Implements Azure VM management using Azure SDK
   - Implements game server communication via Telnet

3. **API Layer** (`ServerManagment.WebApi`):
   - Exposes HTTP endpoints via ASP.NET Core Web API
   - Handles dependency injection and configuration

## Security Considerations

- Use Managed Identity for Azure resource access in production
- Store sensitive configuration in Azure Key Vault
- Enable authentication on Function endpoints for production use
- Restrict network access to game server telnet port