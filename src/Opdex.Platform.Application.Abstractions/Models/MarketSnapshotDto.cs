using System;

namespace Opdex.Platform.Application.Abstractions.Models
{
    public class MarketSnapshotDto
    {
        public long Id { get; set; }
        public long MarketId { get; set; }
        public long TransactionCount { get; set; }
        public decimal Liquidity { get; set; }
        public decimal Volume { get; set; }
        public decimal Weight { get; set; }
        public decimal ProviderRewards { get; set; }
        public decimal StakerRewards { get; set; }
        public int SnapshotType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}