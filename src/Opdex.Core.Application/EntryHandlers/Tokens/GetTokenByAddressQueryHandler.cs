using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Tokens;

namespace Opdex.Core.Application.EntryHandlers.Tokens
{
    public class GetTokenByAddressQueryHandler: IRequestHandler<GetTokenByAddressQuery, TokenDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public GetTokenByAddressQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<TokenDto> Handle(GetTokenByAddressQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Address), cancellationToken);

            return _mapper.Map<TokenDto>(token);
        }
    }
}