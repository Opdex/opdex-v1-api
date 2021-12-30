using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets;

public class RetrieveMarketSummaryByMarketIdQuery : FindQuery<MarketSummary>
{
    public RetrieveMarketSummaryByMarketIdQuery(ulong marketId, bool findOrThrow = true) : base(findOrThrow)
    {
        if (marketId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(marketId), "MarketId must be greater than 0.");
        }

        MarketId = marketId;
    }

    public ulong MarketId { get; }
}
