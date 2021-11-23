using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    public class CollectMiningRewardsEvent : TransactionEvent
    {
        public Address Miner { get; set; }
        public FixedDecimal Amount { get; set; }
    }
}
