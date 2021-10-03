using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools
{
    public class ProcessDailyLiquidityPoolSnapshotRefreshCommand : IRequest<Unit>
    {
        public ProcessDailyLiquidityPoolSnapshotRefreshCommand(ulong liquidityPoolId, ulong marketId, Token srcToken, Token lpToken, decimal crsUsd,
                                                               SnapshotType snapshotType, DateTime blockTime, ulong blockHeight, decimal? stakingTokenUsd = null)
        {
            if (liquidityPoolId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            if (marketId == 0)
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

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType), $"{nameof(snapshotType)} must be valid.");
            }

            if (blockTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(blockTime), $"{nameof(blockTime)} must be a valid date time.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            LiquidityPoolId = liquidityPoolId;
            MarketId = marketId;
            SrcToken = srcToken;
            LpToken = lpToken;
            CrsUsd = crsUsd;
            SnapshotType = snapshotType;
            BlockTime = blockTime;
            BlockHeight = blockHeight;
            StakingTokenUsd = stakingTokenUsd;
        }

        public ulong LiquidityPoolId { get; }
        public ulong MarketId { get; }
        public Token SrcToken { get; }
        public Token LpToken { get; }
        public decimal CrsUsd { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime BlockTime { get; }
        public ulong BlockHeight { get; }
        public decimal? StakingTokenUsd { get; }
    }
}
