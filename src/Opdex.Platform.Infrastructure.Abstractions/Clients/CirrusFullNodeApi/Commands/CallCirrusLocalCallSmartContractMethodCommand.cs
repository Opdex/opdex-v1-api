using MediatR;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;

/// <summary>
/// Generic command to call a smart contract, without broadcasting the transaction. Used to produce a transaction quote.
/// </summary>
public class CallCirrusLocalCallSmartContractMethodCommand : IRequest<TransactionQuote>
{
    /// <summary>
    /// Creates a command to call a smart contract, without broadcasting the transaction.
    /// </summary>
    /// <param name="quoteRequest">The transaction quote parameters.</param>
    public CallCirrusLocalCallSmartContractMethodCommand(TransactionQuoteRequest quoteRequest)
    {
        QuoteRequest = quoteRequest ?? throw new ArgumentNullException(nameof(quoteRequest), "Quote request must be provided.");
    }

    public TransactionQuoteRequest QuoteRequest { get; }
}