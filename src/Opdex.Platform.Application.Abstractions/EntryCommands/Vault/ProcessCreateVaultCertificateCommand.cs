using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vault
{
    public class ProcessCreateVaultCertificateCommand : CreateWalletTransactionCommand
    {
        public ProcessCreateVaultCertificateCommand(string walletAddress, string vault, string holder, string amount) : base(walletAddress)
        {
            if (!vault.HasValue()) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
            if (!holder.HasValue()) throw new ArgumentNullException(nameof(holder), "Holder address must be set.");
            if (!amount.IsNumeric()) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must only contain numeric digits.");

            Vault = vault;
            Holder = holder;
            Amount = amount;
        }

        public string Vault { get; }
        public string Holder { get; }
        public string Amount { get; }
    }
}
