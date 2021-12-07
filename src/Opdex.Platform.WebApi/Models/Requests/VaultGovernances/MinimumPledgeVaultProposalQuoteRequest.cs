using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

/// <summary>
/// A request to quote creating a vault proposal for updating the minimum pledge amount.
/// </summary>
public class MinimumPledgeVaultProposalQuoteRequest
{
    /// <summary>Amount of CRS tokens to set as the minimum pledge.</summary>
    /// <example>"25000.00000000"</example>
    public FixedDecimal Amount { get; set; }

    /// <summary>Details of the proposal. This should provide an explanation of the proposal, with a case for why CRS holders should vote in favor of it.</summary>
    /// <example>OVP-1: Request to change minimum pledge amount. See https://www.example.com for details.</example>
    public string Description { get; set; }
}
