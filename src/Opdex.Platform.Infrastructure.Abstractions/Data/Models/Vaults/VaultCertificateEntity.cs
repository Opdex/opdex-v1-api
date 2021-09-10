using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX
{
    public class VaultCertificateEntity : AuditEntity
    {
        public long Id { get; set; }
        public long VaultId { get; set; }
        public Address Owner { get; set; }
        public UInt256 Amount { get; set; }
        public ulong VestedBlock { get; set; }
        public bool Redeemed { get; set; }
        public bool Revoked { get; set; }
    }
}
