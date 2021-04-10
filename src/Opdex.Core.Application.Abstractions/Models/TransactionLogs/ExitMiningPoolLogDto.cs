namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class ExitMiningPoolLogDto : TransactionLogDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
    }
}