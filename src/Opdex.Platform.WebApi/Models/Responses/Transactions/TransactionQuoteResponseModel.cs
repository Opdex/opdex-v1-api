using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions;

/// <summary>
/// Quote for submitting a smart contract transaction.
/// </summary>
public class TransactionQuoteResponseModel : ITransactionResponseModel
{
    public TransactionQuoteResponseModel()
    {
        Events = new List<TransactionEvent>().AsReadOnly();
    }

    /// <summary>
    /// Value returned as part of the quoted transaction.
    /// </summary>
    /// <example>50</example>
    public object Result { get; set; }

    /// <summary>
    /// Any error that occured as part of the quoted transaction.
    /// </summary>
    public TransactionErrorResponseModel Error { get; set; }

    /// <summary>
    /// Total amount of gas consumed.
    /// </summary>
    /// <example>50000</example>
    public uint GasUsed { get; set; }

    /// <summary>
    /// Events that occured in the quoted transaction.
    /// </summary>
    public IReadOnlyCollection<TransactionEvent> Events { get; set; }

    /// <summary>
    /// Transaction request which can be used to replay or broadcast the transaction.
    /// </summary>
    public QuotedTransactionModel Request { get; set; }
}
