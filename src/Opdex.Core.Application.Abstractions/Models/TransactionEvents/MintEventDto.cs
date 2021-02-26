namespace Opdex.Core.Application.Abstractions.Models.TransactionEvents
{
    public class MintEventDto : TransactionEventDto
    {
        public string Sender { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}