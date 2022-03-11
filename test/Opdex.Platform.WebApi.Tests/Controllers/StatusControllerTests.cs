using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.WebApi.Controllers;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers;

public class StatusControllerTests
{
    [Fact]
    public void GetStatus_ConfigurationValues_Return()
    {
        // Arrange
        var opdexConfiguration = new OpdexConfiguration
        {
            CommitHash = "d27d08fd714ef82f03c06dd757b2067697f98dd2"
        };

        var maintenanceConfiguration = new MaintenanceConfiguration { Locked = true };
        var maintenanceOptionsMock = new Mock<IOptionsSnapshot<MaintenanceConfiguration>>();
        maintenanceOptionsMock.Setup(callTo => callTo.Value).Returns(maintenanceConfiguration);

        var controller = new StatusController(opdexConfiguration, maintenanceOptionsMock.Object);

        // Act
        var response = controller.GetStatus();

        // Assert
        response.Value!.Commit.Should().Be(opdexConfiguration.CommitHash);
        response.Value!.Identifier.Should().Be(opdexConfiguration.InstanceId);
        response.Value!.UnderMaintenance.Should().Be(maintenanceConfiguration.Locked);
    }
}
