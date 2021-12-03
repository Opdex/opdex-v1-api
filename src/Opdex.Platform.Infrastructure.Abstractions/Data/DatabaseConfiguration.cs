using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data;

public class DatabaseConfiguration : IValidatable
{
    public string ConnectionString { get; set; }

    public void Validate()
    {
        if (!ConnectionString.HasValue())
        {
            throw new Exception($"{nameof(DatabaseConfiguration)}.{nameof(ConnectionString)} must not be null or empty.");
        }
    }
}