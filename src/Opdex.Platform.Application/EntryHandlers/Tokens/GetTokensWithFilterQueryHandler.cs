using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System.Globalization;
using System.Linq;

namespace Opdex.Platform.Application.EntryHandlers.Tokens;

public class GetTokensWithFilterQueryHandler : EntryFilterQueryHandler<GetTokensWithFilterQuery, TokensDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;
    private readonly ILogger<GetTokensWithFilterQueryHandler> _logger;

    public GetTokensWithFilterQueryHandler(IMediator mediator, IModelAssembler<Token, TokenDto> tokenAssembler, ILogger<GetTokensWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<TokensDto> Handle(GetTokensWithFilterQuery request, CancellationToken cancellationToken)
    {
        var tokens = await _mediator.Send(new RetrieveTokensWithFilterQuery(0, request.Cursor), cancellationToken);

        var dtos = await Task.WhenAll(tokens.Select(token => _tokenAssembler.Assemble(token)));

        _logger.LogTrace("Assembled queried tokens");

        var dtoResults = dtos.ToList();

        var cursor = BuildCursorDto(dtoResults, request.Cursor, pointerSelector: result => request.Cursor.OrderBy switch
            {
                TokenOrderByType.Name => (result.Name, result.Id),
                TokenOrderByType.Symbol => (result.Symbol, result.Id),
                TokenOrderByType.PriceUsd => (result.Summary?.PriceUsd.ToString(CultureInfo.InvariantCulture), result.Id),
                TokenOrderByType.DailyPriceChangePercent => (result.Summary?.DailyPriceChangePercent.ToString(CultureInfo.InvariantCulture), result.Id),
                _ => (string.Empty, result.Id)
            });

        _logger.LogTrace("Returning {ResultCount} results", dtoResults.Count);

        return new TokensDto { Tokens = dtoResults, Cursor = cursor };
    }
}
