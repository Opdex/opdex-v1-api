namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.MiningPools
{
    public class StopMiningLogDto : TransactionLogDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
    }
}