namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class BurnEventEntity : EventEntityBase
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}