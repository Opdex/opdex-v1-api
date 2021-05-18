namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Markets
{
    public class CreateLiquidityPoolLogDto : TransactionLogDto
    {
        public string Token { get; set; }
        public string Pool { get; set; }
    }
}