using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens;

/// <summary>
/// Distribution schedule details.
/// </summary>
public class MinedTokenDistributionScheduleResponseModel
{
    /// <summary>
    /// Address of the vault contract
    /// </summary>
    public Address Vault { get; set; }

    /// <summary>
    /// Address of the mining governance contract
    /// </summary>
    public Address MiningGovernance { get; set; }

    /// <summary>
    /// Block number at which the next distribution becomes available
    /// </summary>
    public ulong NextDistributionBlock { get; set; }

    /// <summary>
    /// Previous distribution details
    /// </summary>
    public MinedTokenDistributionItemResponseModel Previous { get; set; }
}
