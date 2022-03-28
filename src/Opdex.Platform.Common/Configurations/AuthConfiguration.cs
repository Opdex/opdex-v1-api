using System;

namespace Opdex.Platform.Common.Configurations;

public class AuthConfiguration : IValidatable
{
    public AuthProvider Opdex { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(Opdex?.SigningKey)) throw new Exception("Signing key must be configured");
    }
}

public class AuthProvider
{
    public string SigningKey { get; set; }
}
