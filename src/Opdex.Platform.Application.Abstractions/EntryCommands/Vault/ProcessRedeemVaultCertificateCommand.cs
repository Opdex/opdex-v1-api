using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vault
{
    public class ProcessRedeemVaultCertificateCommand : CreateWalletTransactionCommand
    {
        public ProcessRedeemVaultCertificateCommand(string walletAddress,
                                                    string vault,
                                                    string holder) : base(walletAddress)
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
