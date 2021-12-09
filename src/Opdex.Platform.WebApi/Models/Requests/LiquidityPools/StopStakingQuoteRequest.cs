using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

/// <summary>
/// Request to quote stopping staking in a pool.
/// </summary>
public class StopStakingQuoteRequest
{
    /// <summary>
    /// Option to liquidate earned liquidity pool tokens from staking, back into the pool reserve tokens.
    /// </summary>
    /// <example>true</example>
    [Required]
    public bool Liquidate { get; set; }

    /// <summary>
    /// Amount of governance tokens to stop staking with.
    /// </summary>
    /// <example>"25000.00000000"</example>
    [Required]
    public FixedDecimal Amount { get; set; }
}