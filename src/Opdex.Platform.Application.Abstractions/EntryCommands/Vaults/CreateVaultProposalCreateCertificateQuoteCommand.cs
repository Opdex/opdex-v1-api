using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;

/// <summary>
/// Command to quote proposing the creation of a vault certificate.
/// </summary>
public class CreateVaultProposalCreateCertificateQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a command to quote proposing the creation of a vault certificate.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="owner">Address of the certificate owner.</param>
    /// <param name="amount">Proposed governance token certificate value.</param>
    /// <param name="description">Proposal description.</param>
    /// <param name="walletAddress">Address of the sender.</param>
    public CreateVaultProposalCreateCertificateQuoteCommand(Address vault, Address owner, FixedDecimal amount, string description, Address walletAddress) : base(walletAddress)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (owner == Address.Empty) throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        if (amount == FixedDecimal.Zero) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (!description.HasValue()) throw new ArgumentNullException(nameof(description), "Description must be provided.");
        Vault = vault;
        Owner = owner;
        Amount = amount;
        Description = description;
    }

    public Address Vault { get; }
    public Address Owner { get; }
    public FixedDecimal Amount { get; }
    public string Description { get; }
}
