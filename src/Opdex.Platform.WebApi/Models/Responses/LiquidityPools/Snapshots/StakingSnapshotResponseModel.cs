using Opdex.Platform.WebApi.Models.Responses.OHLC;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;

/// <summary>
/// Staking snapshot details.
/// </summary>
public class StakingSnapshotResponseModel
{
    /// <summary>
    /// Total number of tokens staking.
    /// </summary>
    public OhlcFixedDecimalResponseModel Weight { get; set; }

    /// <summary>
    /// Total USD value of tokens staking.
    /// </summary>
    public OhlcDecimalResponseModel Usd { get; set; }
}