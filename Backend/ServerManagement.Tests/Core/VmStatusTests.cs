using ServerManagement.Core.Models;

namespace ServerManagement.Tests.Core;

public class VmStatusTests
{
    [Fact]
    public void VmStatus_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var vmStatus = new VmStatus();

        // Assert
        Assert.Equal(VmState.Deallocated, vmStatus.VmState);
        Assert.Null(vmStatus.GamePortOpen);
    }

    [Fact]
    public void VmStatus_ShouldAcceptAllVmStates()
    {
        // Arrange & Act & Assert
        foreach (VmState state in Enum.GetValues<VmState>())
        {
            var vmStatus = new VmStatus { VmState = state };
            Assert.Equal(state, vmStatus.VmState);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void VmStatus_ShouldAcceptAllGamePortOpenValues(bool? gamePortOpen)
    {
        // Arrange & Act
        var vmStatus = new VmStatus { GamePortOpen = gamePortOpen };

        // Assert
        Assert.Equal(gamePortOpen, vmStatus.GamePortOpen);
    }
}