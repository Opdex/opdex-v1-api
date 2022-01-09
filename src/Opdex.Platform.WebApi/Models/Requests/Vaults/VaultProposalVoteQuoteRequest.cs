using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults;

/// <summary>
/// A request to quote voting on a vault proposal.
/// </summary>
public class VaultProposalVoteQuoteRequest
{
    /// <summary>Amount of CRS tokens to vote with.</summary>
    /// <example>"50000.00000000"</example>
    public FixedDecimal Amount { get; set; }

    /// <summary>If the vote is in favor of the proposal.</summary>
    /// <example>true</example>
    public bool InFavor { get; set; }
}
