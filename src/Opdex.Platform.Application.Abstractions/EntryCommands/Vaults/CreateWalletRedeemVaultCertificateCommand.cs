using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
    public class CreateWalletRedeemVaultCertificateCommand : CreateWalletTransactionCommand
    {
        public CreateWalletRedeemVaultCertificateCommand(string walletAddress, string vault) : base(walletAddress)
        {
            if (!vault.HasValue()) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");

            Vault = vault;
        }

        public string Vault { get; }
    }
}
