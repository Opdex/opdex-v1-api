using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;

// Todo: Consider removing "Block" and using AuditEntity for Created/Modified Block
public class TransactionEntity
{
    public ulong Id { get; set; }
    public Sha256 Hash { get; set; }
    public Address From { get; set; }
    public Address To { get; set; }
    public Address NewContractAddress { get; set; }
    public bool Success { get; set; }
    public int GasUsed { get; set; }
    public ulong Block { get; set; }
}