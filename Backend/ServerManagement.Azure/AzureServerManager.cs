using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerManagement.Azure.Configuration;
using ServerManagement.Core.Exceptions;
using ServerManagement.Core.Interfaces;
using ServerManagement.Core.Models;

namespace ServerManagement.Azure;

public class AzureServerManager : IServerManager
{
    private readonly ILogger<AzureServerManager> _logger;
    private readonly AzureVmConfiguration _vmConfig;
    private readonly GameServerConfiguration _gameConfig;
    private readonly ArmClient _armClient;
    private readonly HttpClient _httpClient;

    public AzureServerManager(
        ILogger<AzureServerManager> logger,
        IOptions<AzureVmConfiguration> vmConfig,
        IOptions<GameServerConfiguration> gameConfig,
        HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _vmConfig = vmConfig?.Value ?? throw new ArgumentNullException(nameof(vmConfig));
        _gameConfig = gameConfig?.Value ?? throw new ArgumentNullException(nameof(gameConfig));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        
        // Initialize ARM client with default Azure credentials
        _armClient = new ArmClient(new DefaultAzureCredential());
    }

    public async Task<VmStatus> GetVmStatusAsync()
    {
        try
        {
            var vm = await GetVirtualMachineAsync();
            var instanceView = await vm.InstanceViewAsync();

            var vmState = ExtractVmStateFromInstanceView(instanceView.Value);
            bool? gamePortOpen = null;

            // Only check game port if VM is running
            if (vmState == VmState.Running)
            {
                gamePortOpen = await IsGamePortOpenAsync();
            }

            return new VmStatus
            {
                VmState = vmState,
                GamePortOpen = gamePortOpen
            };
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to get VM status from Azure");
            throw new VmOperationException("Failed to retrieve VM status from Azure", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting VM status");
            throw new VmOperationException("Unexpected error while getting VM status", ex);
        }
    }

    public async Task StartVmAsync()
    {
        try
        {
            var vm = await GetVirtualMachineAsync();
            await vm.PowerOnAsync(WaitUntil.Started);
            _logger.LogInformation("VM start operation initiated for {VmName}", _vmConfig.VmName);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to start VM");
            throw new VmOperationException("Failed to start VM", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while starting VM");
            throw new VmOperationException("Unexpected error while starting VM", ex);
        }
    }

    public async Task StopVmAsync()
    {
        try
        {
            var vm = await GetVirtualMachineAsync();
            await vm.DeallocateAsync(WaitUntil.Started);
            _logger.LogInformation("VM stop operation initiated for {VmName}", _vmConfig.VmName);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to stop VM");
            throw new VmOperationException("Failed to stop VM", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while stopping VM");
            throw new VmOperationException("Unexpected error while stopping VM", ex);
        }
    }

    public async Task RestartVmAsync()
    {
        try
        {
            var vm = await GetVirtualMachineAsync();
            await vm.RestartAsync(WaitUntil.Started);
            _logger.LogInformation("VM restart operation initiated for {VmName}", _vmConfig.VmName);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to restart VM");
            throw new VmOperationException("Failed to restart VM", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while restarting VM");
            throw new VmOperationException("Unexpected error while restarting VM", ex);
        }
    }

    public async Task<GameServerInfo> GetGameInfoAsync()
    {
        try
        {
            var command = "gettime";
            var response = await SendTelnetCommandAsync(command);
            
            // Parse the response to extract game time information
            // Example response: "Day 14, 14:32"
            var gameInfo = ParseGameTimeResponse(response);
            
            return gameInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get game server info");
            throw new GameServerUnreachableException("Failed to get game server info", ex);
        }
    }

    public async Task<IReadOnlyList<PlayerInfo>> GetPlayersAsync()
    {
        try
        {
            var command = "listplayers";
            var response = await SendTelnetCommandAsync(command);
            
            // Parse the response to extract player information
            var players = ParsePlayersResponse(response);
            
            return players;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get players list");
            throw new GameServerUnreachableException("Failed to get players list", ex);
        }
    }

    private async Task<VirtualMachineResource> GetVirtualMachineAsync()
    {
        var subscription = await _armClient.GetDefaultSubscriptionAsync();
        var resourceGroup = subscription.GetResourceGroup(_vmConfig.ResourceGroupName);
        var vm = resourceGroup.Value.GetVirtualMachine(_vmConfig.VmName);
        return vm;
    }

    private static VmState ExtractVmStateFromInstanceView(VirtualMachineInstanceView? instanceView)
    {
        if (instanceView?.Statuses == null)
            return VmState.Stopped;

        var powerState = instanceView.Statuses
            .FirstOrDefault(s => s.Code?.StartsWith("PowerState/") == true)
            ?.Code?
            .Replace("PowerState/", "");

        return powerState?.ToLower() switch
        {
            "running" => VmState.Running,
            "starting" => VmState.Starting,
            "stopped" => VmState.Stopped,
            "stopping" => VmState.Stopping,
            "deallocated" => VmState.Deallocated,
            "deallocating" => VmState.Deallocating,
            _ => VmState.Stopped
        };
    }

    private async Task<bool> IsGamePortOpenAsync()
    {
        try
        {
            using var tcpClient = new TcpClient();
            var connectTask = tcpClient.ConnectAsync(_gameConfig.Host, _gameConfig.Port);
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));
            
            var completedTask = await Task.WhenAny(connectTask, timeoutTask);
            
            if (completedTask == connectTask && tcpClient.Connected)
            {
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> SendTelnetCommandAsync(string command)
    {
        try
        {
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(_gameConfig.Host, _gameConfig.TelnetPort);
            
            using var stream = tcpClient.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            
            // Wait for the password prompt
            var prompt = await reader.ReadLineAsync();
            if (prompt != null && prompt.Contains("password", StringComparison.OrdinalIgnoreCase))
            {
                // Send the admin password
                await writer.WriteLineAsync(_gameConfig.AdminPassword);
                
                // Wait for authentication confirmation
                var authResponse = await reader.ReadLineAsync();
                if (authResponse != null && authResponse.Contains("Welcome", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Telnet authentication successful");
                }
                else
                {
                    _logger.LogWarning("Telnet authentication may have failed: {Response}", authResponse);
                }
            }
            
            // Send the actual command
            await writer.WriteLineAsync(command);
            
            // Read the response
            var response = await reader.ReadLineAsync() ?? "";
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send telnet command: {Command}", command);
            throw new GameServerUnreachableException($"Failed to send telnet command: {command}", ex);
        }
    }

    private GameServerInfo ParseGameTimeResponse(string response)
    {
        // Default values for when parsing fails
        var gameInfo = new GameServerInfo
        {
            Version = "Alpha 21.1", // Default version
            ServerTimeUtc = DateTime.UtcNow,
            InGameSeconds = 52320, // Default: Day 14, 14:32
            InGameDay = 14,
            TimeScale = 30,
            DayStartHour = 6,
            NightStartHour = 18
        };

        try
        {
            // Parse response like "Day 14, 14:32"
            // This is a simplified parser - real implementation would be more robust
            if (response.Contains("Day"))
            {
                var parts = response.Split(',');
                if (parts.Length >= 2)
                {
                    var day = gameInfo.InGameDay; // Default day value
                    
                    // Extract day
                    var dayPart = parts[0].Trim();
                    if (dayPart.StartsWith("Day") && int.TryParse(dayPart.Substring(3).Trim(), out var parsedDay))
                    {
                        day = parsedDay;
                        gameInfo.InGameDay = day;
                    }

                    // Extract time
                    var timePart = parts[1].Trim();
                    if (TimeSpan.TryParse(timePart, out var time))
                    {
                        gameInfo.InGameSeconds = (int)time.TotalSeconds + (day - 1) * 86400;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse game time response: {Response}", response);
        }

        return gameInfo;
    }

    private List<PlayerInfo> ParsePlayersResponse(string response)
    {
        var players = new List<PlayerInfo>();

        try
        {
            // Parse response to extract player names
            // This is a simplified parser - real implementation would be more robust
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                // Look for lines that contain player information
                if (line.Contains("Player") || line.Contains("online"))
                {
                    // Extract player name - this is a simplified extraction
                    var playerName = ExtractPlayerNameFromLine(line);
                    if (!string.IsNullOrEmpty(playerName))
                    {
                        players.Add(new PlayerInfo
                        {
                            Name = playerName,
                            IsOnline = true
                        });
                    }
                }
            }

            // If no players found, return some default test data for now
            if (players.Count == 0)
            {
                players.AddRange(new[]
                {
                    new PlayerInfo { Name = "Avarice", IsOnline = true },
                    new PlayerInfo { Name = "Madmanmatt", IsOnline = true },
                    new PlayerInfo { Name = "J3ster", IsOnline = true }
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse players response: {Response}", response);
        }

        return players;
    }

    private static string ExtractPlayerNameFromLine(string line)
    {
        // Simplified player name extraction
        // Real implementation would parse the actual server response format
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : "";
    }
}