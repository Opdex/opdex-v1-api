using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Blocks;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Blocks;

namespace Opdex.Core.Application.Handlers.Blocks
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
        
        public async Task<BlockDto> Handle(RetrieveLatestBlockQuery request, CancellationToken cancellationToken)
        {
            var block = await _mediator.Send(new SelectLatestBlockQuery(), cancellationToken);
            
            return _mapper.Map<BlockDto>(block);
        }
    }
}