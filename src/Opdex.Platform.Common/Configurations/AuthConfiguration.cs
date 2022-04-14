using System;

namespace Opdex.Platform.Common.Configurations;

public class AuthConfiguration : IValidatable
{
    public AuthProvider Opdex { get; set; }

    public string AdminKey { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(Opdex?.SigningKey)) throw new Exception("Signing key must be configured");
        if (string.IsNullOrEmpty(AdminKey)) throw new Exception("Admin key must be configured");
    }
}

public class AuthProvider
{
    public string SigningKey { get; set; }
}
