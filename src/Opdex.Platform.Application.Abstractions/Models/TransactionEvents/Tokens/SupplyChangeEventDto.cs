using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;

public class SupplyChangeEventDto : TransactionEventDto
{
    public FixedDecimal PreviousSupply { get; set; }
    public FixedDecimal TotalSupply { get; set; }
    public override TransactionEventType EventType => TransactionEventType.SupplyChangeEvent;
}
