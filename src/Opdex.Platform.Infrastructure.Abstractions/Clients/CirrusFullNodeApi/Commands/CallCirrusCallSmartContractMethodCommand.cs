using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands
{
    public class CallCirrusCallSmartContractMethodCommand : IRequest<Sha256>
    {
        public CallCirrusCallSmartContractMethodCommand(TransactionQuoteRequest quoteRequest = null, SmartContractCallRequestDto callDto = null)
        {
            // CallDto should not be used going forward, is only used currently in the deploy controller
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
