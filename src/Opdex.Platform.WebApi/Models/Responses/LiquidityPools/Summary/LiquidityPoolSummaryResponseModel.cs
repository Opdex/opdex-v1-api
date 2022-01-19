namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

/// <summary>
/// Liquidity pool summary.
/// </summary>
public class LiquidityPoolSummaryResponseModel
{
    /// <summary>
    /// Locked reserves summary.
    /// </summary>
    public ReservesResponseModel Reserves { get; set; }

    /// <summary>
    /// Rewards details based on transaction volume.
    /// </summary>
    public RewardsResponseModel Rewards { get; set; }

    /// <summary>
    /// Current volume based on the day.
    /// </summary>
    public VolumeResponseModel Volume { get; set; }

    /// <summary>
    /// The like for like cost of each token in the pool.
    /// </summary>
    public CostResponseModel Cost { get; set; }

    /// <summary>
    /// Staking details based on the pool's current status.
    /// </summary>
    public StakingResponseModel Staking { get; set; }
}
