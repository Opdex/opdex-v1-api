using Opdex.Platform.WebApi.Models.Responses.OHLC;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;

/// <summary>
/// Cost snapshot details.
/// </summary>
public class CostSnapshotResponseModel
{
    /// <summary>
    /// Amount of CRS tokens worth 1 full SRC token during the snapshot period.
    /// </summary>
    public OhlcFixedDecimalResponseModel CrsPerSrc { get; set; }

    /// <summary>
    /// Amount of SRC tokens worth 1 full CRS token during the snapshot period.
    /// </summary>
    public OhlcFixedDecimalResponseModel SrcPerCrs { get; set; }
}