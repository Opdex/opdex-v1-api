using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults
{
    public class VaultEntity : AuditEntity
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public Address Address { get; set; }
        public Address Owner { get; set; }
        public ulong Genesis { get; set; }
        public UInt256 UnassignedSupply { get; set; }
    }
}
