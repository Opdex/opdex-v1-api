using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

/// <summary>
/// Request to quote adding liquidity to a pool.
/// </summary>
public class CalculateAddLiquidityAmountsRequestModel
{
    /// <summary>
    /// Amount of tokens to be deposited into a pool.
    /// </summary>
    /// <example>"10.00000000"</example>
    [Required]
    public FixedDecimal AmountIn { get; set; }

    /// <summary>
    /// Address of the deposited token or "CRS" for Cirrus token.
    /// </summary>
    /// <example>CRS</example>
    [Required]
    public Address TokenIn { get; set; }
}