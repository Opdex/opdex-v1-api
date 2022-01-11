using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;

/// <summary>
/// Command to quote proposing a new minimum vote amount for a vault.
/// </summary>
public class CreateVaultProposalMinimumVoteQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a command to quote proposing a new minimum vote amount for a vault.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="amount">Proposed minimum vote amount.</param>
    /// <param name="description">Proposal description.</param>
    /// <param name="walletAddress">Address of the sender.</param>
    public CreateVaultProposalMinimumVoteQuoteCommand(Address vault, FixedDecimal amount, string description, Address walletAddress) : base(walletAddress)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (amount == FixedDecimal.Zero) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (!description.HasValue()) throw new ArgumentNullException(nameof(description), "Description must be provided.");
        Vault = vault;
        Amount = amount;
        Description = description;
    }

    public Address Vault { get; }
    public FixedDecimal Amount { get; }
    public string Description { get; }
}
