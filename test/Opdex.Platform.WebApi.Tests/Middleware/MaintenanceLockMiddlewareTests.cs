using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Opdex.Platform.WebApi.Middleware;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Middleware;

public class MaintenanceLockMiddlewareTests
{
    [Fact]
    public async Task Invoke_MaintenanceLockFalse_ProcessRequest()
    {
        // Arrange
        using var host = await CreateHost(maintenanceLock: false);

        // Act
        var response = await host.GetTestClient().GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Invoke_MaintenanceLockTrue_Return503ServiceUnavailable()
    {
        // Arrange
        using var host = await CreateHost(maintenanceLock: true);

        // Act
        var response = await host.GetTestClient().GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    private static async Task<IHost> CreateHost(bool maintenanceLock)
    {
        var optionsSnapshotMock = new Mock<IOptionsSnapshot<MaintenanceConfiguration>>();
        optionsSnapshotMock.Setup(callTo => callTo.Value)
            .Returns(new MaintenanceConfiguration {Locked = maintenanceLock});

        return await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        // registering configuration options like normal doesn't work, so mocking instead
                        services.AddScoped(_ => optionsSnapshotMock.Object);
                        services.AddRouting();
                    })
                    .Configure(app =>
                    {
                        app.UseMiddleware<MaintenanceLockMiddleware>();
                        app.UseRouting();
                        app.UseEndpoints(builder =>
                        {
                            builder.MapGet("/", async context =>
                            {
                                context.Response.StatusCode = StatusCodes.Status204NoContent;
                                await context.Response.CompleteAsync();
                            });
                        });

                    });
            })
            .StartAsync();
    }
}
