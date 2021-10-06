namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.MarketTokens
{
    public class MarketTokenEntity : AuditEntity
    {
        public ulong Id { get; set; }
        public ulong MarketId { get; set; }
        public ulong TokenId { get; set; }
    }
}
