namespace Opdex.Core.Application.Abstractions.Models.TransactionEvents
{
    public class BurnEventDto : TransactionEventDto
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}