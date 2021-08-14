using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningPools
{
    public class RetrieveMiningPoolByIdQueryHandler : IRequestHandler<RetrieveMiningPoolByIdQuery, MiningPool>
    {
        private readonly IMediator _mediator;

        public RetrieveMiningPoolByIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public Task<MiningPool> Handle(RetrieveMiningPoolByIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectMiningPoolByIdQuery(request.MiningPoolId, request.FindOrThrow), cancellationToken);
        }
    }
}
