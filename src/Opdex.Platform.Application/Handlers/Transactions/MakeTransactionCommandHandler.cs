using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions;

namespace Opdex.Platform.Application.Handlers.Transactions
{
    public class MakeTransactionCommandHandler : IRequestHandler<MakeTransactionCommand, ulong>
    {
        private readonly IMediator _mediator;
        public MakeTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<ulong> Handle(MakeTransactionCommand request, CancellationToken cancellationToken)
        {
            var transactionCommand = new PersistTransactionCommand(request.Transaction);

            return _mediator.Send(transactionCommand, CancellationToken.None);
        }
    }
}
