using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Pairs;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pairs;

namespace Opdex.Core.Application.Handlers.Pairs
{
    public class RetrievePairByAddressQueryHandler : IRequestHandler<RetrievePairByAddressQuery, Pair>
    {
        private readonly IMediator _mediator;
        
        public RetrievePairByAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<Pair> Handle(RetrievePairByAddressQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new SelectPairByAddressQuery(request.Address), cancellationToken);

            return token;
        }
    }
}