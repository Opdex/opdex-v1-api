using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class RetrieveAddressAllowancesByOwnerWithFilterQueryHandler
        : IRequestHandler<RetrieveAddressAllowancesByOwnerWithFilterQuery, IEnumerable<AddressAllowance>>
    {
        private readonly IMediator _mediator;

        public RetrieveAddressAllowancesByOwnerWithFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<IEnumerable<AddressAllowance>> Handle(RetrieveAddressAllowancesByOwnerWithFilterQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectAddressAllowancesByOwnerWithFilterQuery(request.Owner, request.Spender, request.TokenId), cancellationToken);
        }
    }
}
