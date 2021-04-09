namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs
{
    public class MintLogEntity : LogEntityBase
    {
        public string Sender { get; set; }
        public ulong AmountCrs { get; set; }
        public string AmountSrc { get; set; }
    }
}