namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Deployers
{
    public class ChangeDeployerOwnerEventResponseModel : TransactionEventResponseModel
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
