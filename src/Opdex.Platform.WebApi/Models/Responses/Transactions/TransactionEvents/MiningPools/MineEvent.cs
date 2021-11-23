using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    public abstract class MineEvent : TransactionEvent
    {
        public Address Miner { get; set; }
        public FixedDecimal Amount { get; set; }
        public FixedDecimal TotalSupply { get; set; }
        public FixedDecimal MinerBalance { get; set; }
    }
}
