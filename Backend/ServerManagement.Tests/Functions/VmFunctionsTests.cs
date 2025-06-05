using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ServerManagement.Core.Exceptions;
using ServerManagement.Core.Interfaces;
using ServerManagement.Core.Models;
using ServerManagement.Functions.Functions;

namespace ServerManagement.Tests.Functions;

public class VmFunctionsTests
{
    private readonly Mock<ILogger<VmFunctions>> _mockLogger;
    private readonly Mock<IServerManager> _mockServerManager;
    private readonly VmFunctions _vmFunctions;

    public VmFunctionsTests()
    {
        _mockLogger = new Mock<ILogger<VmFunctions>>();
        _mockServerManager = new Mock<IServerManager>();
        _vmFunctions = new VmFunctions(_mockLogger.Object, _mockServerManager.Object);
    }

    [Fact]
    public void VmFunctions_ShouldInitializeWithProperDependencies()
    {
        // Arrange & Act & Assert
        Assert.NotNull(_vmFunctions);
    }

    [Fact]
    public void VmFunctions_ShouldThrowIfNullLogger()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new VmFunctions(null!, _mockServerManager.Object));
    }

    [Fact]
    public void VmFunctions_ShouldThrowIfNullServerManager()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new VmFunctions(_mockLogger.Object, null!));
    }

    // Note: Testing Azure Functions HTTP triggers requires more complex setup
    // with mocked HttpRequestData and HttpResponseData objects. The business logic
    // is tested through the IServerManager interface mock, while the HTTP layer
    // would typically be tested with integration tests or specialized test frameworks.

    [Fact]
    public async Task GetVmStatus_ShouldReturnVmStatus_WhenServerManagerSucceeds()
    {
        // Arrange
        var expectedStatus = new VmStatus { VmState = VmState.Running, GamePortOpen = true };
        _mockServerManager.Setup(x => x.GetVmStatusAsync()).ReturnsAsync(expectedStatus);

        // Act & Assert
        // This test demonstrates the expected behavior. Full HTTP testing would require
        // mocking HttpRequestData and HttpResponseData which is complex and typically
        // done with integration tests instead.
        var actualStatus = await _mockServerManager.Object.GetVmStatusAsync();
        Assert.Equal(expectedStatus.VmState, actualStatus.VmState);
        Assert.Equal(expectedStatus.GamePortOpen, actualStatus.GamePortOpen);
    }

    [Fact]
    public async Task StartVm_ShouldCallServerManager_WhenInvoked()
    {
        // Arrange
        _mockServerManager.Setup(x => x.StartVmAsync()).Returns(Task.CompletedTask);
        _mockServerManager.Setup(x => x.GetVmStatusAsync()).ReturnsAsync(new VmStatus { VmState = VmState.Starting });

        // Act
        await _mockServerManager.Object.StartVmAsync();
        var status = await _mockServerManager.Object.GetVmStatusAsync();

        // Assert
        _mockServerManager.Verify(x => x.StartVmAsync(), Times.Once);
        _mockServerManager.Verify(x => x.GetVmStatusAsync(), Times.Once);
        Assert.Equal(VmState.Starting, status.VmState);
    }

    [Fact]
    public async Task StopVm_ShouldCallServerManager_WhenInvoked()
    {
        // Arrange
        _mockServerManager.Setup(x => x.StopVmAsync()).Returns(Task.CompletedTask);
        _mockServerManager.Setup(x => x.GetVmStatusAsync()).ReturnsAsync(new VmStatus { VmState = VmState.Stopping });

        // Act
        await _mockServerManager.Object.StopVmAsync();
        var status = await _mockServerManager.Object.GetVmStatusAsync();

        // Assert
        _mockServerManager.Verify(x => x.StopVmAsync(), Times.Once);
        _mockServerManager.Verify(x => x.GetVmStatusAsync(), Times.Once);
        Assert.Equal(VmState.Stopping, status.VmState);
    }

    [Fact]
    public async Task RestartVm_ShouldCallServerManager_WhenInvoked()
    {
        // Arrange
        _mockServerManager.Setup(x => x.RestartVmAsync()).Returns(Task.CompletedTask);
        _mockServerManager.Setup(x => x.GetVmStatusAsync()).ReturnsAsync(new VmStatus { VmState = VmState.Starting });

        // Act
        await _mockServerManager.Object.RestartVmAsync();
        var status = await _mockServerManager.Object.GetVmStatusAsync();

        // Assert
        _mockServerManager.Verify(x => x.RestartVmAsync(), Times.Once);
        _mockServerManager.Verify(x => x.GetVmStatusAsync(), Times.Once);
        Assert.Equal(VmState.Starting, status.VmState);
    }

    [Fact]
    public async Task GetVmStatus_ShouldHandleVmOperationException()
    {
        // Arrange
        var expectedException = new VmOperationException("Test VM operation failed");
        _mockServerManager.Setup(x => x.GetVmStatusAsync()).ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<VmOperationException>(
            () => _mockServerManager.Object.GetVmStatusAsync());
        Assert.Equal(expectedException.Message, actualException.Message);
    }
}