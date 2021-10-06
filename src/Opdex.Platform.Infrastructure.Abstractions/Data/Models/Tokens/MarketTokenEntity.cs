namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens
{
    public class MarketTokenEntity : BaseTokenEntity
    {
        public ulong Id { get; set; }
        public ulong MarketId { get; set; }
        public ulong TokenId { get; set; }
    }
}
