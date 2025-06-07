using SevenDTDWebApp.Models;
using Xunit;

namespace App.Tests.Models;

/// <summary>
/// Tests for API data models to ensure they properly store and retrieve values.
/// These tests verify that our data structures work correctly when receiving data from the backend API.
/// </summary>
public class ApiModelsTests
{
    [Fact]
    public void VmStatus_DefaultValues_AreSetCorrectly()
    {
        // Create a new VmStatus object to check its initial state
        var vmStatus = new VmStatus();

        // Verify that new objects start with expected default values
        Assert.Equal(VmState.Deallocated, vmStatus.VmState);
        Assert.Null(vmStatus.GamePortOpen);
    }

    [Fact]
    public void VmStatus_PropertiesCanBeSet()
    {
        // Create VmStatus with specific values to test property assignment
        var vmStatus = new VmStatus
        {
            VmState = VmState.Running,
            GamePortOpen = true
        };

        // Verify the properties were set correctly
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
        // Verify that all enum values are properly defined
        Assert.True(Enum.IsDefined(typeof(VmState), vmState));
    }

    [Fact]
    public void GameServerInfo_DefaultValues_AreSetCorrectly()
    {
        // Create a new GameServerInfo to check default initialization
        var gameServerInfo = new GameServerInfo();

        // Verify all properties start with appropriate default values
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
        var serverTime = DateTime.UtcNow;

        // Create GameServerInfo with specific game state values
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

        // Verify all properties were assigned correctly
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
        // Create a new PlayerInfo to verify initial state
        var playerInfo = new PlayerInfo();

        // Check that new player objects have sensible defaults
        Assert.Equal("", playerInfo.Name);
        Assert.False(playerInfo.IsOnline);
    }

    [Fact]
    public void PlayerInfo_PropertiesCanBeSet()
    {
        // Create PlayerInfo with test data
        var playerInfo = new PlayerInfo
        {
            Name = "TestPlayer",
            IsOnline = true
        };

        // Verify the player information was stored correctly
        Assert.Equal("TestPlayer", playerInfo.Name);
        Assert.True(playerInfo.IsOnline);
    }

    [Fact]
    public void ApiErrorResponse_DefaultValues_AreSetCorrectly()
    {
        // Create a new error response to check initialization
        var errorResponse = new ApiErrorResponse();

        // Verify error objects start with empty/null values
        Assert.Equal("", errorResponse.Code);
        Assert.Equal("", errorResponse.Message);
        Assert.Null(errorResponse.Details);
    }

    [Fact]
    public void ApiErrorResponse_PropertiesCanBeSet()
    {
        // Create error response with test error information
        var errorResponse = new ApiErrorResponse
        {
            Code = "TEST_ERROR",
            Message = "Test error message",
            Details = "Additional details"
        };

        // Verify the error information was stored properly
        Assert.Equal("TEST_ERROR", errorResponse.Code);
        Assert.Equal("Test error message", errorResponse.Message);
        Assert.Equal("Additional details", errorResponse.Details);
    }
}