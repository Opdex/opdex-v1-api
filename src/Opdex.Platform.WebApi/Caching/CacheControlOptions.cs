using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.WebApi.Caching;

public class CacheControlOptions
{
    public CacheControlOptions(CacheType cacheType)
    {
        if (!cacheType.IsValid()) throw new ArgumentOutOfRangeException(nameof(cacheType));
        CacheType = cacheType;
    }

    public CacheType CacheType { get; }
}
