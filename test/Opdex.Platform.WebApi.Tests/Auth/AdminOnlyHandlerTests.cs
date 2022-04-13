using FluentAssertions;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.FeatureManagement;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Auth;
using Opdex.Platform.Application.Abstractions.Models.Auth;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.WebApi.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Auth;

public class AdminOnlyHandlerTests
{
    private readonly AuthConfiguration _authConfiguration;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly HttpContextMock _httpContextMock;

    private readonly AdminOnlyHandler _handler;

    public AdminOnlyHandlerTests()
    {
        var featureManagerMock = new Mock<IFeatureManager>();
        featureManagerMock.Setup(callTo => callTo.IsEnabledAsync("AuthServer")).ReturnsAsync(true);
        _httpContextMock = new HttpContextMock();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(callTo => callTo.HttpContext).Returns(_httpContextMock);
        _authConfiguration = new AuthConfiguration { AdminKey = "L5MMq0~h492*Dg1pxX" };
        _mediatorMock = new Mock<IMediator>();
        _handler = new AdminOnlyHandler(featureManagerMock.Object, httpContextAccessorMock.Object, _authConfiguration, _mediatorMock.Object);
    }

    [Fact]
    public async Task AdminOnlyRequirement_UserNotLoggedIn_DoNotSucceed()
    {
        // Arrange
        var requirements = new[] { new AdminOnlyRequirement() };
        var user = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>()));
        var context = new AuthorizationHandlerContext(requirements, user, null);

        _httpContextMock.SetupRequestHeaders(new Dictionary<string, StringValues>
        {
            { "X-Admin-Key", _authConfiguration.AdminKey }
        });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().Be(false);
    }

    [Fact]
    public async Task AdminOnlyRequirement_UserLoggedIn_CheckIfAdmin()
    {
        // Arrange
        const string address = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";

        var requirements = new[] { new AdminOnlyRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<GetAdminByAddressQuery>(q => q.Address == address), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AdminOnlyRequirement_NotAdmin_DoNotSucceed()
    {
        // Arrange
        const string address = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";

        var requirements = new[] { new AdminOnlyRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, null);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((AdminDto)null);
        _httpContextMock.SetupRequestHeaders(new Dictionary<string, StringValues>
        {
            { "X-Admin-Key", _authConfiguration.AdminKey }
        });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().Be(false);
    }

    [Fact]
    public async Task AdminOnlyRequirement_AdminButNoKeyHeader_DoNotSucceed()
    {
        // Arrange
        const string address = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";

        var requirements = new[] { new AdminOnlyRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, null);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdminDto { Address = address });
        _httpContextMock.SetupRequestHeaders(new Dictionary<string, StringValues>
        {
            { "X-Admin-Key", "invalidAdminKey" }
        });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().Be(false);
    }

    [Fact]
    public async Task AdminOnlyRequirement_AdminButWrongKeyHeader_DoNotSucceed()
    {
        // Arrange
        const string address = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";

        var requirements = new[] { new AdminOnlyRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, null);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdminDto { Address = address });
        _httpContextMock.SetupRequestHeaders(new Dictionary<string, StringValues>
        {
            { "Wrong-Admin-Key-Header", _authConfiguration.AdminKey }
        });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().Be(false);
    }

    [Fact]
    public async Task AdminOnlyRequirement_AdminWithKey_Succeed()
    {
        // Arrange
        const string address = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";

        var requirements = new[] { new AdminOnlyRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, null);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdminDto { Address = address });
        _httpContextMock.SetupRequestHeaders(new Dictionary<string, StringValues>
        {
            { "X-Admin-Key", _authConfiguration.AdminKey }
        });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().Be(true);
    }
}
