using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class TokenSnapshot
    {
        public TokenSnapshot(long tokenId, long marketId, decimal price, SnapshotType type, DateTime startDate, DateTime endDate)
        {
            TokenId = tokenId;
            MarketId = marketId;
            Price = Math.Round(price, 2, MidpointRounding.ToEven);
            SnapshotType = type;
            StartDate = startDate;
            EndDate = endDate;
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

        public void ProcessReservesLog(ReservesLog log, TokenSnapshot crsSnapshot, int srcDecimals)
        {
            var crsReservesRounded = log.ReserveCrs.ToString().ToRoundedDecimal(8, TokenConstants.Cirrus.Decimals);
            var srcReservesRounded = log.ReserveSrc.ToRoundedDecimal(8, srcDecimals);
            
            var srcTokensPerCrsToken = Math.Round(crsReservesRounded / srcReservesRounded, 2, MidpointRounding.AwayFromZero);
            
            Price = Math.Round(srcTokensPerCrsToken * crsSnapshot.Price, 2, MidpointRounding.AwayFromZero);
        }
    }
}