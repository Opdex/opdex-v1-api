using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vault
{
    public class ProcessSetVaultOwnerCommand : CreateWalletTransactionCommand
    {
        public ProcessSetVaultOwnerCommand(string walletName,
                                           string walletAddress,
                                           string walletPassword,
                                           string vault,
                                           string owner) : base(walletName, walletAddress, walletPassword)
        {
            if (!vault.HasValue()) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
            if (!owner.HasValue()) throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
            Vault = vault;
            Owner = owner;
        }

        public string Vault { get; }
        public string Owner { get; }
    }
}
