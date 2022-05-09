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

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Balances;

public class CreateAddressBalanceCommandHandler : IRequestHandler<CreateAddressBalanceCommand, ulong>
{
    private readonly IMediator _mediator;

    public CreateAddressBalanceCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(CreateAddressBalanceCommand request, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token), cancellationToken);

        // Get an existing address balance or create a new one
        var addressBalance = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(request.Wallet, token.Id, findOrThrow: false), cancellationToken)
                             ?? new AddressBalance(token.Id, request.Wallet, UInt256.Zero, request.Block);

        // Reject making out of date updates to address balances
        if (addressBalance.ModifiedBlock > request.Block)
        {
            return addressBalance.Id;
        }

        // Follow through and upsert the address balance
        return await _mediator.Send(new MakeAddressBalanceCommand(addressBalance, token.Address, request.Block), CancellationToken.None);
    }
}
