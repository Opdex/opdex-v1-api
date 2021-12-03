namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models.Transactions;

public class TransactionLogEventDto
{
    public string Event { get; set; }
    public object Data { get; set; }
}