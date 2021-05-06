using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletDistributeTokensTransactionCommandHandler
        : IRequestHandler<CreateWalletDistributeTokensTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        
        public CreateWalletDistributeTokensTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletDistributeTokensTransactionCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeWalletDistributeTokensTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.Token), cancellationToken);
        }
    }
}