namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class ReservesLogDto : TransactionLogDto
    {
        public ulong ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
    }
}