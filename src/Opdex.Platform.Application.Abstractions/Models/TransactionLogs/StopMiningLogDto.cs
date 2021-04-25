namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class StopMiningLogDto : TransactionLogDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
    }
}