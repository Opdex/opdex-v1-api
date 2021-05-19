using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;

namespace Opdex.Platform.Application.Handlers.Blocks
{
    public class RetrieveLatestBlockQueryHandler : IRequestHandler<RetrieveLatestBlockQuery, BlockDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public RetrieveLatestBlockQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        // Todo: Should return Block domain object out
        public async Task<BlockDto> Handle(RetrieveLatestBlockQuery request, CancellationToken cancellationToken)
        {
            var block = await _mediator.Send(new SelectLatestBlockQuery(request.FindOrThrow), cancellationToken);
            
            return _mapper.Map<BlockDto>(block);
        }
    }
}