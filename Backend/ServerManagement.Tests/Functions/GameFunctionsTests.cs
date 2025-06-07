using Microsoft.Extensions.Logging;
using Moq;
using ServerManagement.Core.Exceptions;
using ServerManagement.Core.Interfaces;
using ServerManagement.Core.Models;
using ServerManagement.Functions.Functions;

namespace ServerManagement.Tests.Functions;

public class GameFunctionsTests
{
    private readonly Mock<ILogger<GameFunctions>> _mockLogger;
    private readonly Mock<IServerManager> _mockServerManager;
    private readonly GameFunctions _gameFunctions;

    public GameFunctionsTests()
    {
        _mockLogger = new Mock<ILogger<GameFunctions>>();
        _mockServerManager = new Mock<IServerManager>();
        _gameFunctions = new GameFunctions(_mockLogger.Object, _mockServerManager.Object);
    }

    [Fact]
    public void GameFunctions_ShouldInitializeWithProperDependencies()
    {
        // Arrange & Act & Assert
        Assert.NotNull(_gameFunctions);
    }

    [Fact]
    public void GameFunctions_ShouldThrowIfNullLogger()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GameFunctions(null!, _mockServerManager.Object));
    }

    [Fact]
    public void GameFunctions_ShouldThrowIfNullServerManager()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GameFunctions(_mockLogger.Object, null!));
    }

    [Fact]
    public async Task GetGameInfo_ShouldReturnGameInfo_WhenServerManagerSucceeds()
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
        _mockServerManager.Setup(x => x.GetGameInfoAsync()).ReturnsAsync(expectedGameInfo);

        // Act
        var actualGameInfo = await _mockServerManager.Object.GetGameInfoAsync();

        // Assert
        Assert.Equal(expectedGameInfo.Version, actualGameInfo.Version);
        Assert.Equal(expectedGameInfo.InGameSeconds, actualGameInfo.InGameSeconds);
        Assert.Equal(expectedGameInfo.InGameDay, actualGameInfo.InGameDay);
        Assert.Equal(expectedGameInfo.TimeScale, actualGameInfo.TimeScale);
        Assert.Equal(expectedGameInfo.DayStartHour, actualGameInfo.DayStartHour);
        Assert.Equal(expectedGameInfo.NightStartHour, actualGameInfo.NightStartHour);
    }

    [Fact]
    public async Task GetPlayers_ShouldReturnPlayersList_WhenServerManagerSucceeds()
    {
        // Arrange
        var expectedPlayers = new List<PlayerInfo>
        {
            new() { Name = "Avarice", IsOnline = true },
            new() { Name = "Madmanmatt", IsOnline = true },
            new() { Name = "J3ster", IsOnline = false }
        };
        _mockServerManager.Setup(x => x.GetPlayersAsync()).ReturnsAsync(expectedPlayers);

        // Act
        var actualPlayers = await _mockServerManager.Object.GetPlayersAsync();

        // Assert
        Assert.Equal(expectedPlayers.Count, actualPlayers.Count);
        for (int i = 0; i < expectedPlayers.Count; i++)
        {
            Assert.Equal(expectedPlayers[i].Name, actualPlayers[i].Name);
            Assert.Equal(expectedPlayers[i].IsOnline, actualPlayers[i].IsOnline);
        }
    }

    [Fact]
    public async Task GetGameInfo_ShouldHandleGameServerUnreachableException()
    {
        // Arrange
        var expectedException = new GameServerUnreachableException("Game server is unreachable");
        _mockServerManager.Setup(x => x.GetGameInfoAsync()).ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<GameServerUnreachableException>(
            () => _mockServerManager.Object.GetGameInfoAsync());
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GetPlayers_ShouldHandleGameServerUnreachableException()
    {
        // Arrange
        var expectedException = new GameServerUnreachableException("Game server is unreachable");
        _mockServerManager.Setup(x => x.GetPlayersAsync()).ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<GameServerUnreachableException>(
            () => _mockServerManager.Object.GetPlayersAsync());
        Assert.Equal(expectedException.Message, actualException.Message);
    }

    [Fact]
    public async Task GetGameInfo_ShouldCallServerManagerOnce()
    {
        // Arrange
        var gameInfo = new GameServerInfo();
        _mockServerManager.Setup(x => x.GetGameInfoAsync()).ReturnsAsync(gameInfo);

        // Act
        await _mockServerManager.Object.GetGameInfoAsync();

        // Assert
        _mockServerManager.Verify(x => x.GetGameInfoAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPlayers_ShouldCallServerManagerOnce()
    {
        // Arrange
        var players = new List<PlayerInfo>();
        _mockServerManager.Setup(x => x.GetPlayersAsync()).ReturnsAsync(players);

        // Act
        await _mockServerManager.Object.GetPlayersAsync();

        // Assert
        _mockServerManager.Verify(x => x.GetPlayersAsync(), Times.Once);
    }
}