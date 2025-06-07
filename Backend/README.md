# 7 Days to Die Server Management Backend

Backend solution for managing a 7 Days to Die game server running on Azure VM.

## Solution Structure

- **ServerManagement.Core**: Core domain models and interfaces
- **ServerManagement.Azure**: Azure-specific implementations for VM and game server management
- **ServerManagement.Functions**: Azure Functions HTTP API endpoints
- **Scripts**: PowerShell deployment scripts

## Configuration

The solution uses environment-based configuration. See [ServerManagement.Functions/README.md](ServerManagement.Functions/README.md) for detailed configuration instructions.

## Local Development

1. Ensure prerequisites are installed:
   - .NET 8 SDK
   - Azure Functions Core Tools v4
   - Azure CLI

2. Set up local configuration:
   - Copy `ServerManagement.Functions/local.settings.json.example` to `local.settings.json`
   - Update with your Azure and game server details

3. Run the Functions app:
   ```bash
   cd ServerManagement.Functions
   func start
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

3. **API Layer** (`ServerManagement.Functions`):
   - Exposes HTTP endpoints via Azure Functions
   - Handles dependency injection and configuration

## Security Considerations

- Use Managed Identity for Azure resource access in production
- Store sensitive configuration in Azure Key Vault
- Enable authentication on Function endpoints for production use
- Restrict network access to game server telnet port