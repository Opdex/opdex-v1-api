using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets
{
    public class CreateLiquidityPoolEventDto : TransactionEventDto
    {
        public Address Token { get; set; }
        public Address LiquidityPool { get; set; }
        public override TransactionEventType EventType => TransactionEventType.CreateLiquidityPoolEvent;
    }
}
