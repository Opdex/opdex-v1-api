namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions
{
    // Todo: Consider removing "Block" and using AuditEntity for Created/Modified Block
    public class TransactionEntity
    {
        public long Id { get; set; }
        public string Hash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string NewContractAddress { get; set; }
        public bool Success { get; set; }
        public int GasUsed { get; set; }
        public ulong Block { get; set; }
    }
}