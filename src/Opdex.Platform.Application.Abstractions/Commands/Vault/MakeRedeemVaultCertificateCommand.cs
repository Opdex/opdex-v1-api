using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Vault
{
    public class MakeRedeemVaultCertificateCommand : MakeWalletTransactionCommand
    {
        public MakeRedeemVaultCertificateCommand(string walletName,
                                                 string walletAddress,
                                                 string walletPassword,
                                                 string vault,
                                                 string holder) : base(walletName, walletAddress, walletPassword)
        {
            if (!vault.HasValue()) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
            if (!holder.HasValue()) throw new ArgumentNullException(nameof(holder), "Holder address must be set.");
            Vault = vault;
            Holder = holder;
        }

        public string Vault { get; }
        public string Holder { get; }
    }
}
