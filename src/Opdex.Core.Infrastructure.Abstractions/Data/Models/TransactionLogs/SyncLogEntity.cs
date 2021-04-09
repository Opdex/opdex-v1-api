namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs
{
    public class ReservesLogEntity : LogEntityBase
    {
        public ulong ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
    }
}