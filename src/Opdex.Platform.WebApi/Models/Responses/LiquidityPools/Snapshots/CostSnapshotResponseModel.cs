using Opdex.Platform.WebApi.Models.Responses.OHLC;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;

public class CostSnapshotResponseModel
{
    /// <summary>
    /// The OHLC (open, high, low, close) amount of CRS tokens worth 1 full SRC token during the snapshot period.
    /// </summary>
    public OhlcFixedDecimalResponseModel CrsPerSrc { get; set; }

    /// <summary>
    /// The OHLC (open, high, low, close) amount of SRC tokens worth 1 full CRS token during the snapshot period.
    /// </summary>
    public OhlcFixedDecimalResponseModel SrcPerCrs { get; set; }
}