using System;
using MediatR;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Pools
{
    public class ProcessDailyLiquidityPoolSnapshotRefreshCommand : IRequest<Unit>
    {
        public ProcessDailyLiquidityPoolSnapshotRefreshCommand(long liquidityPoolId, long marketId, Token srcToken, Token lpToken, decimal crsUsd,
                                                               SnapshotType snapshotType, DateTime blockTime, decimal? stakingTokenUsd = null)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }

            if (srcToken == null)
            {
                throw new ArgumentNullException(nameof(srcToken), $"{nameof(srcToken)} must be provided.");
            }

            if (lpToken == null)
            {
                throw new ArgumentNullException(nameof(lpToken), $"{nameof(lpToken)} must be provided.");
            }

            if (crsUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(crsUsd));
            }

            if (snapshotType == SnapshotType.Unknown)
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType), $"{nameof(snapshotType)} must be valid.");
            }

            if (blockTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(blockTime), $"{nameof(blockTime)} must be a valid date time.");
            }

            LiquidityPoolId = liquidityPoolId;
            MarketId = marketId;
            SrcToken = srcToken;
            LpToken = lpToken;
            CrsUsd = crsUsd;
            SnapshotType = snapshotType;
            BlockTime = blockTime;
            StakingTokenUsd = stakingTokenUsd;
        }

        public long LiquidityPoolId { get; }
        public long MarketId { get; }
        public Token SrcToken { get; }
        public Token LpToken { get; }
        public decimal CrsUsd { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime BlockTime { get; }
        public decimal? StakingTokenUsd { get; }
    }
}