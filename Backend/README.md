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

The Azure Functions project can be deployed to Azure using:

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