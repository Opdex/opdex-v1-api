using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;

public class StartMiningEventDto : MineEventDto
{
    public override TransactionEventType EventType => TransactionEventType.StartMiningEvent;
}