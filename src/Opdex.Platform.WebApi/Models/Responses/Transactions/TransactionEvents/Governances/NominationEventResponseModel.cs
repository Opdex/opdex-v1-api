namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Governances
{
    public class NominationEventResponseModel : TransactionEventResponseModel
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Weight { get; set; }
    }
}
