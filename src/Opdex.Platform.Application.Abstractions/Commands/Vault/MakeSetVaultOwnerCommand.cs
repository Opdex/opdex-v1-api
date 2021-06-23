using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;

namespace Opdex.Platform.Application.Abstractions.Commands.Vault
{
    public class MakeSetVaultOwnerCommand : MakeWalletTransactionCommand
    {
        public MakeSetVaultOwnerCommand(string walletAddress,
                                        string vault,
                                        string owner) : base(walletAddress)
        {
            Vault = vault;
            Owner = owner;
        }

        public string Vault { get; }
        public string Owner { get; }
    }
}
