using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class GetTokenByAddressQueryHandler : IRequestHandler<GetTokenByAddressQuery, TokenDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;

        public GetTokenByAddressQueryHandler(IMediator mediator, IMapper mapper, IModelAssembler<Token, TokenDto> tokenAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        }

        public async Task<TokenDto> Handle(GetTokenByAddressQuery request, CancellationToken cancellationToken)
        {
            var market = request.Market != Address.Empty
                ? await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, false), cancellationToken)
                : null;

            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Address), cancellationToken);

            if (market == null)
            {
                return _mapper.Map<TokenDto>(token);
            }

            token.SetMarket(market.Id);

            return await _tokenAssembler.Assemble(token);
        }
    }
}
