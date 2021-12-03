using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;

public class LiquidityPoolEntity : AuditEntity
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public ulong SrcTokenId { get; set; }
    public ulong LpTokenId { get; set; }
    public ulong MarketId { get; set; }
}