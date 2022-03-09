using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using Opdex.Platform.WebApi.Exceptions;
using Opdex.Platform.WebApi.Middleware;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Middleware;

public class MaintenanceLockFilterTests
{
    [Fact]
    public void OnActionExecuting_MaintenanceLockFalse_DoNotThrow()
    {
        var maintenanceConfiguration = new MaintenanceConfiguration { Locked = false };
        var maintenanceOptionsMock = new Mock<IOptionsSnapshot<MaintenanceConfiguration>>();
        maintenanceOptionsMock.Setup(callTo => callTo.Value).Returns(maintenanceConfiguration);

        var filter = new MaintenanceLockFilter(maintenanceOptionsMock.Object);

        filter.Invoking(h => h.OnActionExecuting(CreateActionExecutingContext())).Should().NotThrow();
    }

    [Fact]
    public void OnActionExecuting_MaintenanceLockTrue_ThrowMaintenanceLockException()
    {
        var maintenanceConfiguration = new MaintenanceConfiguration { Locked = true };
        var maintenanceOptionsMock = new Mock<IOptionsSnapshot<MaintenanceConfiguration>>();
        maintenanceOptionsMock.Setup(callTo => callTo.Value).Returns(maintenanceConfiguration);

        var filter = new MaintenanceLockFilter(maintenanceOptionsMock.Object);

        filter.Invoking(h => h.OnActionExecuting(CreateActionExecutingContext())).Should().ThrowExactly<MaintenanceLockException>();
    }

    private static ActionExecutingContext CreateActionExecutingContext()
    {
        var actionContext = new ActionContext(
            Mock.Of<HttpContext>(),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            new ModelStateDictionary()
        );

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            Mock.Of<Controller>()
        );
    }
}
