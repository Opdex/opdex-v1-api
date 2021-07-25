namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault
{
    public class RevokeVaultCertificateEventResponseModel : TransactionEventResponseModel
    {
        public string Holder { get; set; }
        public string NewAmount { get; set; }
        public string OldAmount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}
