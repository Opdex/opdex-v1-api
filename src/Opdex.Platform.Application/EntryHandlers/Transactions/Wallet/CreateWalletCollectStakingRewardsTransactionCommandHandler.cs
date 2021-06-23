using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletCollectStakingRewardsTransactionCommandHandler
        : IRequestHandler<CreateWalletCollectStakingRewardsTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletCollectStakingRewardsTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletCollectStakingRewardsTransactionCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeWalletCollectStakingRewardsTransactionCommand(request.WalletAddress, request.LiquidityPool, request.Liquidate), cancellationToken);
        }
    }
}
