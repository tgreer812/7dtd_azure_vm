using ServerManagement.Core.Exceptions;

namespace ServerManagement.Tests.Core;

public class ExceptionTests
{
    [Fact]
    public void VmOperationException_ShouldHaveDefaultConstructor()
    {
        // Arrange & Act
        var exception = new VmOperationException();

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<VmOperationException>(exception);
    }

    [Fact]
    public void VmOperationException_ShouldAcceptMessage()
    {
        // Arrange
        var message = "Test VM operation error";

        // Act
        var exception = new VmOperationException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void VmOperationException_ShouldAcceptMessageAndInnerException()
    {
        // Arrange
        var message = "Test VM operation error";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new VmOperationException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void GameServerUnreachableException_ShouldHaveDefaultConstructor()
    {
        // Arrange & Act
        var exception = new GameServerUnreachableException();

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<GameServerUnreachableException>(exception);
    }

    [Fact]
    public void GameServerUnreachableException_ShouldAcceptMessage()
    {
        // Arrange
        var message = "Test game server unreachable error";

        // Act
        var exception = new GameServerUnreachableException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void GameServerUnreachableException_ShouldAcceptMessageAndInnerException()
    {
        // Arrange
        var message = "Test game server unreachable error";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new GameServerUnreachableException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }
}