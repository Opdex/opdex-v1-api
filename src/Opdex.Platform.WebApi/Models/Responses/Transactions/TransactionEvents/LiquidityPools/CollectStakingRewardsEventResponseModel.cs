using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    public class CollectStakingRewardsEventResponseModel : TransactionEventResponseModel
    {
        public Address Staker { get; set; }
        public FixedDecimal Amount { get; set; }
    }
}
