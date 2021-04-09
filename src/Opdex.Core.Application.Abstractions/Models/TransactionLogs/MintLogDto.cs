namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class MintLogDto : TransactionLogDto
    {
        public string Sender { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}