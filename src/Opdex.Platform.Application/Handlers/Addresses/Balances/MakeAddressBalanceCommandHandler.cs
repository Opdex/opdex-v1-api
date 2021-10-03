using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Balances
{
    public class MakeAddressBalanceCommandHandler : IRequestHandler<MakeAddressBalanceCommand, ulong>
    {
        private readonly IMediator _mediator;

        public MakeAddressBalanceCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ulong> Handle(MakeAddressBalanceCommand request, CancellationToken cancellationToken)
        {
            var balance = await _mediator.Send(new CallCirrusGetSrcTokenBalanceQuery(request.Token, request.AddressBalance.Owner, request.BlockHeight));

            request.AddressBalance.SetBalance(balance, request.BlockHeight);

            return await _mediator.Send(new PersistAddressBalanceCommand(request.AddressBalance));
        }
    }
}
