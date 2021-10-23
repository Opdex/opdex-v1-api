using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
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
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(request.QuoteRequest), cancellationToken);
        }
    }
}
