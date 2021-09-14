using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Mining;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Mining
{
    public class RetrieveAddressMiningByMiningPoolIdAndOwnerQueryHandler
        : IRequestHandler<RetrieveAddressMiningByMiningPoolIdAndOwnerQuery, AddressMining>
    {
        private readonly IMediator _mediator;

        public RetrieveAddressMiningByMiningPoolIdAndOwnerQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<AddressMining> Handle(RetrieveAddressMiningByMiningPoolIdAndOwnerQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectAddressMiningByMiningPoolIdAndOwnerQuery(request.MiningPoolId, request.Owner, request.FindOrThrow);

            return _mediator.Send(query, cancellationToken);
        }
    }
}
