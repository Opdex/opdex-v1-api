using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    public class EnableMiningEvent : TransactionEvent
    {
        public FixedDecimal Amount { get; set; }
        public FixedDecimal RewardRate { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
    }
}
