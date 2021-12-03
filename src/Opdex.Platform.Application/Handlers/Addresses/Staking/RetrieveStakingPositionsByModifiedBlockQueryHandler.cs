using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Staking;

public class RetrieveStakingPositionsByModifiedBlockQueryHandler : IRequestHandler<RetrieveStakingPositionsByModifiedBlockQuery, IEnumerable<AddressStaking>>
{
    private readonly IMediator _mediator;

    public RetrieveStakingPositionsByModifiedBlockQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    public async Task<IEnumerable<AddressStaking>> Handle(RetrieveStakingPositionsByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectStakingPositionsByModifiedBlockQuery(request.BlockHeight), cancellationToken);
    }
}