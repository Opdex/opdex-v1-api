using System;
using MediatR;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    public class CallCirrusLocalCallSmartContractMethodQuery : IRequest<TransactionQuote>
    {
        public CallCirrusLocalCallSmartContractMethodQuery(TransactionQuoteRequest quoteRequest)
        {
            QuoteRequest = quoteRequest ?? throw new ArgumentNullException(nameof(quoteRequest));
        }

        public TransactionQuoteRequest QuoteRequest { get; }
    }
}
