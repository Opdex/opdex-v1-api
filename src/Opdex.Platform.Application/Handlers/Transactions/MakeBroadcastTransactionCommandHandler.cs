using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Transactions
{
    public class MakeBroadcastTransactionCommandHandler : IRequestHandler<MakeBroadcastTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public MakeBroadcastTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeBroadcastTransactionCommand request, CancellationToken cancellationToken)
        {
            var parameters = request.QuoteRequest.Parameters.Select(p => p.Serialized).ToArray();
            var callRequest = new SmartContractCallRequestDto(request.QuoteRequest.To, "cirrusdev", request.QuoteRequest.Sender,
                                                              "password", request.QuoteRequest.Amount, request.QuoteRequest.Method, parameters);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callRequest), cancellationToken);
        }
    }
}
