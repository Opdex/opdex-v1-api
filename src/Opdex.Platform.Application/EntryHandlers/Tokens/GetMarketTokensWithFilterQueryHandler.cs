using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class GetMarketTokensWithFilterQueryHandler : EntryFilterQueryHandler<GetMarketTokensWithFilterQuery, MarketTokensDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<MarketToken, MarketTokenDto> _marketTokenAssembler;

        public GetMarketTokensWithFilterQueryHandler(IMediator mediator, IModelAssembler<MarketToken, MarketTokenDto> marketTokenAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _marketTokenAssembler = marketTokenAssembler ?? throw new ArgumentNullException(nameof(marketTokenAssembler));
        }

        public override async Task<MarketTokensDto> Handle(GetMarketTokensWithFilterQuery request, CancellationToken cancellationToken)
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market), cancellationToken);

            var tokens = await _mediator.Send(new RetrieveTokensWithFilterQuery(market.Id, request.Cursor), cancellationToken);

            var dtos = await Task.WhenAll(tokens.Select(token => _marketTokenAssembler.Assemble(new MarketToken(market, token))));

            var dtoResults = dtos.ToList();

            var cursor = BuildCursorDto(dtoResults, request.Cursor, pointerSelector: result =>
            {
                return request.Cursor.OrderBy switch
                {
                    TokenOrderByType.PriceUsd => (result.Summary.PriceUsd, result.Id),
                    TokenOrderByType.DailyPriceChangePercent => (result.Summary.DailyPriceChangePercent, result.Id),
                    _ => (0, result.Id)
                };
            });

            return new MarketTokensDto { Tokens = dtoResults, Cursor = cursor };
        }
    }
}
