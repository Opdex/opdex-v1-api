using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class RetrieveLatestTokenSnapshotByTokenIdQueryHandler 
        : IRequestHandler<RetrieveLatestTokenSnapshotByTokenIdQuery, TokenSnapshot >
    {
        private readonly IMediator _mediator;
        
        public RetrieveLatestTokenSnapshotByTokenIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<TokenSnapshot> Handle(RetrieveLatestTokenSnapshotByTokenIdQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLatestTokenSnapshotByTokenIdQuery(request.TokenId), cancellationToken);
        }
    }
}