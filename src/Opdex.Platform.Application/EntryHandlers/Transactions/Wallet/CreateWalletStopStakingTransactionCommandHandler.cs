using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletStopStakingTransactionCommandHandler : IRequestHandler<CreateWalletStopStakingTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        
        public CreateWalletStopStakingTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletStopStakingTransactionCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeWalletStopStakingTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.Recipient, request.LiquidityPool, request.Liquidate), cancellationToken);
        }
    }
}