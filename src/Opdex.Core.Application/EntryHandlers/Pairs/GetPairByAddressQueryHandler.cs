using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.EntryQueries.Pairs;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Pairs;

namespace Opdex.Core.Application.EntryHandlers.Pairs
{
    public class GetPairByAddressQueryHandler: IRequestHandler<GetPairByAddressQuery, PairDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public GetPairByAddressQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<PairDto> Handle(GetPairByAddressQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrievePairByAddressQuery(request.Address), cancellationToken);

            return _mapper.Map<PairDto>(token);
        }
    }
}