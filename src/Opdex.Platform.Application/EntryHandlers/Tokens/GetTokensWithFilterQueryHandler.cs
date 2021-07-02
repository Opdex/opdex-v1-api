using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class GetTokensWithFilterQueryHandler : IRequestHandler<GetTokensWithFilterQuery, IEnumerable<TokenDto>>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;

        public GetTokensWithFilterQueryHandler(IMediator mediator, IModelAssembler<Token, TokenDto> tokenAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        }

        public async Task<IEnumerable<TokenDto>> Handle(GetTokensWithFilterQuery request, CancellationToken cancellationToken)
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.MarketAddress), cancellationToken);

            var tokens = await _mediator.Send(new RetrieveTokensWithFilterQuery(market.Id,
                                                                                request.LpToken,
                                                                                request.Skip,
                                                                                request.Take,
                                                                                request.SortBy,
                                                                                request.OrderBy,
                                                                                request.Tokens), cancellationToken);

            var tokenDtos = new List<TokenDto>();

            foreach (var token in tokens)
            {
                var marketId = token.Address == TokenConstants.Cirrus.Address ? 0 : market.Id;

                token.SetMarket(marketId);

                var tokenDto = await _tokenAssembler.Assemble(token);

                tokenDtos.Add(tokenDto);
            }

            return tokenDtos;
        }
    }
}
