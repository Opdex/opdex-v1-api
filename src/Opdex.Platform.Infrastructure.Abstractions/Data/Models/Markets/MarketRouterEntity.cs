using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets
{
    public class MarketRouterEntity : AuditEntity
    {
        public ulong Id { get; set; }
        public ulong MarketId { get; set; }
        public Address Address { get; set; }
        public bool IsActive { get; set; }
    }
}
