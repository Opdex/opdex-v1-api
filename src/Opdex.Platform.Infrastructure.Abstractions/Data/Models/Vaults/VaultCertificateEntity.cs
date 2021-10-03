using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults
{
    public class VaultCertificateEntity : AuditEntity
    {
        public ulong Id { get; set; }
        public ulong VaultId { get; set; }
        public Address Owner { get; set; }
        public UInt256 Amount { get; set; }
        public ulong VestedBlock { get; set; }
        public bool Redeemed { get; set; }
        public bool Revoked { get; set; }
    }
}
