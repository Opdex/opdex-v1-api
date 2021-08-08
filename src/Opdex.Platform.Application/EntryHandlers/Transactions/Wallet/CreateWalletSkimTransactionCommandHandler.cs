using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletSkimTransactionCommandHandler : IRequestHandler<CreateWalletSkimTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletSkimTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletSkimTransactionCommand request, CancellationToken cancellationToken)
        {
            var command = new MakeWalletSkimTransactionCommand(request.WalletAddress, request.LiquidityPool, request.Recipient);

            return _mediator.Send(command, cancellationToken);
        }
    }
}
