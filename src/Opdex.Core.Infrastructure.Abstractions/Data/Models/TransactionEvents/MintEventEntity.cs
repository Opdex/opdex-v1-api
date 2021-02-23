namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class MintEventEntity : EventEntityBase
    {
        public string Sender { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}