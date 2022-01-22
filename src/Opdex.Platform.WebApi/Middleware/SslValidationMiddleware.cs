using Microsoft.AspNetCore.Http;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Middleware;

public class SslValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SslValidationMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext httpContext, AuthConfiguration authConfig)
    {
        if (authConfig.Opdex.CertificateThumbprint.HasValue())
        {
            // Check if the client connection certificate thumbprint matches ours
            bool isOpdexCertificate = httpContext.Connection.ClientCertificate?.Thumbprint == authConfig.Opdex.CertificateThumbprint;

            // If the request is using the whitelisted certificate, attach the designated API key
            if (isOpdexCertificate)
            {
                httpContext.Request.Headers.Add("OPDEX-API-KEY", authConfig.Opdex.ApiKey);
            }
            // Temporary for testing -- Should not return forbidden for any production network
            else
            {
                var path = httpContext.Request.Path.Value ?? string.Empty;

                if (!path.HasValue() || !path.Contains("status"))
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }
        }

        await _next(httpContext);
    }
}
