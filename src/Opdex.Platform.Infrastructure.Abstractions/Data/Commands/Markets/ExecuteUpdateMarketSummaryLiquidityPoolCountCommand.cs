using MediatR;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;

public class ExecuteUpdateMarketSummaryLiquidityPoolCountCommand : IRequest<bool>
{
    public ExecuteUpdateMarketSummaryLiquidityPoolCountCommand(ulong marketId, ulong blockHeight)
    {
        // a market id of 0 is valid for the procedure, but we never should use it
        if (marketId == 0) throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than 0");
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than 0");
        MarketId = marketId;
        BlockHeight = blockHeight;
    }

    public ulong MarketId { get; }
    public ulong BlockHeight { get; }
}
