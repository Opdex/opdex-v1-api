using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Opdex.Platform.WebApi.Exceptions;
using System;

namespace Opdex.Platform.WebApi.Middleware;

public class MaintenanceLockFilter : IActionFilter
{
    private readonly IOptionsSnapshot<MaintenanceConfiguration> _config;

    public MaintenanceLockFilter(IOptionsSnapshot<MaintenanceConfiguration> config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (_config.Value.Locked) throw new MaintenanceLockException();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
