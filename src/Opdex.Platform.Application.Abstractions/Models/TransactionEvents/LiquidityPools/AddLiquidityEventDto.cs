using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;

public class AddLiquidityEventDto : ProvideEventDto
{
    public override TransactionEventType EventType => TransactionEventType.AddLiquidityEvent;
}