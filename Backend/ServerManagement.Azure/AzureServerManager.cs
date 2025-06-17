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
using Microsoft.VisualBasic;
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
            var (header, response) = await SendTelnetCommandAsync(command);
            
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
            var (header, response) = await SendTelnetCommandAsync(command);
            
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

    private async Task<Tuple<string, string>> SendTelnetCommandAsync(string command)
    {
        try
        {
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(_gameConfig.Host, _gameConfig.TelnetPort);

            //
            Thread.Sleep(500);
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
                if (authResponse != null && authResponse.Contains("Logon successful", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Telnet authentication successful");
                }
                else
                {
                    _logger.LogWarning("Telnet authentication may have failed: {Response}", authResponse);
                }
            }
            
            var responseBuilder = new StringBuilder();
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                responseBuilder.AppendLine(line);
                // Break if you see a known end marker or prompt
                if (line.Contains("Press 'help'") || line.Contains("Press 'exit'"))
                {
                    // One more line past
                    responseBuilder.AppendLine(await reader.ReadLineAsync());
                    break;
                }
            }
            var header = responseBuilder.ToString();

            // Send the actual command
            _logger.LogInformation($"Executing telnet command on server: {command}");
            await writer.WriteLineAsync(command);

            // Read response with timeout-based cancellation
            responseBuilder = new StringBuilder();
            while (true)
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));
                try
                {
                    var respLine = await reader.ReadLineAsync(cts.Token);
                    if (respLine == null)
                        break;
                    responseBuilder.AppendLine(respLine);
                }
                catch (OperationCanceledException)
                {
                    // Timeout: assume response is done
                    break;
                }
            }
            var response = responseBuilder.ToString();

            // Close the connection
            _logger.LogInformation($"Closing telnet connection");
            await writer.WriteLineAsync("exit");

            return new Tuple<String, String>(header, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send telnet command: {Command}", command);
            throw new GameServerUnreachableException($"Failed to send telnet command: {command}", ex);
        }
    }

    private GameServerInfo ParseGameTimeResponse(string response)
    {
        // after doing a telnet connection the response will have the server's header in it which looks like this:
        /*

        *** Connected with 7DTD server.
        *** Server version: V 1.4 (b8) Compatibility Version: V 1.4
        *** Dedicated server only build

        Server IP:   Any
        Server port: 26900
        Max players: 8
        Game mode:   GameModeSurvival
        World:       West Sejiji Territory
        Game name:   TheBoysOnAzure
        Difficulty:  1

        Press 'help' to get a list of all commands. Press 'exit' to end session.

        2025-06-06T03:01:01 951.114 INF Executing command 'gettime' by Telnet from 71.191.20.209:58703
        */
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
            var lst = response.Split("\r\n");

            // Throw away the line that shows the command being executed
            response = lst[1];
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
        /*
        Example response:
        2025-06-06T19:07:32 58941.907 INF Executing command 'listplayers' by Telnet from 127.0.0.1:54081
        0. id=171, Avarice, pos=(-1.0, 0.0, 0.0), rot=(-4.2, 137.8, 0.0), remote=True, health=139, deaths=1, zombies=2759, players=0, score=2754, level=68, pltfmid=Steam_11111111111111111, crossid=EOS_11111111111111111111111111111111, ip=127.0.0.1, ping=11
        Total of 1 in the game
        */
        var players = new List<PlayerInfo>();

        try
        {
            // Parse response to extract player names
            // This is a simplified parser - real implementation would be more robust
            var parts = response.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            // Throw out the command

            if (parts.Length == 1)
            {
                if (parts[0].Contains("Total of 0"))
                {
                    return players;
                }
            }

            var lines = parts[1..];

            foreach (var line in lines)
            {
                // Look for lines that contain player information
                if (line.Contains("id="))
                {
                    // Extract player name - this is a simplified extraction
                    var playerName = ExtractPlayerNameFromLine(line);
                    players.Add(new PlayerInfo
                    {
                        Name = playerName,
                        IsOnline = true
                    });
                }
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
        var parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return parts[1];
    }
}