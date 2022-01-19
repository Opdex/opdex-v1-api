using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;

/// <summary>
/// Volume snapshot details.
/// </summary>
public class VolumeSnapshotResponseModel
{
    /// <summary>
    /// Amount of CRS token volume.
    /// </summary>
    /// <example>"400000.00000000"</example>
    public FixedDecimal Crs { get; set; }

    /// <summary>
    /// Amount of SRC token volume.
    /// </summary>
    /// <example>"8000000.00000000"</example>
    public FixedDecimal Src { get; set; }

    /// <summary>
    /// USD value of token volume.
    /// </summary>
    /// <example>50000000</example>
    public decimal Usd { get; set; }
}