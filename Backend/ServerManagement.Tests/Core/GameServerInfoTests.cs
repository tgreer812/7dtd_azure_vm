using ServerManagement.Core.Models;

namespace ServerManagement.Tests.Core;

public class GameServerInfoTests
{
    [Fact]
    public void GameServerInfo_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var gameInfo = new GameServerInfo();

        // Assert
        Assert.Equal("", gameInfo.Version);
        Assert.Equal(default(DateTime), gameInfo.ServerTimeUtc);
        Assert.Equal(0, gameInfo.InGameSeconds);
        Assert.Equal(0, gameInfo.InGameDay);
        Assert.Equal(0, gameInfo.TimeScale);
        Assert.Equal(0, gameInfo.DayStartHour);
        Assert.Equal(0, gameInfo.NightStartHour);
    }

    [Fact]
    public void GameServerInfo_ShouldAcceptAllProperties()
    {
        // Arrange
        var version = "Alpha 21.1";
        var serverTime = DateTime.UtcNow;
        var inGameSeconds = 52320;
        var inGameDay = 14;
        var timeScale = 30;
        var dayStartHour = 6;
        var nightStartHour = 18;

        // Act
        var gameInfo = new GameServerInfo
        {
            Version = version,
            ServerTimeUtc = serverTime,
            InGameSeconds = inGameSeconds,
            InGameDay = inGameDay,
            TimeScale = timeScale,
            DayStartHour = dayStartHour,
            NightStartHour = nightStartHour
        };

        // Assert
        Assert.Equal(version, gameInfo.Version);
        Assert.Equal(serverTime, gameInfo.ServerTimeUtc);
        Assert.Equal(inGameSeconds, gameInfo.InGameSeconds);
        Assert.Equal(inGameDay, gameInfo.InGameDay);
        Assert.Equal(timeScale, gameInfo.TimeScale);
        Assert.Equal(dayStartHour, gameInfo.DayStartHour);
        Assert.Equal(nightStartHour, gameInfo.NightStartHour);
    }
}