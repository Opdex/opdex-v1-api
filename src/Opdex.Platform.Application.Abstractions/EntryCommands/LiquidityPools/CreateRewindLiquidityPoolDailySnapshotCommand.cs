using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools
{
    public class CreateRewindLiquidityPoolDailySnapshotCommand : IRequest<bool>
    {
        public CreateRewindLiquidityPoolDailySnapshotCommand(long liquidityPoolId, long srcTokenId, decimal crsUsdStartOfDay,
                                                             decimal stakingTokenUsdStartOfDay, DateTime startOfDay, DateTime endOfDay)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than zero.");
            }

            if (srcTokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(srcTokenId), "SRC token id must be greater than zero.");
            }

            if (crsUsdStartOfDay <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(crsUsdStartOfDay), "CRS USD at the start of the day must be greater than 0.");
            }

            if (stakingTokenUsdStartOfDay < 0m) // 0 is valid, negative is not
            {
                throw new ArgumentOutOfRangeException(nameof(stakingTokenUsdStartOfDay), "Staking token USD at the start of the day must be at least 0.");
            }

            if (startOfDay.Equals(default) || endOfDay.Equals(default) || startOfDay >= endOfDay || startOfDay.Date != endOfDay.Date)
            {
                throw new Exception("Start date time must be prior to end date time and the dates must match.");
            }

            LiquidityPoolId = liquidityPoolId;
            SrcTokenId = srcTokenId;
            CrsUsdStartOfDay = crsUsdStartOfDay;
            StakingTokenUsdStartOfDay = stakingTokenUsdStartOfDay;
            StartDate = startOfDay;
            EndDate = endOfDay;
        }

        public long LiquidityPoolId { get; }
        public long SrcTokenId { get; }
        public decimal CrsUsdStartOfDay { get; }
        public decimal StakingTokenUsdStartOfDay { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    }
}
