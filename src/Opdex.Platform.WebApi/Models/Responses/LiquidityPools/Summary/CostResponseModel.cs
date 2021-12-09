using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

/// <summary>
/// Token pairing cost summary.
/// </summary>
public class CostResponseModel
{
    /// <summary>
    /// Amount of CRS tokens worth 1 full SRC token.
    /// </summary>
    /// <example>10.0000000</example>
    [NotNull]
    public FixedDecimal CrsPerSrc { get; set; }

    /// <summary>
    /// Amount of SRC tokens worth 1 full CRS token.
    /// </summary>
    /// <example>0.10000000</example>
    [NotNull]
    public FixedDecimal SrcPerCrs { get; set; }
}