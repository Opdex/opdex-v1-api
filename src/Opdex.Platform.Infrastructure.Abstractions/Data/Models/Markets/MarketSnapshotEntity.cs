using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets
{
    public class MarketSnapshotEntity : SnapshotEntity
    {
        public long Id { get; set; }
        public long MarketId { get; set; }
        public long TransactionCount { get; set; }
        public decimal Liquidity { get; set; }
        public decimal Volume { get; set; }
        public decimal Weight { get; set; }
        public decimal ProviderRewards { get; set; }
        public decimal StakerRewards { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}