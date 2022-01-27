namespace Opdex.Platform.WebApi.Models.Requests.Transactions;

/// <summary>
/// Request to replay a quoted transaction.
/// </summary>
public class QuoteReplayRequest
{
    /// <summary>
    /// Request for a quoted transaction.
    /// </summary>
    public QuotedTransactionModel Request { get; set; }
}
