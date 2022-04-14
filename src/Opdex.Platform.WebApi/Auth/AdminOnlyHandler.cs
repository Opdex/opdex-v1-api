using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;
using Opdex.Platform.Application.Abstractions.EntryQueries.Auth;
using Opdex.Platform.Common.Configurations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Auth;

public class AdminOnlyHandler : AuthorizationHandler<AdminOnlyRequirement>
{
    private readonly IFeatureManager _featureManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthConfiguration _authConfig;
    private readonly IMediator _mediator;

    private const string AdminClaim = "admin";

    public AdminOnlyHandler(IFeatureManager featureManager, IHttpContextAccessor httpContextAccessor,
                            AuthConfiguration authConfig, IMediator mediator)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _authConfig = authConfig ?? throw new ArgumentNullException(nameof(authConfig));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOnlyRequirement requirement)
    {
        if (await _featureManager.IsEnabledAsync("AuthServer"))
        {
            if (!context.User.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub)) return;

            var wallet = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var admin = await _mediator.Send(new GetAdminByAddressQuery(wallet, findOrThrow: false));
            if (admin is null) return;

            if (!_httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("X-Admin-Key", out var key)) return;
            if (key != _authConfig.AdminKey) return;

            context.Succeed(requirement);
        }
        else
        {
            // Return if not found
            if (!context.User.HasClaim(c => c.Type == AdminClaim)) return;

            // Validate the admin key provided against the configuration
            if (context.User.FindFirst(c => c.Type == AdminClaim)!.Value.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Succeed(requirement);
            }
        }
    }
}
