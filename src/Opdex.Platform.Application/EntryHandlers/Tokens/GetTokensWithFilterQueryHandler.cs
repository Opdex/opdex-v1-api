using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System.Linq;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class GetTokensWithFilterQueryHandler : EntryFilterQueryHandler<GetTokensWithFilterQuery, TokensDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;

        public GetTokensWithFilterQueryHandler(IMediator mediator, IModelAssembler<Token, TokenDto> tokenAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        }

        public override async Task<TokensDto> Handle(GetTokensWithFilterQuery request, CancellationToken cancellationToken)
        {
            var tokens = await _mediator.Send(new RetrieveTokensWithFilterQuery(0, request.Cursor), cancellationToken);

            var dtos = await Task.WhenAll(tokens.Select(token => _tokenAssembler.Assemble(token)));

            var dtoResults = dtos.ToList();

            var cursor = BuildCursorDto(dtoResults, request.Cursor, pointerSelector: result =>
            {
                return request.Cursor.OrderBy switch
                {
                    TokenOrderByType.Name => (result.Name, result.Id),
                    TokenOrderByType.Symbol => (result.Symbol, result.Id),
                    TokenOrderByType.PriceUsd => (result.Summary?.PriceUsd.ToString(), result.Id),
                    TokenOrderByType.DailyPriceChangePercent => (result.Summary?.DailyPriceChangePercent.ToString(), result.Id),
                    _ => (string.Empty, result.Id)
                };
            });

            return new TokensDto { Tokens = dtoResults, Cursor = cursor };
        }
    }
}
