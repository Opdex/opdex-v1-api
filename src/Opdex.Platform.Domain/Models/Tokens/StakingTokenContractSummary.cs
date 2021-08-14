namespace Opdex.Platform.Domain.Models.Tokens
{
    public class StakingTokenContractSummary
    {
        public StakingTokenContractSummary(string address, string miningGovernance, uint periodIndex, string vault,
            ulong genesis, ulong periodDuration)
        {
            Address = address;
            MiningGovernance = miningGovernance;
            PeriodIndex = periodIndex;
            Vault = vault;
            Genesis = genesis;
            PeriodDuration = periodDuration;
        }

        public string Address { get; }
        public string MiningGovernance { get; }
        public uint PeriodIndex { get; }
        public string Vault { get; }
        public ulong Genesis { get; }
        public ulong PeriodDuration { get; }
    }
}
