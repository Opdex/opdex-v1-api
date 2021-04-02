using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Pools;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Core.Application.Handlers.Pools
{
    public class RetrievePoolByAddressQueryHandler : IRequestHandler<RetrievePoolByAddressQuery, Pool>
    {
        private readonly IMediator _mediator;
        
        public RetrievePoolByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<Pool> Handle(RetrievePoolByAddressQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new SelectPoolByAddressQuery(request.Address), cancellationToken);

            return token;
        }
    }
}