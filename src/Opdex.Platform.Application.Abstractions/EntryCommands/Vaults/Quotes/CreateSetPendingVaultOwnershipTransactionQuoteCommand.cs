using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults.Quotes;

/// <summary>
/// Quote a transaction to set pending vault ownership.
/// </summary>
public class CreateSetPendingVaultOwnershipTransactionQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a set pending vault ownership quote command.
    /// </summary>
    /// <param name="vault">The address of the vault.</param>
    /// <param name="currentOwner">The address of the current owner.</param>
    /// <param name="newOwner">The address of the new owner.</param>
    public CreateSetPendingVaultOwnershipTransactionQuoteCommand(Address vault, Address currentOwner, Address newOwner) : base(currentOwner)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        NewOwner = newOwner != Address.Empty ? newOwner : throw new ArgumentNullException(nameof(newOwner), "New owner address must be provided.");
    }

    public Address Vault { get; }
    public Address NewOwner { get; }
}