namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public class MarketEntity : AuditEntity
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public bool AuthPoolCreators { get; set; }
        public bool AuthProviders { get; set; }
        public bool AuthTraders { get; set; }
        public uint Fee { get; set; }
        public bool Staking { get; set; }
    }
}