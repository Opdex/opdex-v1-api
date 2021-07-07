namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Governances
{
    public class MiningGovernanceEntity : AuditEntity
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public string Address { get; set; }
        public ulong NominationPeriodEnd { get; set; }
        public ulong MiningDuration { get; set; }
        public uint MiningPoolsFunded { get; set; }
        public string MiningPoolReward { get; set; }
    }
}
