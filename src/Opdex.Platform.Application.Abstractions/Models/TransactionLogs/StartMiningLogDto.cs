namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class StartMiningLogDto : TransactionLogDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
    }
}