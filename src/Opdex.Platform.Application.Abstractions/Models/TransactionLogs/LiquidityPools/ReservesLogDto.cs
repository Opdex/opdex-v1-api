namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.LiquidityPools
{
    public class ReservesLogDto : TransactionLogDto
    {
        public ulong ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
    }
}