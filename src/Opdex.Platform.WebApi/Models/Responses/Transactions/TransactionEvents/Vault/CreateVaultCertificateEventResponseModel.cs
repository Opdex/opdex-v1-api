namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault
{
    public class CreateVaultCertificateEventResponseModel : TransactionEventResponseModel
    {
        public string Holder { get; set; }
        public string Amount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}
