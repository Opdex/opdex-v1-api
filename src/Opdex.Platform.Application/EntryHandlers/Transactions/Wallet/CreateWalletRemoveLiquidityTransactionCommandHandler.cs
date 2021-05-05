using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;

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

        public async Task<string> Handle(CreateWalletRemoveLiquidityTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var liquidity = request.Liquidity.ToSatoshis(TokenConstants.LiquidityPoolToken.Decimals);
            var amountCrsMin = request.AmountCrsMin.ToSatoshis(TokenConstants.Cirrus.Decimals);

            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token), cancellationToken);
            var amountSrcMin = request.AmountSrcMin.ToSatoshis(token.Decimals);
            
            var command = new MakeWalletRemoveLiquidityTransactionCommand(request.Token, liquidity, amountCrsMin, amountSrcMin, request.To, request.Market);
            
            return await _mediator.Send(command, cancellationToken);
        }
    }
}