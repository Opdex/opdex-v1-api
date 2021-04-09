namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class BurnLogDto : TransactionLogDto
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}