using MediatR;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions;

public class MakeTransactionQuoteCommand : IRequest<TransactionQuote>
{
    public MakeTransactionQuoteCommand(TransactionQuoteRequest quoteRequest)
    {
        QuoteRequest = quoteRequest ?? throw new ArgumentNullException(nameof(quoteRequest), "Transaction quote request must be provided.");
    }

    public TransactionQuoteRequest QuoteRequest { get; }
}