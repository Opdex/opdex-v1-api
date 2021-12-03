using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;

public class MarketPermissionEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong MarketId { get; set; }
    public Address User { get; set; }
    public int Permission { get; set; }
    public bool IsAuthorized { get; set; }
    public Address Blame { get; set; }
}