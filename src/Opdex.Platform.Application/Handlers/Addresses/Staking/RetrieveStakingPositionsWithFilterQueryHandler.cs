using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Staking
{
    public class RetrieveStakingPositionsWithFilterQueryHandler : IRequestHandler<RetrieveStakingPositionsWithFilterQuery, IEnumerable<AddressStaking>>
    {
        private readonly IMediator _mediator;

        public RetrieveStakingPositionsWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<AddressStaking>> Handle(RetrieveStakingPositionsWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectStakingPositionsWithFilterQuery(request.Address, request.Cursor), cancellationToken);
        }
    }
}
