using MediatR;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands
{
    public class MakeTransactionQuoteCommand : IRequest<TransactionQuote>
    {
        public MakeTransactionQuoteCommand(TransactionQuoteRequest quoteRequest)
        {
            QuoteRequest = quoteRequest ?? throw new ArgumentNullException(nameof(quoteRequest));
        }

        public TransactionQuoteRequest QuoteRequest { get; }
    }
}
