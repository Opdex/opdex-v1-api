using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;

public class TokenWrappedEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong TokenId { get; set; }
    public Address Owner { get; set; }
    public ushort NativeChainTypeId { get; set; }
    public string NativeAddress { get; set; }
    public bool Validated { get; set; }
}
