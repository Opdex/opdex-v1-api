using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;

/// <summary>
/// Command to quote proposing revoking a vault certificate.
/// </summary>
public class CreateVaultProposalRevokeCertificateQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a command to quote proposing revoking of a vault certificate.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="owner">Address of the certificate owner.</param>
    /// <param name="description">Proposal description.</param>
    /// <param name="walletAddress">Address of the sender.</param>
    public CreateVaultProposalRevokeCertificateQuoteCommand(Address vault, Address owner, string description, Address walletAddress) : base(walletAddress)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (owner == Address.Empty) throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        if (!description.HasValue()) throw new ArgumentNullException(nameof(description), "Description must be provided.");
        Vault = vault;
        Owner = owner;
        Description = description;
    }

    public Address Vault { get; }
    public Address Owner { get; }
    public string Description { get; }
}
