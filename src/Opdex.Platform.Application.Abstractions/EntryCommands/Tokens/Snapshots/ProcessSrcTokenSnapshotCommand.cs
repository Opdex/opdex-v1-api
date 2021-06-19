using System;
using MediatR;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots
{
    public class ProcessSrcTokenSnapshotCommand : IRequest<decimal>
    {
        public ProcessSrcTokenSnapshotCommand(long marketId, Token srcToken, SnapshotType snapshotType, DateTime blockTime,
                                              decimal crsUsd, string reserveCrs, string reserveSrc)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            if (srcToken == null)
            {
                throw new ArgumentNullException(nameof(srcToken), $"{nameof(srcToken)} must be not be null.");
            }

            if (snapshotType == SnapshotType.Unknown)
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType), $"{nameof(snapshotType)} must be a valid type.");
            }

            if (blockTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(blockTime), $"{nameof(blockTime)} must be a valid date time.");
            }

            if (crsUsd <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(crsUsd), $"{nameof(crsUsd)} must be greater than 0.");
            }

            if (!reserveCrs.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(reserveCrs), $"{nameof(reserveCrs)} must be numeric.");
            }

            if (!reserveSrc.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(reserveSrc), $"{nameof(reserveSrc)} must be numeric.");
            }

            MarketId = marketId;
            SrcToken = srcToken;
            SnapshotType = snapshotType;
            BlockTime = blockTime;
            CrsUsd = crsUsd;
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }

        public long MarketId { get; }
        public Token SrcToken { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime BlockTime { get; }
        public decimal CrsUsd { get; }
        public string ReserveCrs { get; }
        public string ReserveSrc { get; }
    }
}