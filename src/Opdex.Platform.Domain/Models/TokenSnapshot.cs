using System;

namespace Opdex.Platform.Domain.Models
{
    public class TokenSnapshot
    {
        public TokenSnapshot(long tokenId, decimal price, SnapshotType type, DateTime startDate, DateTime endDate)
        {
            TokenId = tokenId;
            Price = price;
            SnapshotType = type;
            SnapshotStartDate = startDate;
            SnapshotEndDate = endDate;
        }
        
        public TokenSnapshot(long id, long tokenId, decimal price, int snapshotType, DateTime startDate, DateTime endDate)
        {
            Id = id;
            TokenId = tokenId;
            Price = price;
            SnapshotType = (SnapshotType)snapshotType;
            SnapshotStartDate = startDate;
            SnapshotEndDate = endDate;
        }
        
        public long Id { get; }
        public long TokenId { get; }
        public decimal Price { get; private set; }
        public SnapshotType SnapshotType { get; }
        public DateTime SnapshotStartDate { get; }
        public DateTime SnapshotEndDate { get; }
        public DateTime LastUpdated { get; private set; }

        public void UpdatePrice(decimal price)
        {
            Price = price;
            LastUpdated = DateTime.UtcNow;
        }
    }
}