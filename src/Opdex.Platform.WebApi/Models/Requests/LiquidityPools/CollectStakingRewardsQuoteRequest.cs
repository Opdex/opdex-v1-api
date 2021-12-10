using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

/// <summary>
/// Request to quote collecting staking rewards from a pool.
/// </summary>
public class CollectStakingRewardsQuoteRequest
{
    /// <summary>
    /// Option to liquidate earned liquidity pool tokens from staking back into the pool reserves tokens.
    /// </summary>
    /// <example>true</example>
    [Required]
    public bool Liquidate { get; set; }
}