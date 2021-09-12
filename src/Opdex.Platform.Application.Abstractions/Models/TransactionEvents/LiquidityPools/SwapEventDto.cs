using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public class SwapEventDto : TransactionEventDto
    {
        public Address Sender { get; set; }
        public Address To { get; set; }
        public FixedDecimal AmountCrsIn { get; set; }
        public FixedDecimal AmountSrcIn { get; set; }
        public FixedDecimal AmountCrsOut { get; set; }
        public FixedDecimal AmountSrcOut { get; set; }
        public Address SrcToken { get; set; }
        public override TransactionEventType EventType => TransactionEventType.SwapEvent;
    }
}
