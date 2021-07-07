namespace Opdex.Platform.Domain.Models.Governances
{
    public class MiningGovernanceContractSummary
    {
        public MiningGovernanceContractSummary(string address, string minedToken, ulong nominationPeriodEnd,
            uint miningPoolsFunded, string miningPoolReward, ulong miningDuration)
        {
            Address = address;
            MinedToken = minedToken;
            NominationPeriodEnd = nominationPeriodEnd;
            MiningPoolsFunded = miningPoolsFunded;
            MiningPoolReward = miningPoolReward;
            MiningDuration = miningDuration;
        }
        public string Address { get; }
        public string MinedToken { get; }
        public ulong NominationPeriodEnd { get; }
        public uint MiningPoolsFunded { get; }
        public string MiningPoolReward { get; }
        public ulong MiningDuration { get; }
    }
}
