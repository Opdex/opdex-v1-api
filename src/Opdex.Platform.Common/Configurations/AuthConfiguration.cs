using System;

namespace Opdex.Platform.Common.Configurations;

public class AuthConfiguration : IValidatable
{
    public string AdminKey { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(AdminKey)) throw new Exception("Admin key must be configured");
    }
}
