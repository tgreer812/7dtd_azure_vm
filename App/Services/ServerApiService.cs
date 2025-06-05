using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SevenDTDWebApp.Models;

namespace SevenDTDWebApp.Services;

public class ServerApiService : IServerApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ServerApiService> _logger;
    private readonly string _apiBaseUrl;

    public ServerApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ServerApiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiBaseUrl = _configuration["ApiBaseUrl"] ?? "/api";
    }

    public async Task<VmStatus> GetVmStatusAsync()
    {
        try
        {
            _logger.LogInformation("Getting VM status from {Endpoint}", $"{_apiBaseUrl}/vm/status");
            var response = await _httpClient.GetFromJsonAsync<VmStatus>($"{_apiBaseUrl}/vm/status");
            return response ?? throw new InvalidOperationException("Received null response from VM status endpoint");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get VM status");
            throw new ServiceException("Failed to retrieve VM status", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse VM status response");
            throw new ServiceException("Invalid response format from VM status endpoint", ex);
        }
    }

    public async Task<VmStatus> StartVmAsync()
    {
        try
        {
            _logger.LogInformation("Starting VM via {Endpoint}", $"{_apiBaseUrl}/vm/start");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/vm/start", null);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<VmStatus>();
            return result ?? throw new InvalidOperationException("Received null response from VM start endpoint");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to start VM");
            throw new ServiceException("Failed to start VM", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse VM start response");
            throw new ServiceException("Invalid response format from VM start endpoint", ex);
        }
    }

    public async Task<VmStatus> StopVmAsync()
    {
        try
        {
            _logger.LogInformation("Stopping VM via {Endpoint}", $"{_apiBaseUrl}/vm/stop");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/vm/stop", null);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<VmStatus>();
            return result ?? throw new InvalidOperationException("Received null response from VM stop endpoint");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to stop VM");
            throw new ServiceException("Failed to stop VM", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse VM stop response");
            throw new ServiceException("Invalid response format from VM stop endpoint", ex);
        }
    }

    public async Task<VmStatus> RestartVmAsync()
    {
        try
        {
            _logger.LogInformation("Restarting VM via {Endpoint}", $"{_apiBaseUrl}/vm/restart");
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/vm/restart", null);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<VmStatus>();
            return result ?? throw new InvalidOperationException("Received null response from VM restart endpoint");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to restart VM");
            throw new ServiceException("Failed to restart VM", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse VM restart response");
            throw new ServiceException("Invalid response format from VM restart endpoint", ex);
        }
    }

    public async Task<GameServerInfo> GetGameInfoAsync()
    {
        try
        {
            _logger.LogInformation("Getting game info from {Endpoint}", $"{_apiBaseUrl}/game/info");
            var response = await _httpClient.GetFromJsonAsync<GameServerInfo>($"{_apiBaseUrl}/game/info");
            return response ?? throw new InvalidOperationException("Received null response from game info endpoint");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get game info");
            throw new ServiceException("Failed to retrieve game information", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse game info response");
            throw new ServiceException("Invalid response format from game info endpoint", ex);
        }
    }

    public async Task<IReadOnlyList<PlayerInfo>> GetPlayersAsync()
    {
        try
        {
            _logger.LogInformation("Getting players from {Endpoint}", $"{_apiBaseUrl}/game/players");
            var response = await _httpClient.GetFromJsonAsync<PlayerInfo[]>($"{_apiBaseUrl}/game/players");
            return response ?? Array.Empty<PlayerInfo>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get players");
            throw new ServiceException("Failed to retrieve player list", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse players response");
            throw new ServiceException("Invalid response format from players endpoint", ex);
        }
    }
}

public class ServiceException : Exception
{
    public ServiceException(string message) : base(message) { }
    public ServiceException(string message, Exception innerException) : base(message, innerException) { }
}