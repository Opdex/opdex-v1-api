using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Common.Configurations;

public class EncryptionConfiguration : IValidatable
{
    public string Key { get; set; }

    public void Validate()
    {
        if (!Key.HasValue() || Key.Length % 16 != 0) throw new Exception("Encryption key must have length equal to multiple of 16.");
    }
}