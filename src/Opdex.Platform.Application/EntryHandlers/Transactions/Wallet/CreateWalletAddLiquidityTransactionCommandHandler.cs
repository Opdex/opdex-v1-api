using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;

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

        public async Task<string> Handle(CreateWalletAddLiquidityTransactionCommand request, CancellationToken cancellationToken)
        {
            if (!decimal.TryParse(request.AmountCrs, out var amountCrsDecimal))
            {
                throw new Exception("Cannot add liquidity with 0 CRS");
            }

            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPool), cancellationToken);
            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId), cancellationToken);
            var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(pool.MarketId, findOrThrow: true), cancellationToken);

            var amountCrsMin = Math.Round(amountCrsDecimal * request.Tolerance, 8).ToSatoshis(TokenConstants.Cirrus.Decimals);

            var amountSrc = request.AmountSrc.ToSatoshis(srcToken.Decimals);
            var amountSrcMin = amountSrc.ToleranceAsSatoshis(request.Tolerance);

            var command = new MakeWalletAddLiquidityTransactionCommand(request.WalletAddress, srcToken.Address, request.AmountCrs, amountSrc,
                                                                       amountCrsMin.ToString(), amountSrcMin, request.Recipient, router.Address);

            return await _mediator.Send(command, cancellationToken);
        }
    }
}
