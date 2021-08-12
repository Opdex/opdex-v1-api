using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletStartMiningTransactionCommandHandler : IRequestHandler<CreateWalletStartMiningTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletStartMiningTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(CreateWalletStartMiningTransactionCommand request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPool, findOrThrow: true), cancellationToken);

            var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id, findOrThrow: true), cancellationToken);

            var amount = request.Amount.ToSatoshis(TokenConstants.LiquidityPoolToken.Decimals);

            return await _mediator.Send(new MakeWalletStartMiningTransactionCommand(request.WalletAddress, amount, miningPool.Address), cancellationToken);
        }
    }
}
