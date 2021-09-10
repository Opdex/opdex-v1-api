using System;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.OHLC;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class TokenSnapshot
    {
        public TokenSnapshot(long tokenId, long marketId, SnapshotType snapshotType, DateTime dateTime)
        {
            TokenId = tokenId;
            MarketId = marketId;
            SnapshotType = snapshotType;
            Price = new OhlcDecimalSnapshot();
            StartDate = dateTime.ToStartOf(snapshotType);
            EndDate = dateTime.ToEndOf(snapshotType);
        }

        public TokenSnapshot(long id, long tokenId, long marketId, OhlcDecimalSnapshot price, SnapshotType snapshotType, DateTime startDate,
                             DateTime endDate, DateTime modifiedDate)
        {
            Id = id;
            TokenId = tokenId;
            MarketId = marketId;
            Price = price;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
            ModifiedDate = modifiedDate;
        }

        public long Id { get; private set; }
        public long TokenId { get; }
        public long MarketId { get; }
        public OhlcDecimalSnapshot Price { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        public void UpdatePrice(decimal price, bool reset = false)
        {
            Price.Update(Math.Round(price, 8, MidpointRounding.AwayFromZero), reset);
            ModifiedDate = DateTime.UtcNow;
        }

        public void UpdatePrice(ulong reserveCrs, UInt256 reserveSrc, decimal crsUsd, ulong srcSats)
        {
            var price = reserveCrs
                .Token0PerToken1(reserveSrc, srcSats)
                .TotalFiat(crsUsd, TokenConstants.Cirrus.Sats);

            UpdatePrice(price);
        }

        public void ResetStaleSnapshot(UInt256 crsPerSrc, decimal crsUsd, DateTime blockTime)
        {
            Id = 0;

            UpdatePrice(crsPerSrc.TotalFiat(crsUsd, TokenConstants.Cirrus.Sats), true);

            StartDate = blockTime.ToStartOf(SnapshotType);
            EndDate = blockTime.ToEndOf(SnapshotType);
        }

        public void ResetStaleSnapshot(decimal price, DateTime blockTime)
        {
            Id = 0;

            UpdatePrice(price, true);

            StartDate = blockTime.ToStartOf(SnapshotType);
            EndDate = blockTime.ToEndOf(SnapshotType);
        }
    }
}
