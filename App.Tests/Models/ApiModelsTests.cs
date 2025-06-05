using SevenDTDWebApp.Models;
using Xunit;

namespace App.Tests.Models;

public class ApiModelsTests
{
    [Fact]
    public void VmStatus_DefaultValues_AreSetCorrectly()
    {
        // Act
        var vmStatus = new VmStatus();

        // Assert
        Assert.Equal(VmState.Deallocated, vmStatus.VmState);
        Assert.Null(vmStatus.GamePortOpen);
    }

    [Fact]
    public void VmStatus_PropertiesCanBeSet()
    {
        // Act
        var vmStatus = new VmStatus
        {
            VmState = VmState.Running,
            GamePortOpen = true
        };

        // Assert
        Assert.Equal(VmState.Running, vmStatus.VmState);
        Assert.True(vmStatus.GamePortOpen);
    }

    [Theory]
    [InlineData(VmState.Deallocated)]
    [InlineData(VmState.Deallocating)]
    [InlineData(VmState.Starting)]
    [InlineData(VmState.Running)]
    [InlineData(VmState.Stopping)]
    [InlineData(VmState.Stopped)]
    public void VmState_AllEnumValues_AreValid(VmState vmState)
    {
        // Act & Assert
        Assert.True(Enum.IsDefined(typeof(VmState), vmState));
    }

    [Fact]
    public void GameServerInfo_DefaultValues_AreSetCorrectly()
    {
        // Act
        var gameServerInfo = new GameServerInfo();

        // Assert
        Assert.Equal("", gameServerInfo.Version);
        Assert.Equal(default(DateTime), gameServerInfo.ServerTimeUtc);
        Assert.Equal(0, gameServerInfo.InGameSeconds);
        Assert.Equal(0, gameServerInfo.InGameDay);
        Assert.Equal(0, gameServerInfo.TimeScale);
        Assert.Equal(0, gameServerInfo.DayStartHour);
        Assert.Equal(0, gameServerInfo.NightStartHour);
    }

    [Fact]
    public void GameServerInfo_PropertiesCanBeSet()
    {
        // Arrange
        var serverTime = DateTime.UtcNow;

        // Act
        var gameServerInfo = new GameServerInfo
        {
            Version = "Alpha 21.1",
            ServerTimeUtc = serverTime,
            InGameSeconds = 52320,
            InGameDay = 14,
            TimeScale = 30,
            DayStartHour = 6,
            NightStartHour = 18
        };

        // Assert
        Assert.Equal("Alpha 21.1", gameServerInfo.Version);
        Assert.Equal(serverTime, gameServerInfo.ServerTimeUtc);
        Assert.Equal(52320, gameServerInfo.InGameSeconds);
        Assert.Equal(14, gameServerInfo.InGameDay);
        Assert.Equal(30, gameServerInfo.TimeScale);
        Assert.Equal(6, gameServerInfo.DayStartHour);
        Assert.Equal(18, gameServerInfo.NightStartHour);
    }

    [Fact]
    public void PlayerInfo_DefaultValues_AreSetCorrectly()
    {
        // Act
        var playerInfo = new PlayerInfo();

        // Assert
        Assert.Equal("", playerInfo.Name);
        Assert.False(playerInfo.IsOnline);
    }

    [Fact]
    public void PlayerInfo_PropertiesCanBeSet()
    {
        // Act
        var playerInfo = new PlayerInfo
        {
            Name = "TestPlayer",
            IsOnline = true
        };

        // Assert
        Assert.Equal("TestPlayer", playerInfo.Name);
        Assert.True(playerInfo.IsOnline);
    }

    [Fact]
    public void ApiErrorResponse_DefaultValues_AreSetCorrectly()
    {
        // Act
        var errorResponse = new ApiErrorResponse();

        // Assert
        Assert.Equal("", errorResponse.Code);
        Assert.Equal("", errorResponse.Message);
        Assert.Null(errorResponse.Details);
    }

    [Fact]
    public void ApiErrorResponse_PropertiesCanBeSet()
    {
        // Act
        var errorResponse = new ApiErrorResponse
        {
            Code = "TEST_ERROR",
            Message = "Test error message",
            Details = "Additional details"
        };

        // Assert
        Assert.Equal("TEST_ERROR", errorResponse.Code);
        Assert.Equal("Test error message", errorResponse.Message);
        Assert.Equal("Additional details", errorResponse.Details);
    }
}