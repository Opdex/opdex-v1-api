using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Domain.Models
{
    public class TokenSnapshot
    {
        public TokenSnapshot(long tokenId, decimal price, SnapshotType type, DateTime startDate, DateTime endDate)
        {
            TokenId = tokenId;
            Price = Math.Round(price, 2, MidpointRounding.ToEven);
            SnapshotType = type;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public TokenSnapshot(long id, long tokenId, decimal price, int snapshotType, DateTime startDate, DateTime endDate)
        {
            Id = id;
            TokenId = tokenId;
            Price = Math.Round(price, 2, MidpointRounding.ToEven);;
            SnapshotType = (SnapshotType)snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public long Id { get; }
        public long TokenId { get; }
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

        public void ProcessReservesLog(ReservesLog log, TokenSnapshot crsSnapshot, Token crs, int srcDecimals)
        {
            var crsReservesRounded = log.ReserveCrs.ToString().ToRoundedDecimal(8, crs.Decimals);
            var srcReservesRounded = log.ReserveSrc.ToRoundedDecimal(8, srcDecimals);
            
            var srcTokensPerCrsToken = Math.Round(crsReservesRounded / srcReservesRounded, 2, MidpointRounding.AwayFromZero);
            
            Price = Math.Round(srcTokensPerCrsToken * crsSnapshot.Price, 2, MidpointRounding.AwayFromZero);
        }
    }
}