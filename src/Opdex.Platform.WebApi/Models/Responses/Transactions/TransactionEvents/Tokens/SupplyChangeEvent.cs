using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens;

/// <summary>
/// Interflux supply change event.
/// </summary>
public class SupplyChangeEvent : TransactionEvent
{
    /// <summary>
    /// Old supply of the token
    /// </summary>
    /// <example>"1000.00000000"</example>
    public FixedDecimal Previous { get; set; }

    /// <summary>
    /// New supply of the token
    /// </summary>
    /// <example>"50000.00000000"</example>
    public FixedDecimal Updated { get; set; }
}
