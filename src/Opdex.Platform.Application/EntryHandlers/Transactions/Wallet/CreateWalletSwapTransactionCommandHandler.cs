using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletSwapTransactionCommandHandler : IRequestHandler<CreateWalletSwapTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        
        public CreateWalletSwapTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<string> Handle(CreateWalletSwapTransactionCommand request, CancellationToken cancellationToken)
        {
            var command = new MakeWalletSwapTransactionCommand(request.TokenIn, request.TokenOut, request.TokenInAmount, 
                request.TokenOutAmount, request.TokenInExactAmount, request.Tolerance, request.To);
            
            return _mediator.Send(command, cancellationToken);
        }
    }
}