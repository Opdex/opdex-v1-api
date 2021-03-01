namespace Opdex.Core.Infrastructure.Abstractions.Data.Models
{
    public class MarketSnapshotEntity : AuditEntity
    {
        public long Id { get; set; }
        public long TokenCount { get; set; }
        public long PairCount { get; set; }
        public long DailyTransactionCount { get; set; }
        public decimal CrsPrice { get; set; }
        public decimal Liquidity { get; set; }
        public decimal DailyFees { get; set; }
        public decimal DailyVolume { get; set; }
        public ulong Block { get; set; }
    }
}