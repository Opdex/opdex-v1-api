using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Balances;

public class RetrieveAddressBalanceByOwnerAndTokenQueryHandler
    : IRequestHandler<RetrieveAddressBalanceByOwnerAndTokenQuery, AddressBalance>
{
    private readonly IMediator _mediator;

    public RetrieveAddressBalanceByOwnerAndTokenQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<AddressBalance> Handle(RetrieveAddressBalanceByOwnerAndTokenQuery request, CancellationToken cancellationToken)
    {
        var token = request.TokenAddress != Address.Empty
            ? await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenAddress, request.FindOrThrow), cancellationToken)
            : await _mediator.Send(new RetrieveTokenByIdQuery(request.TokenId.GetValueOrDefault(), request.FindOrThrow), cancellationToken);

        if (token == null)
        {
            return null;
        }

        if (token.Address != Address.Cirrus)
        {
            return await _mediator.Send(new SelectAddressBalanceByOwnerAndTokenIdQuery(request.Owner,
                                                                                       token.Id,
                                                                                       request.FindOrThrow), cancellationToken);
        }

        var balance = await _mediator.Send(new CallCirrusGetAddressCrsBalanceQuery(request.Owner, request.FindOrThrow), cancellationToken);

        return new AddressBalance(token.Id, request.Owner, balance, 1);
    }
}