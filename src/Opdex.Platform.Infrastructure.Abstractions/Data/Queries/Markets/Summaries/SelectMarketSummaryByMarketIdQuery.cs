using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Summaries;

public class SelectMarketSummaryByMarketIdQuery : FindQuery<MarketSummary>
{
    public SelectMarketSummaryByMarketIdQuery(ulong marketId, bool findOrThrow = true) : base(findOrThrow)
    {
        if (marketId < 1)
        {
            throw new ArgumentNullException(nameof(marketId), "MarketId must be greater than 0.");
        }

        MarketId = marketId;
    }

    public ulong MarketId { get; }
}
