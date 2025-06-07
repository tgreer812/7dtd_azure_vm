# Backend API Implementation

This directory contains the backend implementation for the 7DTD server management system, following the specifications outlined in `proposal.md`.

## Project Structure

```
Backend/
├── ServerManagement.Core/           # Class library with core interfaces and models
├── ServerManagement.Azure/          # Azure specific implementation of server control
├── ServerManagement.Functions/      # Azure Functions project exposing HTTP API
├── ServerManagement.Tests/          # Unit tests for all components
└── ServerManagement.sln             # Solution file
```

## Prerequisites

To run the Azure Functions project locally, you must install the Azure Functions Core Tools to get the `func` command:

- **With npm:**
  ```powershell
  npm install -g azure-functions-core-tools@4 --unsafe-perm true
  ```
- **With Chocolatey:**
  ```powershell
  choco install azure-functions-core-tools-4
  ```
- **With winget:**
  ```powershell
  winget install Microsoft.AzureFunctionsCoreTools
  ```

### Troubleshooting: 'func' command not found after npm install
If you installed with npm and the `func` command is still not found:
1. Run `npm config get prefix` to get your global npm path (e.g., `C:\Users\<your-username>\AppData\Roaming\npm`).
2. Add this path to your system `PATH` environment variable:
   - Open System Properties > Environment Variables.
   - Edit your `Path` variable and add the npm path.
   - Click OK and restart your terminal.
3. Verify with:
   ```powershell
   func --version
   ```
If you still have issues, try installing with Chocolatey or winget as an alternative.

## Running Locally

To run the Azure Functions project locally:

1. Navigate to the `ServerManagement.Functions` directory:
   ```powershell
   cd Backend/ServerManagement.Functions
   ```
2. Copy the example settings file to create your local settings:
   ```powershell
   copy local.settings.example.json local.settings.json
   ```
   Edit `local.settings.json` as needed to provide your own configuration values (such as Azure subscription, resource group, VM name, etc.).
3. Start the Azure Functions host locally:
   ```powershell
   func start
   ```
   or, if you prefer to use dotnet (for in-process projects only):
   ```powershell
   dotnet run
   ```
   > **Note:** For .NET Isolated Worker projects, always use `func start`.
4. The Functions app will be available at the local URL shown in the terminal (e.g., http://localhost:7071).

## API Endpoints

The backend provides the following REST API endpoints:

### VM Management
- `GET /api/vm/status` - Get current VM status
- `POST /api/vm/start` - Start the VM
- `POST /api/vm/stop` - Stop the VM  
- `POST /api/vm/restart` - Restart the VM

### Game Server Management
- `GET /api/game/info` - Get game server information
- `GET /api/game/players` - Get list of players

## Configuration

Configure the Azure Functions app using environment variables or `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureVmConfiguration__SubscriptionId": "your-subscription-id",
    "AzureVmConfiguration__ResourceGroupName": "your-resource-group",
    "AzureVmConfiguration__VmName": "your-vm-name",
    "GameServerConfiguration__Host": "your-vm-ip",
    "GameServerConfiguration__Port": "26900",
    "GameServerConfiguration__TelnetPort": "8081",
    "GameServerConfiguration__AdminPassword": "your-admin-password"
  }
}
```

## Azure Functions Runtime Setting: FUNCTIONS_WORKER_RUNTIME

The `FUNCTIONS_WORKER_RUNTIME` setting in `local.settings.json` tells Azure Functions which .NET runtime model to use:

- `"dotnet"`: **In-Process Model**
  - Function code runs inside the Azure Functions host process.
  - Used for .NET 6 and earlier.
  - Tight integration with Azure Functions SDK, but limited to host-supported .NET versions.
  - Not supported for .NET 8.
- `"dotnet-isolated"`: **Isolated Worker Model**
  - Function code runs in a separate process from the host.
  - Required for .NET 7 and .NET 8 Azure Functions projects.
  - Allows use of the latest .NET features, custom middleware, and more control over startup/configuration.
  - Recommended for new projects and required for .NET 8.

**For this project (.NET 8), you must use:**
```json
"FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
```

If you use the wrong value, your functions will not be detected or run. See the [official docs](https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide) for more details.

## Required Environment Variables for Local Development

The Azure Functions backend expects the following environment variables to be set in your `local.settings.json` (under the `Values` section):

- `AzureVmConfiguration__SubscriptionId`: Your Azure subscription ID (string)
- `AzureVmConfiguration__ResourceGroupName`: The resource group name containing your VM (string)
- `AzureVmConfiguration__VmName`: The name of the VM to manage (string)
- `GameServerConfiguration__Host`: The public IP or DNS name of your 7DTD game server VM (string)
- `GameServerConfiguration__Port`: The game server port (integer, e.g. 26900)
- `GameServerConfiguration__TelnetPort`: The telnet admin port (integer, e.g. 8081)
- `GameServerConfiguration__AdminPassword`: The admin password for the game server (string)

**Where to set these:**
- For local development, add them to the `Values` section of `Backend/ServerManagement.Functions/local.settings.json`.
- For Azure deployment, set them as Application Settings in the Azure Portal for your Function App.

### Example `local.settings.json` Values Section
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureVmConfiguration__SubscriptionId": "00000000-0000-0000-0000-000000000000",
    "AzureVmConfiguration__ResourceGroupName": "my-resource-group",
    "AzureVmConfiguration__VmName": "my-7dtd-vm",
    "GameServerConfiguration__Host": "1.2.3.4",
    "GameServerConfiguration__Port": "26900",
    "GameServerConfiguration__TelnetPort": "8081",
    "GameServerConfiguration__AdminPassword": "dummy-password",
    "ServerApi__BaseUrl": "http://localhost:5000/api/",
    "Logging__LogLevel__Default": "Information"
  },
  "Host": {
    "CORS": "http://localhost:5002",
    "CORSCredentials": false
  }
}
```

Replace the dummy values with your actual Azure and game server details.

## Building and Testing

Build all projects:
```bash
dotnet build
```

Run tests:
```bash
dotnet test
```

Run tests with code coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Deployment

The Azure Functions project can be deployed to Azure using either the raw `func` command or the helper scripts in this repository.

### Using Scripts
From the repository root run:

```powershell
cd Backend/Scripts
./deploy_backend.ps1 -FunctionAppName <your-function-app-name> -ProvisionInfrastructure
```

The `-ProvisionInfrastructure` switch runs the ARM template deployment found under `Deployment/Azure/Scripts` before publishing the latest code.

### Manual `func` Command

### Bash/Linux/macOS:
```bash
cd ServerManagement.Functions
func azure functionapp publish <your-function-app-name>
```

### PowerShell/Windows:
```powershell
Set-Location ServerManagement.Functions
func azure functionapp publish <your-function-app-name>
```

## Dependencies

- .NET 8.0
- Azure SDK for .NET
- Azure Functions Worker
- xUnit and Moq for testing

## Error Handling

All endpoints return consistent error responses:

```json
{
  "code": "ERROR_CODE",
  "message": "Human readable message",
  "details": "Optional technical details"
}
```

Common HTTP status codes:
- `200 OK` - Success
- `400 Bad Request` - Invalid input
- `500 Internal Server Error` - Unexpected error
- `503 Service Unavailable` - Azure or game server unreachable