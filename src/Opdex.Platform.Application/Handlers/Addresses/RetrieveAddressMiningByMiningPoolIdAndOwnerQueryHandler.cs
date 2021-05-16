using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;

namespace Opdex.Platform.Application.Handlers.Addresses
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
            return _mediator.Send(new SelectAddressMiningByMiningPoolIdAndOwnerQuery(request.MiningPoolId, request.Owner), cancellationToken);
        }
    }
}