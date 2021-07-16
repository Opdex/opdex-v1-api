using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools
{
    public class EnableMiningLogDto : TransactionEventDto
    {
        public string Amount { get; set; }
        public string RewardRate { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
    }
}
