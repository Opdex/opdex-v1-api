using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.MiningPools;

/// <summary>
/// Mining pool details.
/// </summary>
public class MiningPoolResponseModel
{
    /// <summary>
    /// Address of the mining pool.
    /// </summary>
    /// <example>tNgQhNxvachxKGvRonk2S8nrpYi44carYv</example>
    public Address Address { get; set; }

    /// <summary>
    /// Address of the related liquidity pool.
    /// </summary>
    /// <example>tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L</example>
    public Address LiquidityPool { get; set; }

    /// <summary>
    /// Block number which the current mining period ends.
    /// </summary>
    /// <example>750000</example>
    public ulong MiningPeriodEndBlock { get; set; }

    /// <summary>
    /// Amount of governance tokens rewarded to the pool per block.
    /// </summary>
    /// <example>"20.00000000"</example>
    public FixedDecimal RewardPerBlock { get; set; }

    /// <summary>
    /// Amount of governance tokens rewarded per liquidity pool token.
    /// </summary>
    /// <example>"0.00050000"</example>
    public FixedDecimal RewardPerLpt { get; set; }

    /// <summary>
    /// Amount of liquidity pool tokens mining in the pool.
    /// </summary>
    /// <example>"2500000.00000000"</example>
    public FixedDecimal TokensMining { get; set; }

    /// <summary>
    /// If liquidity mining is currently active.
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; set; }
}