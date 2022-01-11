using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;

/// <summary>
/// Command to quote completing a vault proposal.
/// </summary>
public class CreateCompleteVaultProposalQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a command to quote completing a vault proposal.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="walletAddress">Address of the sender.</param>
    public CreateCompleteVaultProposalQuoteCommand(Address vault, ulong proposalId, Address walletAddress) : base(walletAddress)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (proposalId == 0) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than zero.");
        Vault = vault;
        ProposalId = proposalId;
    }

    public Address Vault { get; }
    public ulong ProposalId { get; }
}
