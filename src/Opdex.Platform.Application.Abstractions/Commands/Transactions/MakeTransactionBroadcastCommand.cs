using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions
{
    public class MakeTransactionBroadcastCommand : IRequest<Sha256>
    {
        public MakeTransactionBroadcastCommand(TransactionQuoteRequest transactionQuote)
        {
            QuoteRequest = transactionQuote ?? throw new ArgumentNullException(nameof(transactionQuote));
        }

        public TransactionQuoteRequest QuoteRequest { get; }
    }
}
