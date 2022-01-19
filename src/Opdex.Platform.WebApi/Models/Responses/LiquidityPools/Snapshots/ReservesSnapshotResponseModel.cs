using Opdex.Platform.WebApi.Models.Responses.OHLC;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;

/// <summary>
/// Liquidity pool reserve snapshot details.
/// </summary>
public class ReservesSnapshotResponseModel
{
    /// <summary>
    /// Total amount of locked CRS tokens.
    /// </summary>
    public OhlcFixedDecimalResponseModel Crs { get; set; }

    /// <summary>
    /// Total amount of locked SRC tokens.
    /// </summary>
    public OhlcFixedDecimalResponseModel Src { get; set; }

    /// <summary>
    /// Total USD value of locked reserves.
    /// </summary>
    public OhlcDecimalResponseModel Usd { get; set; }
}