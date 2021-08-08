namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools
{
    public class LiquidityPoolEntity : AuditEntity
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public long SrcTokenId { get; set; }
        public long LpTokenId { get; set; }
        public long MarketId { get; set; }
    }
}