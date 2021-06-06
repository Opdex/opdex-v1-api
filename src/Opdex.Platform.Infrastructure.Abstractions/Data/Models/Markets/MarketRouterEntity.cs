namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets
{
    public class MarketRouterEntity : AuditEntity
    {
        public long Id { get; set; }
        public long MarketId { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}