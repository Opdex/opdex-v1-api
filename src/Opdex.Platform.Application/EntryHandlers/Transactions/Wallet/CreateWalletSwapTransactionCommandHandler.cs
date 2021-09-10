using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
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
            var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn.ToString(), findOrThrow: true), cancellationToken);
            var tokenInAmount = request.TokenInAmount.ToSatoshis(tokenIn.Decimals);

            var tokenOut = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenOut.ToString(), findOrThrow: true), cancellationToken);
            var tokenOutAmount = request.TokenOutAmount.ToSatoshis(tokenOut.Decimals);

            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);
            var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(market.Id), cancellationToken);

            var command = new MakeWalletSwapTransactionCommand(request.WalletAddress, request.TokenIn.ToString(), request.TokenOut.ToString(), tokenInAmount,
                                                               tokenOutAmount, request.TokenInExactAmount, request.Tolerance, request.Recipient,
                                                               router.Address);

            return await _mediator.Send(command, cancellationToken);
        }
    }
}
