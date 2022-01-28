using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Opdex.Platform.WebApi.Caching;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CacheUntilNextBlockAttribute : Attribute, IFilterFactory
{
    private readonly CacheType _cacheType;

    public CacheUntilNextBlockAttribute(CacheType cacheType)
    {
        _cacheType = cacheType;
    }

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

        var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return new BlockBasedCacheControlFilter(new CacheControlOptions(_cacheType), mediator);
    }
}
