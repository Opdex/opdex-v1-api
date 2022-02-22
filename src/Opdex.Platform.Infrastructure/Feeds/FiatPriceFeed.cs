using MediatR;
using Microsoft.FeatureManagement;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinGeckoApi.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Feeds;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Feeds;

public class FiatPriceFeed : IFiatPriceFeed
{
    private readonly IMediator _mediator;
    private readonly IFeatureManager _featureManager;

    public FiatPriceFeed(IMediator mediator, IFeatureManager featureManager)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
    }

    public async Task<decimal> GetCrsUsdPrice(DateTime blockTime, CancellationToken cancellationToken)
    {
        var isFiveMinutesOrOlder = DateTime.UtcNow.Subtract(blockTime) > TimeSpan.FromMinutes(5);
        if (await _featureManager.IsEnabledAsync(FeatureFlags.CoinMarketCapPriceFeed))
        {
            var price = isFiveMinutesOrOlder
                ? await _mediator.Send(new CallCmcGetStraxHistoricalQuoteQuery(blockTime), cancellationToken)
                : await _mediator.Send(new CallCmcGetStraxLatestQuoteQuery(), cancellationToken);
            if (price != 0) return price;
            // falls back to CoinGecko if coin market cap returns 0
        }

        return isFiveMinutesOrOlder
            ? await _mediator.Send(new CallCoinGeckoGetStraxHistoricalPriceQuery(blockTime), cancellationToken)
            : await _mediator.Send(new CallCoinGeckoGetStraxLatestPriceQuery(), cancellationToken);
    }
}
