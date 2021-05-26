namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.MiningPools
{
    public class StartMiningLogDto : TransactionLogDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
    }
}