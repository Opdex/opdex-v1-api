using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
    public class CreateWalletSetVaultOwnerCommand : CreateWalletTransactionCommand
    {
        public CreateWalletSetVaultOwnerCommand(string walletAddress, string vault, string owner) : base(walletAddress)
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
