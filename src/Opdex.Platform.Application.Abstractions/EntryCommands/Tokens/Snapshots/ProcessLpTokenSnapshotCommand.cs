using System;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots
{
    public class ProcessLpTokenSnapshotCommand : IRequest<decimal>
    {
        public ProcessLpTokenSnapshotCommand(ulong marketId, Token lpToken, decimal reservesUsd, SnapshotType snapshotType,
                                             DateTime blockTime, ulong blockHeight)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            if (lpToken == null)
            {
                throw new ArgumentNullException(nameof(lpToken), $"{nameof(lpToken)} must be not be null.");
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType), $"{nameof(snapshotType)} must be a valid type.");
            }

            if (blockTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(blockTime), $"{nameof(blockTime)} must be a valid date time.");
            }

            if (reservesUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reservesUsd), $"{nameof(reservesUsd)} must be greater than 0.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            MarketId = marketId;
            LpToken = lpToken;
            SnapshotType = snapshotType;
            BlockTime = blockTime;
            ReservesUsd = reservesUsd;
            BlockHeight = blockHeight;
        }

        public ulong MarketId { get; }
        public Token LpToken { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime BlockTime { get; }
        public decimal ReservesUsd { get; }
        public ulong BlockHeight { get; }
    }
}
