using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.Vaults
{
    public class VaultContractSummary
    {
        public VaultContractSummary(Address lockedToken, ulong genesis, Address owner, UInt256 unassignedSupply)
        {
            LockedToken = lockedToken;
            Genesis = genesis;
            Owner = owner;
            UnassignedSupply = unassignedSupply;
        }

        public Address LockedToken { get; }
        public ulong Genesis { get; }
        public Address Owner { get; }
        public UInt256 UnassignedSupply { get; }
    }
}
