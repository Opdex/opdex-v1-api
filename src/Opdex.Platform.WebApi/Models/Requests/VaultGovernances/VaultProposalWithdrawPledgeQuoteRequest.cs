using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

/// <summary>
/// A request to quote withdrawing a pledge to a vault proposal.
/// </summary>
public class VaultProposalWithdrawPledgeQuoteRequest
{
    /// <summary>Amount of CRS tokens to withdraw.</summary>
    /// <example>"25000.00000000"</example>
    public FixedDecimal Amount { get; set; }
}
