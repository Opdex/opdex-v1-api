using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class RetrieveActiveTokenSnapshotsByTokenIdQueryHandler : IRequestHandler<RetrieveActiveTokenSnapshotsByTokenIdQuery, IEnumerable<TokenSnapshot>>
    {
        private readonly IMediator _mediator;
        
        public RetrieveActiveTokenSnapshotsByTokenIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<IEnumerable<TokenSnapshot>> Handle(RetrieveActiveTokenSnapshotsByTokenIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectActiveTokenSnapshotsByTokenIdQuery(request.TokenId, request.Time), cancellationToken);
        }
    }
}