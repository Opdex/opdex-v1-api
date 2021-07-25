namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers
{
    public class CreateMarketEventDto : TransactionEventDto
    {
        public string Market { get; set; }
        public string Owner { get; set; }
        public string Router { get; set; }
        public bool AuthPoolCreators { get; set; }
        public bool AuthProviders { get; set; }
        public bool AuthTraders { get; set; }
        public uint TransactionFee { get; set; }
        public string StakingToken { get; set; }
        public bool EnableMarketFee { get; set; }
        public override TransactionEventType EventType => TransactionEventType.CreateMarketEvent;
    }
}
