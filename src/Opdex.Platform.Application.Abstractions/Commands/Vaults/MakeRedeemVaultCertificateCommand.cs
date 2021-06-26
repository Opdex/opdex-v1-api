using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Vaults
{
    public class MakeRedeemVaultCertificateCommand : MakeWalletTransactionCommand
    {
        public MakeRedeemVaultCertificateCommand(string walletAddress, string vault) : base(walletAddress)
        {
            if (!vault.HasValue()) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");

            Vault = vault;
        }

        public string Vault { get; }
    }
}
