namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Deployers
{
    public class CreateMarketEventResponseModel : TransactionEventResponseModel
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
    }
}
