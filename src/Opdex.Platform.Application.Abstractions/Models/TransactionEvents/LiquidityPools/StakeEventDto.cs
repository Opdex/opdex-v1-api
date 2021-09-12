using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public abstract class StakeEventDto : TransactionEventDto
    {
        public Address Staker { get; set; }
        public FixedDecimal Amount { get; set; }
        public FixedDecimal TotalStaked { get; set; }
        public FixedDecimal StakerBalance { get; set; }
    }
}
