namespace Opdex.Platform.Domain.Models
{
    public class StakingTokenContractSummary
    {
        public StakingTokenContractSummary(string address, string miningGovernance, uint periodIndex, string owner, 
            ulong genesis, ulong periodDuration)
        {
            Address = address;
            MiningGovernance = miningGovernance;
            PeriodIndex = periodIndex;
            Owner = owner;
            Genesis = genesis;
            PeriodDuration = periodDuration;
        }
        
        public string Address { get; }
        public string MiningGovernance { get; }
        public uint PeriodIndex { get; }
        public string Owner { get; }
        public ulong Genesis { get; }
        public ulong PeriodDuration { get; }
    }
}