using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;

public class RemoveLiquidityEventDto : ProvideEventDto
{
    public override TransactionEventType EventType => TransactionEventType.RemoveLiquidityEvent;
}