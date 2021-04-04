using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletCreateLiquidityPoolTransactionCommandHandler 
        : IRequestHandler<CreateWalletCreateLiquidityPoolTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletCreateLiquidityPoolTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletCreateLiquidityPoolTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var command = new MakeWalletCreateLiquidityPoolTransactionCommand(request.Token, request.Sender);
            
            return _mediator.Send(command, cancellationToken);
        }
    }
}