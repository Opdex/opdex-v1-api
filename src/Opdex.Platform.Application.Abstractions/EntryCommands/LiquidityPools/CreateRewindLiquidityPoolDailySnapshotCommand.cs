using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools
{
    public class CreateRewindLiquidityPoolDailySnapshotCommand : IRequest<bool>
    {
        public CreateRewindLiquidityPoolDailySnapshotCommand(long liquidityPoolId, DateTime startOfDay, DateTime endOfDay)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than zero.");
            }

            if (startOfDay.Equals(default) || endOfDay.Equals(default) || startOfDay >= endOfDay || startOfDay.Date != endOfDay.Date)
            {
                throw new Exception("Start date time must be prior to end date time and the dates must match.");
            }

            LiquidityPoolId = liquidityPoolId;
            StartDate = startOfDay;
            EndDate = endOfDay;
        }

        public long LiquidityPoolId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    }
}
