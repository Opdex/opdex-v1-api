using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;

namespace Opdex.Platform.Application.Handlers;

public class RetrieveCmcStraxPriceQueryHandler : IRequestHandler<RetrieveCmcStraxPriceQuery, decimal>
{
    private readonly IMediator _mediator;
    private readonly ILogger<RetrieveCmcStraxPriceQueryHandler> _logger;

    public RetrieveCmcStraxPriceQueryHandler(IMediator mediator, ILogger<RetrieveCmcStraxPriceQueryHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<decimal> Handle(RetrieveCmcStraxPriceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var isFiveMinutesOrOlder = DateTime.UtcNow.Subtract(request.BlockTime) > TimeSpan.FromMinutes(5);

            return isFiveMinutesOrOlder
                ? await _mediator.Send(new CallCmcGetStraxHistoricalQuoteQuery(request.BlockTime))
                : await _mediator.Send(new CallCmcGetStraxLatestQuoteQuery());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving STRAX price quote");
            return 0m;
        }
    }
}