using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools
{
    public class CollectMiningRewardsLogDto : TransactionEventDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
    }
}
