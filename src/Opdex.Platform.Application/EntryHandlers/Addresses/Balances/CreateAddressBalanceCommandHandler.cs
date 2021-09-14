using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Balances
{
    public class CreateAddressBalanceCommandHandler : IRequestHandler<CreateAddressBalanceCommand, long>
    {
        private readonly IMediator _mediator;

        public CreateAddressBalanceCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(CreateAddressBalanceCommand request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token), cancellationToken);

            var addressBalance = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(request.Wallet, token.Id, findOrThrow: false))
                                 ?? new AddressBalance(token.Id, request.Wallet, UInt256.Zero, request.Block);

            return await _mediator.Send(new MakeAddressBalanceCommand(addressBalance, token.Address, request.Block), CancellationToken.None);
        }
    }
}
