using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults;

/// <summary>
/// A request to quote creating a vault proposal for updating the minimum vote amount.
/// </summary>
public class MinimumVoteVaultProposalQuoteRequest
{
    /// <summary>Amount of CRS tokens to set as the minimum vote.</summary>
    /// <example>"50000.00000000"</example>
    public FixedDecimal Amount { get; set; }

    /// <summary>Details of the proposal. This should provide an explanation of the proposal, with a case for why CRS holders should vote in favor of it.</summary>
    /// <example>OVP-1: Request to change minimum vote amount. See https://www.example.com for details.</example>
    public string Description { get; set; }
}
