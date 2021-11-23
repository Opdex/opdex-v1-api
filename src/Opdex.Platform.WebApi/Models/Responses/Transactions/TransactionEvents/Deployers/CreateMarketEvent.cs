using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Deployers
{
    public class CreateMarketEvent : TransactionEvent
    {
        public Address Market { get; set; }
        public Address Owner { get; set; }
        public Address Router { get; set; }
        public bool AuthPoolCreators { get; set; }
        public bool AuthProviders { get; set; }
        public bool AuthTraders { get; set; }
        public uint TransactionFee { get; set; }
        public Address StakingToken { get; set; }
        public bool EnableMarketFee { get; set; }
    }
}
