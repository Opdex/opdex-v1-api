using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools;

/// <summary>
/// Liquidity pool reserves change event.
/// </summary>
public class ReservesChangeEvent : TransactionEvent
{
    /// <summary>
    /// Updated CRS reserves
    /// </summary>
    /// <example>"1000.00000000"</example>
    public FixedDecimal Crs { get; set; }

    /// <summary>
    /// Updates SRC reserves
    /// </summary>
    /// <example>"100000.00000000"</example>
    public FixedDecimal Src { get; set; }
}
