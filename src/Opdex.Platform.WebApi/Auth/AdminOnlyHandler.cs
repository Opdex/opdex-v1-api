using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Auth;

public class AdminOnlyHandler : AuthorizationHandler<AdminOnlyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOnlyRequirement requirement)
    {
        // Return if not found
        if (!context.User.HasClaim(c => c.Type == "admin"))
        {
            return Task.CompletedTask;
        }

        // Validate the admin key provided against the configuration
        if (context.User.FindFirst(c => c.Type == "admin").Value == requirement.Key)
        {
            context.Succeed(requirement);
        }

        // Return if admin key doesn't match
        return Task.CompletedTask;
    }
}