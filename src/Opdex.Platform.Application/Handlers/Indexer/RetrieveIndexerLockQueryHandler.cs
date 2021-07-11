using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Indexer
{
    public class RetrieveIndexerLockQueryHandler : IRequestHandler<RetrieveIndexerLockQuery, IndexLock>
    {
        private readonly IMediator _mediator;

        public RetrieveIndexerLockQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IndexLock> Handle(RetrieveIndexerLockQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectIndexerLockQuery(), cancellationToken);
        }
    }
}
