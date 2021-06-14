using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class TokenSnapshot
    {
        public TokenSnapshot(long tokenId, long marketId, SnapshotType snapshotType, DateTime dateTime)
        {
            TokenId = tokenId;
            MarketId = marketId;
            SnapshotType = snapshotType;
            Price = 0.00m;
            StartDate = dateTime.ToStartOf(snapshotType);
            EndDate = dateTime.ToEndOf(snapshotType);
        }

        public TokenSnapshot(long id, long tokenId, long marketId, decimal price, int snapshotType, DateTime startDate, DateTime endDate)
        {
            Id = id;
            TokenId = tokenId;
            MarketId = marketId;
            Price = Math.Round(price, 2, MidpointRounding.ToEven);;
            SnapshotType = (SnapshotType)snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }

        public long Id { get; }
        public long TokenId { get; }
        public long MarketId { get; }
        public decimal Price { get; private set; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public DateTime LastUpdated { get; private set; }

        public void UpdatePrice(decimal price)
        {
            Price = price;
            LastUpdated = DateTime.UtcNow;
        }

        public void ProcessReservesLog(ReservesLog log, decimal crsUsd, ulong srcSats)
        {
            const int crsDecimals = TokenConstants.Cirrus.Decimals;

            var crsPerSrc = log.CrsPerSrc(srcSats).ToRoundedDecimal(crsDecimals, crsDecimals);

            var price = Math.Round(crsPerSrc * crsUsd, 2, MidpointRounding.AwayFromZero);

            UpdatePrice(price);
        }
    }
}