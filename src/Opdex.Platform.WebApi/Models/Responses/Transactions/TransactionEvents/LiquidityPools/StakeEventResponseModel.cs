using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    public abstract class StakeEventResponseModel : TransactionEventResponseModel
    {
        public Address Staker { get; set; }
        public FixedDecimal Amount { get; set; }
        public FixedDecimal StakerBalance { get; set; }
        public FixedDecimal TotalStaked { get; set; }
    }
}
