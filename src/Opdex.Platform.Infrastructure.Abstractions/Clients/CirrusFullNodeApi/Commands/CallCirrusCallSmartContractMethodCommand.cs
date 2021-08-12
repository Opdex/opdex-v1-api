using MediatR;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands
{
    public class CallCirrusCallSmartContractMethodCommand : IRequest<string>
    {
        public CallCirrusCallSmartContractMethodCommand(TransactionQuoteRequest quoteRequest = null, SmartContractCallRequestDto callDto = null)
        {
            // Todo: This is a temporary work around while we keep the API backward compatible
            // Once all quote endpoints are finished and can use the transactions/broadcast-quote remove CallDto from here
            // both are null or both are populated, throw
            if (!(quoteRequest == null ^ callDto == null))
            {
                throw new ArgumentNullException(nameof(quoteRequest), "Call Dto or quote request must be provided.");
            }

            QuoteRequest = quoteRequest;
            CallDto = callDto;
        }

        public TransactionQuoteRequest QuoteRequest { get; }
        public SmartContractCallRequestDto CallDto { get; }
    }
}
