using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class CreateNotifyUserOfTransactionBroadcastCommandHandler : IRequestHandler<CreateNotifyUserOfTransactionBroadcastCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateNotifyUserOfTransactionBroadcastCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateNotifyUserOfTransactionBroadcastCommand request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new MakeNotifyUserOfTransactionBroadcastCommand(request.User, request.TransactionHash), cancellationToken);
        }
    }
}
