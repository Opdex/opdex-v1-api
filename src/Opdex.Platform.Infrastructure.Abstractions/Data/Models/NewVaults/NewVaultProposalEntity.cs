using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.NewVaults
{
    public class NewVaultProposalEntity : AuditEntity
    {
        public ulong Id { get; set; }
        public UInt256 Amount { get; set; }
        public Address Wallet { get; set; }
        public string Description { get; set; }
        public byte Type { get; set; }
        public byte Status { get; set; }
        public ulong Expiration { get; set; }
        public ulong YesAmount { get; set; }
        public ulong NoAmount { get; set; }
        public ulong PledgeAmount { get; set; }
    }
}
