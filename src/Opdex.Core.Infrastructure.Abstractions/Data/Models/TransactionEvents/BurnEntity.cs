namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class BurnEntity
    {
        public long TransactionId { get; set; }
        public string Sender { get; set; }
        public string To { get; set; }
        public ulong AmountCrs { get; set; }
        public ulong AmountToken { get; set; }
    }
}