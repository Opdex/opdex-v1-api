using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletAddLiquidityTransactionCommandHandler 
        : IRequestHandler<CreateWalletAddLiquidityTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletAddLiquidityTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletAddLiquidityTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var command = new MakeWalletAddLiquidityTransactionCommand(request.Token, request.AmountCrsDesired, request.AmountSrcDesired,
                request.AmountCrsMin, request.AmountSrcMin, request.To, request.Market);
            
            return _mediator.Send(command, cancellationToken);
        }
    }
}