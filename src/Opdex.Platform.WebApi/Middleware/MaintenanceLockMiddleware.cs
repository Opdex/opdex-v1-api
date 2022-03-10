using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Opdex.Platform.WebApi.Exceptions;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Middleware;

public class MaintenanceLockMiddleware
{
    private readonly RequestDelegate _next;

    public MaintenanceLockMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));;
    }

    public async Task Invoke(HttpContext httpContext, IOptionsSnapshot<MaintenanceConfiguration> config)
    {
        if (config.Value.Locked) throw new MaintenanceLockException();

        await _next(httpContext);
    }
}
