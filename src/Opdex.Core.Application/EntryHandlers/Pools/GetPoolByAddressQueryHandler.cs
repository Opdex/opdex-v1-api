using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.EntryQueries.Pools;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Pools;

namespace Opdex.Core.Application.EntryHandlers.Pools
{
    public class GetPoolByAddressQueryHandler: IRequestHandler<GetPoolByAddressQuery, PoolDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public GetPoolByAddressQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<PoolDto> Handle(GetPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrievePoolByAddressQuery(request.Address), cancellationToken);

            return _mapper.Map<PoolDto>(token);
        }
    }
}