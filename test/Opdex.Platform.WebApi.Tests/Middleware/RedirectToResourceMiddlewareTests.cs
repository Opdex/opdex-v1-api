using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Opdex.Platform.Application.Exceptions;
using Opdex.Platform.WebApi.Middleware;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Middleware;

public class RedirectToResourceMiddlewareTests
{
    [Fact]
    public async Task Invoke_RegularException_Next()
    {
        // Arrange
        using var host = await CreateHost();

        // Act
        HttpResponseMessage response = null;
        try
        {
            response = await host.GetTestClient().GetAsync("/generic-exception");
        }
        catch (Exception) { }

        // Assert
        response?.StatusCode.Should().NotBe(HttpStatusCode.SeeOther);
    }

    [Fact]
    public async Task Invoke_TokenAlreadyIndexedException_Intercept()
    {
        // Arrange
        using var host = await CreateHost();

        // Act
        var response = await host.GetTestClient().GetAsync("/token-already-indexed-exception");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.SeeOther);
    }

    private async Task<IHost> CreateHost()
    {
        return await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                    })
                    .Configure(app =>
                    {
                        app.UseMiddleware<RedirectToResourceMiddleware>();
                        app.UseRouting();
                        app.UseEndpoints(builder =>
                        {
                            builder.MapGet("/token-already-indexed-exception", context =>
                            {
                                throw new TokenAlreadyIndexedException("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
                            });
                            builder.MapGet("/generic-exception", context =>
                            {
                                throw new Exception("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
                            });
                        });

                    });
            })
            .StartAsync();
    }
}