using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;

namespace Opdex.Platform.Application.Abstractions.Commands.Vault
{
    public class MakeSetVaultOwnerCommand : MakeWalletTransactionCommand
    {
        public MakeSetVaultOwnerCommand(string walletName,
                                        string walletAddress,
                                        string walletPassword,
                                        string vault,
                                        string owner) : base(walletName, walletAddress, walletPassword)
        {
            Vault = vault;
            Owner = owner;
        }

        public string Vault { get; }
        public string Owner { get; }
    }
}
