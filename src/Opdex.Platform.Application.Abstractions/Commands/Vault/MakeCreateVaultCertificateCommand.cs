using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Vault
{
    public class MakeCreateVaultCertificateCommand : MakeWalletTransactionCommand
    {
        public MakeCreateVaultCertificateCommand(string walletName,
                                                 string walletAddress,
                                                 string walletPassword,
                                                 string vault,
                                                 string holder,
                                                 string amount) : base(walletName, walletAddress, walletPassword)
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
