using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

/// <summary>
/// A request to quote withdrawing a vote on a vault proposal.
/// </summary>
public class VaultProposalWithdrawVoteQuoteRequest
{
    /// <summary>Amount of CRS tokens to withdraw.</summary>
    /// <example>"50000.00000000"</example>
    public FixedDecimal Amount { get; set; }
}
