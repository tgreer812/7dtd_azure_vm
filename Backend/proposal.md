# Backend API Proposal

This document outlines the proposed backend design for the 7DTD static web app. The backend will expose HTTP endpoints via Azure Functions and be implemented in C#. The goal is to provide a simple contract so the frontend can be developed independently while leaving room for future expansion.

## Guiding Principles

- **Simplicity**: Provide a minimal set of endpoints needed by the current frontend.
- **Extensibility**: Design routes and data models that can grow with new features (e.g., authentication, more server controls, metrics).
- **Testability**: Core logic should reside in a separate library that can be unit-tested with high coverage. Azure Function projects should contain only thin HTTP wrappers.
- **Reusability**: Backend logic should be reusable from command line tools or other services.
- **Polling Instead of SignalR**: Real-time connections add complexity and cost. This design relies on polling the `/api/vm/status` endpoint rather than using SignalR.

## Proposed Project Structure

```
Backend/
├── ServerManagement.Core/           # Class library with core interfaces and models
├── ServerManagement.Azure/          # Azure specific implementation of server control
└── ServerManagement.Functions/      # Azure Functions project exposing HTTP API
```

- **ServerManagement.Core** will define DTOs and the `IServerManager` interface (VM control and game queries).
- **ServerManagement.Azure** will implement `IServerManager` using Azure SDK/VM APIs and game server commands.
- **ServerManagement.Functions** will reference the core library and register an implementation (for now `ServerManagement.Azure`). Each function will simply call into the service and return results.

With this separation we can unit test `ServerManagement.Core` and `ServerManagement.Azure` without starting the function host. Azure Functions themselves can be tested with lightweight host utilities or integration tests.

## API Endpoints

The frontend currently calls placeholder methods as seen in `Index.razor`:

```csharp
statusText = await GetServerStatus();
players = await GetPlayers();
serverData = await GetServerStatusData();
```
【F:App/Pages/Index.razor†L156-L160】

The README also shows an example of calling `/api/vm/status` via `Http.GetFromJsonAsync`:

```csharp
var response = await Http.GetFromJsonAsync<ServerStatusResponse>("/api/vm/status");
```
【F:App/README.md†L160-L170】

Based on these calls the backend should provide the following endpoints. All URLs are relative to the Static Web App base (`/api` when deployed).

## VM / Infrastructure Endpoints (`/api/vm`)
These manage the Azure Virtual Machine that hosts the game server.

### 1. GET `/api/vm/status`
Returns basic VM status information. The backend is stateless and derives this data directly from Azure Resource Manager (ARM). When a user clicks **Start**, the frontend should poll this endpoint every ~10 seconds until `vmState` equals `"running"`.

**Response Schema**
```json
{
    "vmState": "starting",      // raw state from ARM e.g. "starting", "running", "stopped"
    "gamePortOpen": true
}
```
The `vmState` field comes from the VM's [`instanceView.statuses`](https://learn.microsoft.com/en-us/azure/virtual-machines/states-billing) array. Example values include `deallocated`, `deallocating`, `starting`, `running`, `stopping`, and `stopped`. Look for the entry that begins with `"PowerState/"` and strip the prefix.

The `gamePortOpen` value is optional and may be `null` if the backend does not perform port probing. When supported, it allows the frontend to wait until the game server is reachable before declaring it fully online.

Because the backend does not store session state, each call reflects the real-time state of the VM. If you need the raw data yourself you can query ARM via:

```
GET /subscriptions/{subId}/resourceGroups/{rg}/providers/Microsoft.Compute/virtualMachines/{vmName}/instanceView?api-version=2023-03-01
```
This response includes the detailed power state information the backend exposes via `vmState`.

### 2. POST `/api/vm/start`
Starts the VM. After calling this endpoint the client should poll `/api/vm/status` until the VM reports `running`.

**Response Schema**
```json
{
    "vmState": "starting",
    "gamePortOpen": null
}
```

