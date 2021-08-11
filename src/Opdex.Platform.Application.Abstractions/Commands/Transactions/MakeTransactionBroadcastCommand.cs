using MediatR;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions
{
    public class MakeTransactionBroadcastCommand : IRequest<string>
    {
        public MakeTransactionBroadcastCommand(TransactionQuoteRequest transactionQuote)
        {
            QuoteRequest = transactionQuote ?? throw new ArgumentNullException(nameof(transactionQuote));
        }

        public TransactionQuoteRequest QuoteRequest { get; }
    }
}
