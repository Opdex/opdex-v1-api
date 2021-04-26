using System;

namespace Opdex.Platform.Domain.Models
{
    public class MarketSnapshot
    {
        public MarketSnapshot(long marketId, long transactionCount, decimal liquidity, decimal volume, decimal weight,
            decimal providerRewards, decimal stakerRewards, SnapshotType snapshotType, DateTime startDate, DateTime endDate)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }
            
            if (transactionCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(transactionCount));
            }

            MarketId = marketId;
            TransactionCount = transactionCount;
            Liquidity = liquidity;
            Volume = volume;
            Weight = weight;
            ProviderRewards = providerRewards;
            StakerRewards = stakerRewards;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public MarketSnapshot(long id, long marketId, long transactionCount, decimal liquidity, decimal volume, decimal weight,
            decimal providerRewards, decimal stakerRewards, SnapshotType snapshotType, DateTime startDate, DateTime endDate)
        {
            Id = id;
            MarketId = marketId;
            TransactionCount = transactionCount;
            Liquidity = liquidity;
            Volume = volume;
            Weight = weight;
            ProviderRewards = providerRewards;
            StakerRewards = stakerRewards;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public long Id { get; }
        public long MarketId { get; }
        public long TransactionCount { get; }
        public decimal Liquidity { get; }
        public decimal Volume { get; }
        public decimal Weight { get; }
        public decimal ProviderRewards { get; }
        public decimal StakerRewards { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
    }
}