using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class GetAllTokensByMarketAddressQueryHandler : IRequestHandler<GetAllTokensByMarketAddressQuery, IEnumerable<TokenDto>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetAllTokensByMarketAddressQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TokenDto>> Handle(GetAllTokensByMarketAddressQuery request, CancellationToken cancellationToken)
        {
            // Todo: Implement assembler to include daily snapshot - only include tokens that have a snapshot
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.MarketAddress));

            var query = new RetrieveAllTokensQuery();

            var tokens = await _mediator.Send(query, cancellationToken);

            return _mapper.Map<IEnumerable<TokenDto>>(tokens);
        }
    }
}
