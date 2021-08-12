using MediatR;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands
{
    public class CallCirrusLocalCallSmartContractMethodCommand : IRequest<TransactionQuote>
    {
        public CallCirrusLocalCallSmartContractMethodCommand(TransactionQuoteRequest quoteRequest)
        {
            QuoteRequest = quoteRequest ?? throw new ArgumentNullException(nameof(quoteRequest), "Quote request must be provided.");
        }

        public TransactionQuoteRequest QuoteRequest { get; }
    }
}
