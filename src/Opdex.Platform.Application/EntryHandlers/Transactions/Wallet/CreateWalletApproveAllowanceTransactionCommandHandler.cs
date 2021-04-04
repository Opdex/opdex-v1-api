using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletApproveAllowanceTransactionCommandHandler 
        : IRequestHandler<CreateWalletApproveAllowanceTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletApproveAllowanceTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletApproveAllowanceTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var command = new MakeWalletApproveAllowanceTransactionCommand(request.Token, request.Amount, request.Owner, request.Spender);
            
            return _mediator.Send(command, cancellationToken);
        }
    }
}