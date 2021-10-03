using System;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots
{
    public class ProcessSrcTokenSnapshotCommand : IRequest<decimal>
    {
        public ProcessSrcTokenSnapshotCommand(ulong marketId, Token srcToken, SnapshotType snapshotType, DateTime blockTime,
                                              decimal crsUsd, ulong reserveCrs, UInt256 reserveSrc)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            if (srcToken == null)
            {
                throw new ArgumentNullException(nameof(srcToken), $"{nameof(srcToken)} must be not be null.");
            }

            if (!snapshotType.IsValid())
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

            MarketId = marketId;
            SrcToken = srcToken;
            SnapshotType = snapshotType;
            BlockTime = blockTime;
            CrsUsd = crsUsd;
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }

        public ulong MarketId { get; }
        public Token SrcToken { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime BlockTime { get; }
        public decimal CrsUsd { get; }
        public ulong ReserveCrs { get; }
        public UInt256 ReserveSrc { get; }
    }
}
