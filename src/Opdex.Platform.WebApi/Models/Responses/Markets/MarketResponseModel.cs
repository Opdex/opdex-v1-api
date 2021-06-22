using Opdex.Platform.Application.Abstractions.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Markets
{
    public class MarketResponseModel
    {
        public string Address { get; set; }
        public string Owner { get; set; }
        public TokenResponseModel StakingToken { get; set; }
        public TokenResponseModel CrsToken { get; set; }
        public bool AuthPoolCreators { get; set; }
        public bool AuthTraders { get; set; }
        public bool AuthProviders { get; set; }
        public bool MarketFeeEnabled { get; set; }
        public uint TransactionFee { get; set; }
        public MarketSnapshotResponseModel Summary { get; set; }
    }
}
