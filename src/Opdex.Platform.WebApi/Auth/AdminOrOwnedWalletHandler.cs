using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.JsonWebTokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Auth;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Exceptions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Auth;

public class AdminOrOwnedWalletHandler : AuthorizationHandler<AdminOrOwnedWalletRequirement>
{
    private readonly AuthConfiguration _authConfig;
    private readonly IMediator _mediator;

    public AdminOrOwnedWalletHandler(AuthConfiguration authConfig, IMediator mediator)
    {
        _authConfig = authConfig ?? throw new ArgumentNullException(nameof(authConfig));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrOwnedWalletRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub)) return;
        var wallet = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var httpContext = context.Resource as HttpContext ??
                          throw new InvalidOperationException("Can only handle admin or owned wallet policy for HTTP requests");

        if (!httpContext.Request.Path.StartsWithSegments("/v1/wallets", out var pathRemainder) || !pathRemainder.HasValue)
            throw new InvalidOperationException("Can only handle admin or owned wallet policy for wallet requests");

        var trimmedPath = pathRemainder.Value![1..];
        var targetWallet = trimmedPath[..trimmedPath.IndexOf('/')];

        if (wallet == targetWallet)
        {
            context.Succeed(requirement);
            return;
        }

        var admin = await _mediator.Send(new GetAdminByAddressQuery(wallet, findOrThrow: false));
        if (admin is null) throw new NotAllowedException("Cannot refresh position of another address");

        if (!httpContext.Request.Headers.TryGetValue("X-Admin-Key", out var key)) return;
        if (key != _authConfig.AdminKey) return;

        context.Succeed(requirement);
    }
}
