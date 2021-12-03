using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;

public class TransactionLogEntity
{
    public ulong Id { get; set; }
    public ulong TransactionId { get; set; }
    public int LogTypeId { get; set; }
    public Address Contract { get; set; }
    public int SortOrder { get; set; }
    public string Details { get; set; }
}