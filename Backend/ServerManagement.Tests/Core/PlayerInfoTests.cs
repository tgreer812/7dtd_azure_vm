using ServerManagement.Core.Models;

namespace ServerManagement.Tests.Core;

public class PlayerInfoTests
{
    [Fact]
    public void PlayerInfo_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var playerInfo = new PlayerInfo();

        // Assert
        Assert.Equal("", playerInfo.Name);
        Assert.False(playerInfo.IsOnline);
    }

    [Theory]
    [InlineData("TestPlayer", true)]
    [InlineData("AnotherPlayer", false)]
    [InlineData("", true)]
    public void PlayerInfo_ShouldAcceptAllPropertyValues(string name, bool isOnline)
    {
        // Arrange & Act
        var playerInfo = new PlayerInfo
        {
            Name = name,
            IsOnline = isOnline
        };

        // Assert
        Assert.Equal(name, playerInfo.Name);
        Assert.Equal(isOnline, playerInfo.IsOnline);
    }
}