using System;
using MediatR;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Application.Abstractions.Queries
{
    public class RetrieveCirrusLocalCallSmartContractQuery : IRequest<TransactionQuote>
    {
        public RetrieveCirrusLocalCallSmartContractQuery(TransactionQuoteRequest quoteRequest)
        {
            QuoteRequest = quoteRequest ?? throw new ArgumentNullException(nameof(quoteRequest));
        }

        public TransactionQuoteRequest QuoteRequest { get; }
    }
}
