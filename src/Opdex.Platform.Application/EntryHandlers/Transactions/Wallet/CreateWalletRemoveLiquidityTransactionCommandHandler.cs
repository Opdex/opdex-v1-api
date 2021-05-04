using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletRemoveLiquidityTransactionCommandHandler 
        : IRequestHandler<CreateWalletRemoveLiquidityTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletRemoveLiquidityTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletRemoveLiquidityTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var command = new MakeWalletRemoveLiquidityTransactionCommand(request.Token, request.Liquidity,
                request.AmountCrsMin, request.AmountSrcMin, request.To, request.Market);
            
            return _mediator.Send(command, cancellationToken);
        }
    }
}