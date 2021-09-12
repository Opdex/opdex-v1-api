using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class StakingTokenContractSummary
    {
        public StakingTokenContractSummary(Address address, Address miningGovernance, uint periodIndex, Address vault,
            ulong genesis, ulong periodDuration)
        {
            Address = address;
            MiningGovernance = miningGovernance;
            PeriodIndex = periodIndex;
            Vault = vault;
            Genesis = genesis;
            PeriodDuration = periodDuration;
        }

        public Address Address { get; }
        public Address MiningGovernance { get; }
        public uint PeriodIndex { get; }
        public Address Vault { get; }
        public ulong Genesis { get; }
        public ulong PeriodDuration { get; }
    }
}
