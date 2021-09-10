using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

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
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token.ToString(), findOrThrow: false), cancellationToken);

            var amount = request.Amount.ToSatoshis(token.Decimals);

            var command = new MakeWalletApproveAllowanceTransactionCommand(request.WalletAddress, request.Token, amount, request.Spender);

            return await _mediator.Send(command, cancellationToken);
        }
    }
}
