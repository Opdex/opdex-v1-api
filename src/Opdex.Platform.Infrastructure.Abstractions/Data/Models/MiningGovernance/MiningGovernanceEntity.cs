namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernance
{
    public class MiningGovernanceEntity
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public long TokenId { get; set; }
        public string Balance { get; set; }
        public ulong NominationPeriodEnd { get; set; }
        public int MiningPoolsFunded { get; set; }
        public string MiningPoolReward { get; set; }
    }
}