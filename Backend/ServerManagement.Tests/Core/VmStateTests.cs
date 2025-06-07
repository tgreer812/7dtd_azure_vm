using ServerManagement.Core.Models;

namespace ServerManagement.Tests.Core;

public class VmStateTests
{
    [Fact]
    public void VmState_ShouldHaveAllRequiredValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            VmState.Deallocated,
            VmState.Deallocating,
            VmState.Starting,
            VmState.Running,
            VmState.Stopping,
            VmState.Stopped
        };

        // Act
        var actualValues = Enum.GetValues<VmState>();

        // Assert
        Assert.Equal(expectedValues.Length, actualValues.Length);
        foreach (var expected in expectedValues)
        {
            Assert.Contains(expected, actualValues);
        }
    }

    [Theory]
    [InlineData(VmState.Deallocated)]
    [InlineData(VmState.Deallocating)]
    [InlineData(VmState.Starting)]
    [InlineData(VmState.Running)]
    [InlineData(VmState.Stopping)]
    [InlineData(VmState.Stopped)]
    public void VmState_ShouldBeValidEnum(VmState vmState)
    {
        // Arrange & Act & Assert
        Assert.True(Enum.IsDefined(typeof(VmState), vmState));
    }
}