using Microsoft.AspNetCore.Authorization;

namespace Opdex.Platform.WebApi.Auth;

public class AdminOnlyRequirement : IAuthorizationRequirement
{
}
