using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletRewardMiningPoolsTransactionCommandHandler
        : IRequestHandler<CreateWalletRewardMiningPoolsTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletRewardMiningPoolsTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletRewardMiningPoolsTransactionCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeWalletRewardMiningPoolsTransactionCommand(request.WalletAddress, request.Governance), cancellationToken);
        }
    }
}
