using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Opdex.Platform.Application.Abstractions.EntryQueries.Indexer;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Caching;

internal class BlockBasedCacheControlFilter : IAsyncActionFilter
{
    private const int BlockIntervalSeconds = 16;
    private readonly CacheControlOptions _options;
    private readonly IMediator _mediator;

    public BlockBasedCacheControlFilter(CacheControlOptions options, IMediator mediator)
    {
        _options = options;
        _mediator = mediator;
    }

    private string CacheTypeDirective => _options.CacheType == CacheType.Public ? "public" : "private";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var headers = context.HttpContext.Response.Headers;

        // clear headers
        headers.Remove(HeaderNames.CacheControl);
        headers.Remove(HeaderNames.Pragma);

        var status = await _mediator.Send(new GetIndexerStatusQuery(), CancellationToken.None);
        if (status.Locked)
        {
            // do not cache responses when locked
            await next();
            // response data may not be valid, potentially we could even return 503
            return;
        }

        var latestBlock = await _mediator.Send(new RetrieveLatestBlockQuery(), CancellationToken.None);
        var secondsSinceLastBlock = (DateTime.UtcNow - latestBlock.Time).TotalSeconds;
        var secondsExpectedToNextBlock = BlockIntervalSeconds - ((int)secondsSinceLastBlock % BlockIntervalSeconds);

        var cacheControlValue = $"{CacheTypeDirective}, max-age={secondsExpectedToNextBlock}";
        headers.CacheControl = cacheControlValue;

        await next();
    }

}
