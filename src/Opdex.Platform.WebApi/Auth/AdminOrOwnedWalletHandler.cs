using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Opdex.Platform.Application.Abstractions.EntryQueries.Auth;
using Opdex.Platform.Common.Exceptions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Auth;

public class AdminOrOwnedWalletHandler : AuthorizationHandler<AdminOrOwnedWalletRequirement>
{
    private readonly IFeatureManager _featureManager;
    private readonly IMediator _mediator;

    private const string AdminClaim = "admin";

    public AdminOrOwnedWalletHandler(IFeatureManager featureManager, IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrOwnedWalletRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == "wallet")) return;
        var wallet = context.User.FindFirstValue("wallet");

        var httpContext = context.Resource as HttpContext ??
                          throw new InvalidOperationException("Can only handle admin or owned wallet policy for HTTP requests");

        if (!httpContext.Request.Path.StartsWithSegments("/v1/wallets", out var pathRemainder) || !pathRemainder.HasValue)
            throw new InvalidOperationException("Can only handle admin or owned wallet policy for wallet requests");

        var trimmedPath = pathRemainder.Value![1..];
        var targetWallet = trimmedPath[..trimmedPath.IndexOf('/')];
        if (wallet == targetWallet) context.Succeed(requirement);

        if (await _featureManager.IsEnabledAsync("AuthServer"))
        {
            var admin = await _mediator.Send(new GetAdminByAddressQuery(wallet, findOrThrow: false));
            if (admin is null) throw new NotAllowedException("Cannot refresh position of another address");

            context.Succeed(requirement);
        }
        else
        {
            // Return if not found
            if (!context.User.HasClaim(c => c.Type == AdminClaim)
                || !(context.User.FindFirst(c => c.Type == AdminClaim)?.Value.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase) ?? true))
                throw new NotAllowedException("Cannot refresh position of another address");

            context.Succeed(requirement);
        }
    }
}
