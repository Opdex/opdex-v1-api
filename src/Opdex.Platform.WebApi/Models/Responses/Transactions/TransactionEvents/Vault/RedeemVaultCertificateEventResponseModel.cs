using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault
{
    public class RedeemVaultCertificateEventResponseModel : TransactionEventResponseModel
    {
        public Address Holder { get; set; }
        public FixedDecimal Amount { get; set; }
        public ulong VestedBlock { get; set; }
    }
}
