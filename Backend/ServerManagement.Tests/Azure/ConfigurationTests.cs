using ServerManagement.Azure.Configuration;

namespace ServerManagement.Tests.Azure;

public class ConfigurationTests
{
    [Fact]
    public void AzureVmConfiguration_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var config = new AzureVmConfiguration();

        // Assert
        Assert.Equal("", config.SubscriptionId);
        Assert.Equal("", config.ResourceGroupName);
        Assert.Equal("", config.VmName);
    }

    [Fact]
    public void AzureVmConfiguration_ShouldAcceptAllProperties()
    {
        // Arrange
        var subscriptionId = "test-subscription";
        var resourceGroupName = "test-rg";
        var vmName = "test-vm";

        // Act
        var config = new AzureVmConfiguration
        {
            SubscriptionId = subscriptionId,
            ResourceGroupName = resourceGroupName,
            VmName = vmName
        };

        // Assert
        Assert.Equal(subscriptionId, config.SubscriptionId);
        Assert.Equal(resourceGroupName, config.ResourceGroupName);
        Assert.Equal(vmName, config.VmName);
    }

    [Fact]
    public void GameServerConfiguration_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var config = new GameServerConfiguration();

        // Assert
        Assert.Equal("", config.Host);
        Assert.Equal(26900, config.Port);
        Assert.Equal(8081, config.TelnetPort);
        Assert.Equal("", config.AdminPassword);
    }

    [Fact]
    public void GameServerConfiguration_ShouldAcceptAllProperties()
    {
        // Arrange
        var host = "test-host";
        var port = 12345;
        var telnetPort = 9999;
        var adminPassword = "test-password";

        // Act
        var config = new GameServerConfiguration
        {
            Host = host,
            Port = port,
            TelnetPort = telnetPort,
            AdminPassword = adminPassword
        };

        // Assert
        Assert.Equal(host, config.Host);
        Assert.Equal(port, config.Port);
        Assert.Equal(telnetPort, config.TelnetPort);
        Assert.Equal(adminPassword, config.AdminPassword);
    }
}