using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    public abstract class ProvideEvent : TransactionEvent
    {
        public FixedDecimal AmountCrs { get; set; }
        public FixedDecimal AmountSrc { get; set; }
        public FixedDecimal AmountLpt { get; set; }
        public Address TokenSrc { get; set; }
        public Address TokenLp { get; set; }
        public FixedDecimal TokenLpTotalSupply { get; set; }
    }
}
