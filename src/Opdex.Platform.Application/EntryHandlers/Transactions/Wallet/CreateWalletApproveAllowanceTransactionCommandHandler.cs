using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;

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

        public async Task<string> Handle(CreateWalletApproveAllowanceTransactionCommand request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token), cancellationToken);

            var amount = request.Amount.ToSatoshis(token.Decimals);
            
            var command = new MakeWalletApproveAllowanceTransactionCommand(request.Token, amount, request.Owner, request.Spender);
            
            return await _mediator.Send(command, cancellationToken);
        }
    }
}