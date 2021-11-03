using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Transactions
{
    public class MakeTransactionBroadcastCommandHandler : IRequestHandler<MakeTransactionBroadcastCommand, Sha256>
    {
        private readonly IMediator _mediator;

        public MakeTransactionBroadcastCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<Sha256> Handle(MakeTransactionBroadcastCommand request, CancellationToken cancellationToken)
        {
            var call = new SmartContractCallRequestDto(request.QuoteRequest.To, "cirrusdev", request.QuoteRequest.Sender, "password",
                                                       request.QuoteRequest.Amount, request.QuoteRequest.Method, request.QuoteRequest.MethodParameters);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(call), cancellationToken);
        }
    }
}
