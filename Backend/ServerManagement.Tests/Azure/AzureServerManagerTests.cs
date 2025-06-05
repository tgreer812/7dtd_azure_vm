using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ServerManagement.Azure;
using ServerManagement.Azure.Configuration;
using ServerManagement.Core.Exceptions;
using ServerManagement.Core.Models;

namespace ServerManagement.Tests.Azure;

public class AzureServerManagerTests
{
    private readonly Mock<ILogger<AzureServerManager>> _mockLogger;
    private readonly Mock<IOptions<AzureVmConfiguration>> _mockVmOptions;
    private readonly Mock<IOptions<GameServerConfiguration>> _mockGameOptions;
    private readonly Mock<HttpClient> _mockHttpClient;
    private readonly AzureVmConfiguration _vmConfig;
    private readonly GameServerConfiguration _gameConfig;

    public AzureServerManagerTests()
    {
        _mockLogger = new Mock<ILogger<AzureServerManager>>();
        _mockVmOptions = new Mock<IOptions<AzureVmConfiguration>>();
        _mockGameOptions = new Mock<IOptions<GameServerConfiguration>>();
        _mockHttpClient = new Mock<HttpClient>();

        _vmConfig = new AzureVmConfiguration
        {
            SubscriptionId = "test-subscription-id",
            ResourceGroupName = "test-rg",
            VmName = "test-vm"
        };

        _gameConfig = new GameServerConfiguration
        {
            Host = "test-host",
            Port = 26900,
            TelnetPort = 8081,
            AdminPassword = "test-password"
        };

        _mockVmOptions.Setup(x => x.Value).Returns(_vmConfig);
        _mockGameOptions.Setup(x => x.Value).Returns(_gameConfig);
    }

    [Fact]
    public void AzureServerManager_ShouldInitializeWithProperDependencies()
    {
        // Arrange & Act
        var httpClient = new HttpClient();
        var serverManager = new AzureServerManager(
            _mockLogger.Object,
            _mockVmOptions.Object,
            _mockGameOptions.Object,
            httpClient);

        // Assert
        Assert.NotNull(serverManager);
    }

    [Fact]
    public void AzureServerManager_ShouldThrowIfNullLogger()
    {
        // Arrange & Act & Assert
        var httpClient = new HttpClient();
        Assert.Throws<ArgumentNullException>(() => new AzureServerManager(
            null!,
            _mockVmOptions.Object,
            _mockGameOptions.Object,
            httpClient));
    }

    [Fact]
    public void AzureServerManager_ShouldThrowIfNullVmOptions()
    {
        // Arrange & Act & Assert
        var httpClient = new HttpClient();
        Assert.Throws<ArgumentNullException>(() => new AzureServerManager(
            _mockLogger.Object,
            null!,
            _mockGameOptions.Object,
            httpClient));
    }

    [Fact]
    public void AzureServerManager_ShouldThrowIfNullGameOptions()
    {
        // Arrange & Act & Assert
        var httpClient = new HttpClient();
        Assert.Throws<ArgumentNullException>(() => new AzureServerManager(
            _mockLogger.Object,
            _mockVmOptions.Object,
            null!,
            httpClient));
    }

    [Fact]
    public void AzureServerManager_ShouldThrowIfNullHttpClient()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AzureServerManager(
            _mockLogger.Object,
            _mockVmOptions.Object,
            _mockGameOptions.Object,
            null!));
    }

    // Note: Full integration testing of Azure SDK operations would require 
    // more complex mocking or actual Azure resources. These tests focus on 
    // the basic initialization and parameter validation.
    // Additional tests would mock the Azure SDK components to test the
    // business logic without requiring actual Azure resources.
}