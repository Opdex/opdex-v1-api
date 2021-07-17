namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens
{
    public class DistributionEventResponseModel : TransactionEventResponseModel
    {
        public string VaultAmount { get; set; }
        public string MiningAmount { get; set; }
        public uint PeriodIndex { get; set; }
    }
}
