using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningGovernances
{
    public class NominationEventDto : TransactionEventDto
    {
        public Address StakingPool { get; set; }
        public Address MiningPool { get; set; }
        public FixedDecimal Weight { get; set; }
        public override TransactionEventType EventType => TransactionEventType.NominationEvent;
    }
}
