using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
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

        public async Task<string> Handle(CreateWalletRemoveLiquidityTransactionCommand request, CancellationToken cancellationToken)
        {
            var liquidity = request.Liquidity.ToSatoshis(TokenConstants.LiquidityPoolToken.Decimals);
            var amountCrsMin = request.AmountCrsMin.ToSatoshis(TokenConstants.Cirrus.Decimals);

            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPool), cancellationToken);
            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId), cancellationToken);
            var amountSrcMin = request.AmountSrcMin.ToSatoshis(srcToken.Decimals);

            var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(pool.MarketId, findOrThrow: true), cancellationToken);

            var command = new MakeWalletRemoveLiquidityTransactionCommand(request.WalletAddress, srcToken.Address, liquidity, amountCrsMin,
                                                                          amountSrcMin, request.Recipient, router.Address);

            return await _mediator.Send(command, cancellationToken);
        }
    }
}