### 3. POST `/api/vm/stop`
Stops the VM and returns the updated status.

**Response Schema**
```json
{
    "vmState": "stopped",
    "gamePortOpen": null
}
```

### 4. POST `/api/vm/restart` *(optional)*
Convenience endpoint for restarting the VM.

**Response Schema**
```json
{
    "vmState": "starting",
    "gamePortOpen": null
}
```

## Game Server / Application Endpoints (`/api/game`)
These query or control the 7 Days to Die application running on the VM.

### 1. GET `/api/game/info`
Returns detailed game state needed by the UI.

**Response Schema**
```json
{
    "inGameSeconds": 52320,
    "inGameDay": 14,
    "timeScale": 30,
    "dayStartHour": 6,
    "nightStartHour": 18
}
```
This corresponds to the `GameServerInfoDto` used in the front‑end.

### 2. GET `/api/game/players`
Returns the list of players currently known to the server.

**Response Schema**
```json
[
    { "name": "Avarice", "isOnline": true },
    { "name": "Madmanmatt", "isOnline": true },
    { "name": "J3ster", "isOnline": true }
]
```
In future we can extend the player object with additional fields (steam id, score, etc.).

### 3. GET `/api/game/logs` *(planned)*
Retrieve recent server logs. Optional `?tail=100` parameter could limit lines returned.

**Response Schema**
```text
<log lines>
```

All endpoints return JSON and use standard HTTP status codes. Additional endpoints can be introduced without breaking existing routes, keeping the contract extensible.

## Data Models (Core Library)

```csharp
public enum VmState
{
    Deallocated,
    Deallocating,
    Starting,
    Running,
    Stopping,
    Stopped
}

public class VmStatus
{
    // Maps to Azure VM PowerState (via instanceView.statuses)
    public VmState VmState { get; set; }
    public bool? GamePortOpen { get; set; }
}

public class GameServerInfo
{
    public string Version { get; set; } = "";
    public DateTime ServerTimeUtc { get; set; }
    public int InGameSeconds { get; set; }
    public int InGameDay { get; set; }
    public int TimeScale { get; set; }
    public int DayStartHour { get; set; }
    public int NightStartHour { get; set; }
}

public class PlayerInfo
{
    public string Name { get; set; } = "";
    public bool IsOnline { get; set; }
}

public interface IServerManager
{
    Task<VmStatus> GetVmStatusAsync();
    Task StartVmAsync();
    Task StopVmAsync();
    Task RestartVmAsync();
    Task<GameServerInfo> GetGameInfoAsync();
    Task<IReadOnlyList<PlayerInfo>> GetPlayersAsync();
}
```

The Azure Functions project will inject an `IServerManager` implementation and map HTTP routes to these methods.

## Testing Strategy

- **Unit Tests**: All logic in `ServerManagement.Core` and `ServerManagement.Azure` will be fully unit tested using xUnit and Moq.
- **Function Tests**: The thin HTTP layer can be tested using the `Microsoft.Azure.Functions.Worker` test host or integration tests that spin up the Functions runtime.
- **Coverage**: Enable `coverlet`/`dotnet test --collect:"XPlat Code Coverage"` in CI to ensure high coverage.
- **Mocking**: Interfaces allow mocking of Azure SDK calls so tests don’t need real cloud resources.

## Future Extensions

- **Authentication**: Add JWT or Static Web App authentication to secure POST endpoints.
- **Optional Real-Time Updates**: If push-based communication becomes necessary, we can integrate Azure SignalR Service or another approach. For now polling suffices and avoids extra infrastructure.
- **Metrics**: Expose additional endpoints for CPU/memory usage or world save management.
- **Configuration Management**: Endpoints to modify server settings or patch `serverconfig.xml`.

This design provides a clear separation of concerns and allows front‑end developers to work with mock implementations while the backend is being built. The endpoint structure leaves room for new features and can be tested automatically with high coverage.

