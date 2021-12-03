using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.WebApi.Auth;

public class AuthConfiguration : IValidatable
{
    public AuthProvider Opdex { get; set; }
    public string AdminKey { get; set; }
    public StratisOpenAuthConfiguration StratisOpenAuthProtocol { get; set; }

    public class AuthProvider
    {
        public string SigningKey { get; set; }
    }

    public class StratisOpenAuthConfiguration
    {
        public string CallbackPath { get; set; }
    }

    public void Validate()
    {
        if (!AdminKey.HasValue())
        {
            throw new Exception($"{nameof(AuthConfiguration)}.{nameof(AdminKey)} must not be null or empty.");
        }
    }
}