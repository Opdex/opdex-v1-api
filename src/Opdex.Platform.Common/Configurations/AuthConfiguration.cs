using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Common.Configurations;

public class AuthConfiguration : IValidatable
{
    public AuthProvider Opdex { get; set; }
    public string AdminKey { get; set; }
    public StratisSignatureAuthConfiguration StratisSignatureAuth { get; set; }

    public void Validate()
    {
        if (!AdminKey.HasValue())
        {
            throw new Exception($"{nameof(AuthConfiguration)}.{nameof(AdminKey)} must not be null or empty.");
        }
    }
}

public class AuthProvider
{
    public string SigningKey { get; set; }
}

public class StratisSignatureAuthConfiguration
{
    public string CallbackPath { get; set; }
}