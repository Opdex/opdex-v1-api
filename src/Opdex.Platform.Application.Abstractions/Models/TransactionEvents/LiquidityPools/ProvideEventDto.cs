using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public abstract class ProvideEventDto : TransactionEventDto
    {
        public FixedDecimal AmountCrs { get; set; }
        public FixedDecimal AmountSrc { get; set; }
        public FixedDecimal AmountLpt { get; set; }
        public Address TokenSrc { get; set; }
        public Address TokenLp { get; set; }
        public FixedDecimal TokenLpTotalSupply { get; set; }
    }
}
