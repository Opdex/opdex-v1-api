using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Blocks
{
    public class RetrieveBlockByMedianTimeQueryHandler : IRequestHandler<RetrieveBlockByMedianTimeQuery, Block>
    {
        private readonly IMediator _mediator;

        public RetrieveBlockByMedianTimeQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<Block> Handle(RetrieveBlockByMedianTimeQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectBlockByMedianTimeQuery(request.MedianTime, request.FindOrThrow);

            return _mediator.Send(query, cancellationToken);
        }
    }
}
