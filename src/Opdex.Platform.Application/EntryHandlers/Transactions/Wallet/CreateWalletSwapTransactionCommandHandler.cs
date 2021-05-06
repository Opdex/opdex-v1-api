using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletSwapTransactionCommandHandler : IRequestHandler<CreateWalletSwapTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        
        public CreateWalletSwapTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<string> Handle(CreateWalletSwapTransactionCommand request, CancellationToken cancellationToken)
        {
            var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn), cancellationToken);
            var tokenInAmount = request.TokenInAmount.ToSatoshis(tokenIn.Decimals);
            
            var tokenOut = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenOut), cancellationToken);
            var tokenOutAmount = request.TokenOutAmount.ToSatoshis(tokenOut.Decimals);
            
            var command = new MakeWalletSwapTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword, 
                request.TokenIn, request.TokenOut, tokenInAmount, tokenOutAmount, request.TokenInExactAmount, 
                request.Tolerance, request.Recipient, request.Market);
            
            return await _mediator.Send(command, cancellationToken);
        }
    }
}