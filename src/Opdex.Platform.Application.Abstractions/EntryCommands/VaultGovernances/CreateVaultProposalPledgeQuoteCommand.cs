using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;

/// <summary>
/// Command to quote pledging to a vault proposal.
/// </summary>
public class CreateVaultProposalPledgeQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a command to quote voting on a vault proposal.
    /// </summary>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="amount">Amount of CRS to vote.</param>
    /// <param name="inFavor">If the vote is in favor of the proposal.</param>
    /// <param name="walletAddress">Address of the sender.</param>
    public CreateVaultProposalPledgeQuoteCommand(Address vault, ulong proposalId, FixedDecimal amount, Address walletAddress) : base(walletAddress)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
        if (proposalId == 0) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than zero.");
        if (amount == FixedDecimal.Zero) throw new ArgumentOutOfRangeException(nameof(amount), "Pledge amount must be greater than zero.");
        Vault = vault;
        ProposalId = proposalId;
        Amount = amount;
    }

    public Address Vault { get; }
    public ulong ProposalId { get; }
    public FixedDecimal Amount { get; }
}
