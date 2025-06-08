# DEPRECATED -- ServerManagement.Functions
**This project has been deprecated** - use the ServerManagement.WebApi instead.


Azure Functions app for managing 7 Days to Die game server running on Azure VM.

## Configuration

The Functions app uses environment-based configuration that maps to strongly-typed classes.

### Configuration Classes

- **AzureVmConfiguration**: Settings for Azure VM operations
- **GameServerConfiguration**: Settings for game server connectivity

### Environment Variables

The following environment variables must be configured:

| Variable Name | Description | Example |
|--------------|-------------|---------|
| `AzureVmConfiguration__SubscriptionId` | Azure Subscription ID | `ab355bca-21aa-4453-898a-492c0f0685f9` |
| `AzureVmConfiguration__ResourceGroupName` | Resource Group containing the VM | `TG-7D2D-DEDICATED` |
| `AzureVmConfiguration__VmName` | Name of the Azure VM | `7dtdvm` |
| `GameServerConfiguration__Host` | Game server hostname or IP | `server.7dtd.tylergreer.io` |
| `GameServerConfiguration__Port` | Game server port | `26900` |
| `GameServerConfiguration__TelnetPort` | Telnet admin port | `8081` |
| `GameServerConfiguration__AdminPassword` | Telnet admin password | `your-password` |

### Local Development

1. Create a `local.settings.json` file in the project root:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureVmConfiguration__SubscriptionId": "your-subscription-id",
    "AzureVmConfiguration__ResourceGroupName": "your-resource-group",
    "AzureVmConfiguration__VmName": "your-vm-name",
    "GameServerConfiguration__Host": "localhost",
    "GameServerConfiguration__Port": "26900",
    "GameServerConfiguration__TelnetPort": "8081",
    "GameServerConfiguration__AdminPassword": "your-password"
  }
}
```

2. Run the Functions app:
```bash
func start
```

### Production Deployment

1. Create a `prod.settings.json` file with production values (see `prod.settings.json.example`)

2. Use the deployment script:
```powershell
cd Scripts
.\publish_functions.ps1 -FunctionAppName "your-function-app" -ResourceGroupName "your-rg"
```

The script will:
- Build and publish the Functions project
- Deploy to Azure using `func azure functionapp publish`
- Upload settings from `prod.settings.json` to Azure Function App configuration

### How Configuration Works

1. **Configuration Loading**: The `Program.cs` uses `.ConfigureAppConfiguration()` to add environment variables as a configuration source
2. **Configuration Binding**: The `IConfiguration.GetSection()` method binds environment variables to configuration classes based on naming convention:
   - Double underscore (`__`) in environment variables maps to nested properties
   - Example: `AzureVmConfiguration__SubscriptionId` → `AzureVmConfiguration.SubscriptionId`
3. **Dependency Injection**: Configuration objects are registered with DI using `services.Configure<T>()`
4. **Usage**: Services receive configuration via `IOptions<T>` constructor injection

### Available Functions

- `GetVmStatus` - GET `/api/vm/status` - Returns VM state and game port status
- `StartVm` - POST `/api/vm/start` - Starts the VM
- `StopVm` - POST `/api/vm/stop` - Stops (deallocates) the VM
- `RestartVm` - POST `/api/vm/restart` - Restarts the VM
- `GetGameInfo` - GET `/api/game/info` - Returns game server information
- `GetPlayers` - GET `/api/game/players` - Returns list of online players

### Authentication

Currently configured with `AuthorizationLevel.Anonymous` for development. Update to `AuthorizationLevel.Function` for production use with function keys.

### Required Azure Resources

- Azure VM running the game server
- Azure Function App (.NET 8 Isolated)
- Service Principal or Managed Identity with permissions to manage the VM
