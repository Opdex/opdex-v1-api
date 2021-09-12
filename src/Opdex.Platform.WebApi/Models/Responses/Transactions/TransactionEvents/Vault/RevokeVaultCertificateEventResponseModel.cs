using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault
{
    public class RevokeVaultCertificateEventResponseModel : TransactionEventResponseModel
    {
        public Address Holder { get; set; }
        public FixedDecimal NewAmount { get; set; }
        public FixedDecimal OldAmount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}
