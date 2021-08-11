using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;

namespace Opdex.Platform.Application.Handlers
{
    public class MakeTransactionQuoteCommandHandler : IRequestHandler<MakeTransactionQuoteCommand, TransactionQuote>
    {
        private readonly IMediator _mediator;

        public MakeTransactionQuoteCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<TransactionQuote> Handle(MakeTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusLocalCallSmartContractMethodCommand(request.QuoteRequest), cancellationToken);
        }
    }
}
