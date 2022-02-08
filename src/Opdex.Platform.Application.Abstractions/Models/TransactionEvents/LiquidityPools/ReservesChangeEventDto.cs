using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;

public class ReservesChangeEventDto : TransactionEventDto
{
    public FixedDecimal Crs { get; set; }
    public FixedDecimal Src { get; set; }
    public override TransactionEventType EventType => TransactionEventType.ReservesChangeEvent;
}
