namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions
{
    public class TransactionEntity : AuditEntity
    {
        public long Id { get; set; }
        public string Hash { get; set; }
        public int GasUsed { get; set; }
        public string From { get; set; }
        public ulong Block { get; set; }
        public string To { get; set; }
    }
}