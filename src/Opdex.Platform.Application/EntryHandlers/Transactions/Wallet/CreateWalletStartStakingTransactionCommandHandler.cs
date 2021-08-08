using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletStartStakingTransactionCommandHandler : IRequestHandler<CreateWalletStartStakingTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletStartStakingTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateWalletStartStakingTransactionCommand request, CancellationToken cancellationToken)
        {
            var amount = request.Amount.ToSatoshis(TokenConstants.Opdex.Decimals);

            return _mediator.Send(new MakeWalletStartStakingTransactionCommand(request.WalletAddress, amount, request.LiquidityPool), cancellationToken);
        }
    }
}
