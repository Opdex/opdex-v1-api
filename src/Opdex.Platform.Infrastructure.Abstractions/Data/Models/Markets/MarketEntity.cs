namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets
{
    public class MarketEntity : AuditEntity
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public long DeployerId { get; set; }
        public long? StakingTokenId { get; set; }
        public bool AuthPoolCreators { get; set; }
        public bool AuthProviders { get; set; }
        public bool AuthTraders { get; set; }
        public uint Fee { get; set; }
    }
}