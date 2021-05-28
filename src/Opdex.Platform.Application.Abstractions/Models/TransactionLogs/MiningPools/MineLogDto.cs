namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.MiningPools
{
    public class MineLogDto : TransactionLogDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
        public string TotalSupply { get; set; }
        public byte EventType { get; set; }
    }
}