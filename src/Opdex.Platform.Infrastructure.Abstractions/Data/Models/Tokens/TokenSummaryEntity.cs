namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens
{
    public class TokenSummaryEntity : AuditEntity
    {
        public ulong Id { get; set; }
        public ulong MarketId { get; set; }
        public ulong TokenId { get; set; }
        public ulong DailyChangeUsd { get; set; }
        public ulong PriceUsd { get; set; }
    }
}
