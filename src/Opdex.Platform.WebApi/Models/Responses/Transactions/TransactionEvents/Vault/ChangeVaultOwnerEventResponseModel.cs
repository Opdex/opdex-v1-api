namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault
{
    public class ChangeVaultOwnerEventResponseModel : TransactionEventResponseModel
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
