namespace Opdex.Core.Application.Abstractions.Models.TransactionEvents
{
    public class SyncEventDto : TransactionEventDto
    {
        public ulong ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
    }
}