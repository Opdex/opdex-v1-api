using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;

namespace Opdex.Platform.WebApi.Models.Requests.Vault
{
    public class SetVaultOwnerRequest : LocalWalletCredentials
    {
        public string Owner { get; set; }
    }
}
