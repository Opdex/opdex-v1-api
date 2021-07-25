using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers
{
    public abstract class SelectFilterQueryHandler<T, TK> : IRequestHandler<T, TK> where T : SelectFilterQuery<TK>
    {
        protected readonly IMediator _mediator;

        protected SelectFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public abstract Task<TK> Handle(T request, CancellationToken cancellationToken);
    }
}
