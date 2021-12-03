using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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

        var controller = new StatusController(opdexConfiguration);

        // Act
        var response = controller.GetStatus();

        // Assert
        response.Value.Commit.Should().Be(opdexConfiguration.CommitHash);
        response.Value.Identifier.Should().Be(opdexConfiguration.InstanceId);
    }
}