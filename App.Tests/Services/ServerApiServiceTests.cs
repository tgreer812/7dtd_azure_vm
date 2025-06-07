using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SevenDTDWebApp.Models;
using SevenDTDWebApp.Services;
using Xunit;

namespace App.Tests.Services;

public class ServerApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<ServerApiService>> _loggerMock;
    private readonly IConfiguration _configuration;
    private readonly ServerApiService _serverApiService;

    public ServerApiServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://test.example.com/")
        };
        _loggerMock = new Mock<ILogger<ServerApiService>>();
        
        var configData = new Dictionary<string, string?>
        {
            ["ApiBaseUrl"] = "/api"
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _serverApiService = new ServerApiService(_httpClient, _configuration, _loggerMock.Object);
    }

    private void SetupHttpResponse<T>(string endpoint, T responseObject, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var jsonResponse = JsonSerializer.Serialize(responseObject);
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith(endpoint)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
            });
    }

    private void SetupHttpError(HttpStatusCode statusCode)
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(statusCode));
    }

    [Fact]
    public async Task GetVmStatusAsync_ReturnsVmStatus_WhenApiCallSucceeds()
    {
        // Arrange
        var expectedVmStatus = new VmStatus
        {
            VmState = VmState.Running,
            GamePortOpen = true
        };
        SetupHttpResponse("/api/vm/status", expectedVmStatus);

        // Act
        var result = await _serverApiService.GetVmStatusAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(VmState.Running, result.VmState);
        Assert.True(result.GamePortOpen);
    }

    [Fact]
    public async Task GetVmStatusAsync_ThrowsServiceException_WhenHttpRequestFails()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ServiceException>(() => _serverApiService.GetVmStatusAsync());
        Assert.Contains("Failed to retrieve VM status", exception.Message);
    }

    [Fact]
    public async Task StartVmAsync_ReturnsVmStatus_WhenApiCallSucceeds()
    {
        // Arrange
        var expectedVmStatus = new VmStatus
        {
            VmState = VmState.Starting,
            GamePortOpen = null
        };
        SetupHttpResponse("/api/vm/start", expectedVmStatus);

        // Act
        var result = await _serverApiService.StartVmAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(VmState.Starting, result.VmState);
        Assert.Null(result.GamePortOpen);
    }

    [Fact]
    public async Task StopVmAsync_ReturnsVmStatus_WhenApiCallSucceeds()
    {
        // Arrange
        var expectedVmStatus = new VmStatus
        {
            VmState = VmState.Stopped,
            GamePortOpen = null
        };
        SetupHttpResponse("/api/vm/stop", expectedVmStatus);

        // Act
        var result = await _serverApiService.StopVmAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(VmState.Stopped, result.VmState);
        Assert.Null(result.GamePortOpen);
    }

    [Fact]
    public async Task RestartVmAsync_ReturnsVmStatus_WhenApiCallSucceeds()
    {
        // Arrange
        var expectedVmStatus = new VmStatus
        {
            VmState = VmState.Starting,
            GamePortOpen = null
        };
        SetupHttpResponse("/api/vm/restart", expectedVmStatus);

        // Act
        var result = await _serverApiService.RestartVmAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(VmState.Starting, result.VmState);
        Assert.Null(result.GamePortOpen);
    }

    [Fact]
    public async Task GetGameInfoAsync_ReturnsGameServerInfo_WhenApiCallSucceeds()
    {
        // Arrange
        var expectedGameInfo = new GameServerInfo
        {
            Version = "Alpha 21.1",
            ServerTimeUtc = DateTime.UtcNow,
            InGameSeconds = 52320,
            InGameDay = 14,
            TimeScale = 30,
            DayStartHour = 6,
            NightStartHour = 18
        };
        SetupHttpResponse("/api/game/info", expectedGameInfo);

        // Act
        var result = await _serverApiService.GetGameInfoAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alpha 21.1", result.Version);
        Assert.Equal(52320, result.InGameSeconds);
        Assert.Equal(14, result.InGameDay);
        Assert.Equal(30, result.TimeScale);
        Assert.Equal(6, result.DayStartHour);
        Assert.Equal(18, result.NightStartHour);
    }

    [Fact]
    public async Task GetPlayersAsync_ReturnsPlayerList_WhenApiCallSucceeds()
    {
        // Arrange
        var expectedPlayers = new[]
        {
            new PlayerInfo { Name = "Avarice", IsOnline = true },
            new PlayerInfo { Name = "Madmanmatt", IsOnline = true },
            new PlayerInfo { Name = "J3ster", IsOnline = false }
        };
        SetupHttpResponse("/api/game/players", expectedPlayers);

        // Act
        var result = await _serverApiService.GetPlayersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Name == "Avarice" && p.IsOnline);
        Assert.Contains(result, p => p.Name == "Madmanmatt" && p.IsOnline);
        Assert.Contains(result, p => p.Name == "J3ster" && !p.IsOnline);
    }

    [Fact]
    public async Task GetPlayersAsync_ReturnsEmptyList_WhenApiReturnsNull()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().EndsWith("/api/game/players")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _serverApiService.GetPlayersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task ApiMethods_ThrowServiceException_WhenHttpStatusIsNotSuccess(HttpStatusCode statusCode)
    {
        // Arrange
        SetupHttpError(statusCode);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceException>(() => _serverApiService.StartVmAsync());
        await Assert.ThrowsAsync<ServiceException>(() => _serverApiService.StopVmAsync());
        await Assert.ThrowsAsync<ServiceException>(() => _serverApiService.RestartVmAsync());
    }

    [Fact]
    public async Task ApiMethods_ThrowServiceException_WhenJsonIsInvalid()
    {
        // Arrange
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("invalid json", System.Text.Encoding.UTF8, "application/json")
            });

        // Act & Assert
        await Assert.ThrowsAsync<ServiceException>(() => _serverApiService.GetVmStatusAsync());
        await Assert.ThrowsAsync<ServiceException>(() => _serverApiService.GetGameInfoAsync());
    }
}