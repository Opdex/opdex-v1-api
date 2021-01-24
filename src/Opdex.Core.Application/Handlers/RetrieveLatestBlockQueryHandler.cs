using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries;

namespace Opdex.Core.Application.Handlers
{
    public class RetrieveLatestBlockQueryHandler : IRequestHandler<RetrieveLatestBlockQuery, Block>
    {
        private readonly IMediator _mediator;
        
        public RetrieveLatestBlockQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<Block> Handle(RetrieveLatestBlockQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLatestBlockQuery(), cancellationToken);
        }
    }
}