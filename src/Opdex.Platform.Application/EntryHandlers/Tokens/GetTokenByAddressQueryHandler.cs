using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
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