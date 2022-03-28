using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;
using Opdex.Platform.Application.Abstractions.EntryQueries.Auth;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Auth;

public class AdminOnlyHandler : AuthorizationHandler<AdminOnlyRequirement>
{
    private readonly IFeatureManager _featureManager;
    private readonly IMediator _mediator;

    private const string AdminClaim = "admin";

    public AdminOnlyHandler(IFeatureManager featureManager, IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOnlyRequirement requirement)
    {
        if (await _featureManager.IsEnabledAsync("AuthServer"))
        {
            if (!context.User.HasClaim(c => c.Type == "wallet")) return;

            var wallet = context.User.FindFirstValue("wallet");
            var admin = await _mediator.Send(new GetAdminByAddressQuery(wallet, findOrThrow: false));
            if (admin is null) return;

            context.Succeed(requirement);
        }
        else
        {
            // Return if not found
            if (!context.User.HasClaim(c => c.Type == AdminClaim)) return;

            // Validate the admin key provided against the configuration
            if (context.User.FindFirst(c => c.Type == AdminClaim)?.Value == requirement.Key)
            {
                context.Succeed(requirement);
            }
        }
    }
}
