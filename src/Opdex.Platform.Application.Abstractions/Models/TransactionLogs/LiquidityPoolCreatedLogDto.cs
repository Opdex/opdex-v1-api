namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class LiquidityPoolCreatedLogDto : TransactionLogDto
    {
        public string Token { get; set; }
        public string Pool { get; set; }
    }
}