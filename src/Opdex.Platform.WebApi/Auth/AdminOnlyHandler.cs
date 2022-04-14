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
    private readonly AuthConfiguration _authConfig;
    private readonly IMediator _mediator;

    public AdminOnlyHandler(AuthConfiguration authConfig, IMediator mediator)
    {
        _authConfig = authConfig ?? throw new ArgumentNullException(nameof(authConfig));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOnlyRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub)) return;
        var wallet = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var httpContext = context.Resource as HttpContext ??
                          throw new InvalidOperationException("Can only handle admin policy for HTTP requests");

        var admin = await _mediator.Send(new GetAdminByAddressQuery(wallet, findOrThrow: false));
        if (admin is null) return;

        if (!httpContext.Request.Headers.TryGetValue("X-Admin-Key", out var key)) return;
        if (key != _authConfig.AdminKey) return;

        context.Succeed(requirement);
    }
}
