using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Queries.Pools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletCollectMiningRewardsTransactionCommandHandler : IRequestHandler<CreateWalletCollectMiningRewardsTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletCollectMiningRewardsTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(CreateWalletCollectMiningRewardsTransactionCommand request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPool), cancellationToken);

            var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id), cancellationToken);

            return await _mediator.Send(new MakeWalletCollectMiningRewardsTransactionCommand(request.WalletAddress, miningPool.Address), cancellationToken);
        }
    }
}
