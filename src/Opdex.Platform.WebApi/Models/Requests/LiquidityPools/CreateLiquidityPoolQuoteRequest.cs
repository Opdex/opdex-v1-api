using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

/// <summary>
/// Request to quote the creation of a liquidity pool.
/// </summary>
public class CreateLiquidityPoolQuoteRequest
{
    /// <summary>
    /// Address of the SRC token to create a liquidity pool for.
    /// </summary>
    /// <example>tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3</example>
    [Required]
    public Address Token { get; set; }

    /// <summary>
    /// Address of the market contract to add a liquidity pool to.
    /// </summary>
    /// <example>t7RorA7xQCMVYKPM1ibPE1NSswaLbpqLQb</example>
    [Required]
    public Address Market { get; set; }
}