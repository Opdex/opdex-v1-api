using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;

namespace Opdex.Platform.Application.Handlers.Blocks
{
    public class RetrieveLatestBlockQueryHandler : IRequestHandler<RetrieveLatestBlockQuery, Block>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RetrieveLatestBlockQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<Block> Handle(RetrieveLatestBlockQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectLatestBlockQuery(request.FindOrThrow), cancellationToken);
        }
    }
}
