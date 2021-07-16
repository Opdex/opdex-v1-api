using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools
{
    public class MineLogDto : TransactionEventDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
        public string TotalSupply { get; set; }
        public string EventType { get; set; }
    }
}
