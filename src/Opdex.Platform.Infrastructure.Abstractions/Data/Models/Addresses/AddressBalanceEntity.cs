using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;

public class AddressBalanceEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong TokenId { get; set; }
    public Address Owner { get; set; }
    public UInt256 Balance { get; set; }
}