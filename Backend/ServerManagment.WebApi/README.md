# ServerManagment.WebApi

ASP.NET Core Web API providing HTTP endpoints to manage a 7 Days to Die server running on an Azure VM. This project replaces the earlier `ServerManagement.Functions` Azure Functions app but reuses the same business logic.

## Configuration

Configuration is loaded from environment variables using the same naming convention as the Functions project.

### Environment Variables

| Variable Name | Description |
|---------------|-------------|
| `AzureVmConfiguration__SubscriptionId` | Azure Subscription ID |
| `AzureVmConfiguration__ResourceGroupName` | Resource Group containing the VM |
| `AzureVmConfiguration__VmName` | Name of the Azure VM |
| `GameServerConfiguration__Host` | Game server hostname or IP |
| `GameServerConfiguration__Port` | Game server port |
| `GameServerConfiguration__TelnetPort` | Telnet admin port |
| `GameServerConfiguration__AdminPassword` | Telnet admin password |

### Local Development

1. Copy `appsettings.Development.json.example` to `appsettings.Development.json` and fill in your local values.
2. (Optional) copy `appsettings.json.example` to `appsettings.json` for production settings.
3. Run the API:

```bash
cd ServerManagment.WebApi
dotnet run
```

Swagger UI will be available at `http://localhost:5000/swagger` by default.

### Available Endpoints

- `GET /api/vm/status` - Returns VM state and game port status
- `POST /api/vm/start` - Starts the VM
- `POST /api/vm/stop` - Stops the VM
- `POST /api/vm/restart` - Restarts the VM
- `GET /api/game/info` - Returns game server information
- `GET /api/game/players` - Returns list of online players

