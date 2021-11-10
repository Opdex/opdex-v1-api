using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Markets
{
    public class MarketDto
    {
        public ulong Id { get; set; }
        public Address Address { get; set; }
        public TokenDto StakingToken { get; set; }
        public TokenDto CrsToken { get; set; }
        public Address PendingOwner { get; set; }
        public Address Owner { get; set; }
        public bool AuthPoolCreators { get; set; }
        public bool AuthProviders { get; set; }
        public bool AuthTraders { get; set; }
        public uint TransactionFee { get; set; }
        public bool MarketFeeEnabled { get; set; }
        public MarketSnapshotDto Summary { get; set; }
    }
}
