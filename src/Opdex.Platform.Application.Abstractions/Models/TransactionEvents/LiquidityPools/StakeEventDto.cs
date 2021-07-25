using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public class StakeEventDto : TransactionEventDto
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public string SubEventType { get; set; }
        public override TransactionEventType EventType => TransactionEventType.StakeEvent;
    }
}
