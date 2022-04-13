using Microsoft.AspNetCore.Authorization;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.WebApi.Auth;

public class AdminOnlyRequirement : IAuthorizationRequirement
{
    public const string Name = "AdminOnly";

    public AdminOnlyRequirement(string key = "true") // For now setting the default requirement to have a claim == "true".
    {
        if (!key.HasValue())
        {
            throw new ArgumentNullException(nameof(key), "Admin key must have a value set to be included as a requirement.");
        }

        Key = key;
    }

    public string Key { get; }
}
