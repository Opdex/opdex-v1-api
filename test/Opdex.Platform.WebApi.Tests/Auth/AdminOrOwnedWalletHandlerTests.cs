using FluentAssertions;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Auth;
using Opdex.Platform.Application.Abstractions.Models.Auth;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.WebApi.Auth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Auth;

public class AdminOrOwnedWalletHandlerTests
{
    private readonly AuthConfiguration _authConfiguration;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly HttpContextMock _httpContextMock;

    private readonly AdminOrOwnedWalletHandler _handler;

    public AdminOrOwnedWalletHandlerTests()
    {
        _httpContextMock = new HttpContextMock();
        _authConfiguration = new AuthConfiguration { AdminKey = "L5MMq0~h492*Dg1pxX" };
        _mediatorMock = new Mock<IMediator>();
        _handler = new AdminOrOwnedWalletHandler(_authConfiguration, _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleRequirementAsync_UserNotLoggedIn_DoNotSucceed()
    {
        // Arrange
        var requirements = new[] { new AdminOrOwnedWalletRequirement() };
        var user = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>()));
        var context = new AuthorizationHandlerContext(requirements, user, _httpContextMock);

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
    public async Task HandleRequirementAsync_UserIsTargetWallet_Succeed()
    {
        // Arrange
        const string address = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";

        var requirements = new[] { new AdminOrOwnedWalletRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, _httpContextMock);
        _httpContextMock.SetupUrl("https://api.opdex.com/v1/wallets/tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm/balance");

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().Be(true);
    }

    [Fact]
    public async Task HandleRequirementAsync_DifferentAddressAndNotAdmin_ThrowNotAllowedException()
    {
        // Arrange
        const string address = "tHYHem7cLKgoLkeb792yn4WayqKzLrjJak";

        var requirements = new[] { new AdminOrOwnedWalletRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, _httpContextMock);
        _httpContextMock.SetupUrl("https://api.opdex.com/v1/wallets/tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm/balance");

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((AdminDto)null);
        _httpContextMock.SetupRequestHeaders(new Dictionary<string, StringValues>
        {
            { "X-Admin-Key", _authConfiguration.AdminKey }
        });

        // Act
        Task Act() => _handler.HandleAsync(context);

        // Assert
        await this.Invoking(async _ => await Act()).Should().ThrowExactlyAsync<NotAllowedException>();
    }

    [Fact]
    public async Task HandleRequirementAsync_DifferentAddressAndAdminButNoKeyHeader_DoNotSucceed()
    {
        // Arrange
        const string address = "tHYHem7cLKgoLkeb792yn4WayqKzLrjJak";

        var requirements = new[] { new AdminOrOwnedWalletRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, _httpContextMock);
        _httpContextMock.SetupUrl("https://api.opdex.com/v1/wallets/tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm/balance");

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
    public async Task HandleRequirementAsync_DifferentAddressAndAdminButWrongKeyHeader_DoNotSucceed()
    {
        // Arrange
        const string address = "tHYHem7cLKgoLkeb792yn4WayqKzLrjJak";

        var requirements = new[] { new AdminOrOwnedWalletRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, _httpContextMock);
        _httpContextMock.SetupUrl("https://api.opdex.com/v1/wallets/tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm/balance");

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
    public async Task HandleRequirementAsync_DifferentAddressAndAdminWithKey_Succeed()
    {
        // Arrange
        const string address = "tHYHem7cLKgoLkeb792yn4WayqKzLrjJak";

        var requirements = new[] { new AdminOrOwnedWalletRequirement() };
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, address) };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(requirements, user, _httpContextMock);
        _httpContextMock.SetupUrl("https://api.opdex.com/v1/wallets/tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm/balance");

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
