using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletSyncTransactionCommandHandler : IRequestHandler<CreateWalletSyncTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        
        public CreateWalletSyncTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletSyncTransactionCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeWalletSyncTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.LiquidityPool), cancellationToken);
        }
    }
}