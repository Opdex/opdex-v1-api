using Opdex.Platform.Application.Abstractions.Models.TokenDtos;

namespace Opdex.Platform.Application.Abstractions.Models
{
    public class MarketDto
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public TokenDto StakingToken { get; set; }
        public TokenDto CrsToken { get; set; }
        public string Owner { get; set; }
        public bool AuthPoolCreators { get; set; }
        public bool AuthProviders { get; set; }
        public bool AuthTraders { get; set; }
        public uint TransactionFee { get; set; }
        public bool MarketFeeEnabled { get; set; }
        public MarketSnapshotDto Summary { get; set; }
    }
}
