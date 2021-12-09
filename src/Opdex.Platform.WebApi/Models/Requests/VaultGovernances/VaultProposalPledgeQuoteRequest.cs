using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

/// <summary>
/// A request to quote pledging to a vault proposal.
/// </summary>
public class VaultProposalPledgeQuoteRequest
{
    /// <summary>Amount of CRS tokens to pledge.</summary>
    /// <example>"25000.00000000"</example>
    public FixedDecimal Amount { get; set; }
}
