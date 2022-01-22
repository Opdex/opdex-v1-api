using Microsoft.AspNetCore.Http;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using System;
using System.Security.Cryptography.X509Certificates;
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
            var certificate = await httpContext.Connection.GetClientCertificateAsync();

            var header = httpContext.Request.Headers["X-ARR-ClientCert"];
            var thumbprint = TryGetThumbprint(header);

            bool isOpdexCertificate = thumbprint.HasValue() || (certificate is not null && certificate.Thumbprint == authConfig.Opdex.CertificateThumbprint);


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
                    httpContext.Response.Headers.Add("INVALID-CERTIFICATE", "true");
                }
            }
        }

        await _next(httpContext);
    }

    private static string TryGetThumbprint(string header)
    {
        if (string.IsNullOrEmpty(header)) return string.Empty;
        try
        {
            byte[] clientCertBytes = Convert.FromBase64String(header);
            var certificate = new X509Certificate2(clientCertBytes);
            return certificate.Thumbprint;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
