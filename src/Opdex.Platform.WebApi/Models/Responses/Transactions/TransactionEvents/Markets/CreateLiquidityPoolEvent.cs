using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets;

/// <summary>
/// Create liquidity pool event.
/// </summary>
public class CreateLiquidityPoolEvent : TransactionEvent
{
    /// <summary>
    /// Address of the SRC token in the pool.
    /// </summary>
    /// <example>tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3</example>
    public Address Token { get; set; }

    /// <summary>
    /// Address of the created liquidity pool.
    /// </summary>
    /// <example>tFCyMPURX9pVWXuXQqYY2hJmRcCrhmBPfY</example>
    public Address LiquidityPool { get; set; }
}