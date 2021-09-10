using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Governances
{
    public class RewardMiningPoolEventResponseModel : TransactionEventResponseModel
    {
        public Address StakingPool { get; set; }
        public Address MiningPool { get; set; }
        public FixedDecimal Amount { get; set; }
    }
}
