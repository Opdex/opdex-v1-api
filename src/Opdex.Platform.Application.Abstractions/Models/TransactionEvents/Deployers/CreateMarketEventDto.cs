using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers
{
    public class CreateMarketEventDto : TransactionEventDto
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
        public override TransactionEventType EventType => TransactionEventType.CreateMarketEvent;
    }
}
