namespace Opdex.Core.Infrastructure.Abstractions.Models
{
    public class TransactionEntity
    {
        public string From { get; set; }
        public ulong Block { get; set; }
        public string TxHash { get; set; }
    }
}