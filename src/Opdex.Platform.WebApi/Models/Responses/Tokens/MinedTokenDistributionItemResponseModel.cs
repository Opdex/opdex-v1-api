using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens;

/// <summary>
/// Distribution item details.
/// </summary>
public class MinedTokenDistributionItemResponseModel
{
    /// <summary>
    /// Amount of tokens distributed to the vault contract
    /// </summary>
    public FixedDecimal Vault { get; set; }

    /// <summary>
    /// Amount of tokens distributed to the mining governance contract
    /// </summary>
    public FixedDecimal MiningGovernance { get; set; }

    /// <summary>
    /// Block height of the distribution
    /// </summary>
    public ulong Block { get; set; }
}
