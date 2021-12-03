using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Summaries;

public class SelectTokenSummaryByMarketAndTokenIdQueryHandler : IRequestHandler<SelectTokenSummaryByMarketAndTokenIdQuery, TokenSummary>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(TokenSummaryEntity.Id)},
                {nameof(TokenSummaryEntity.MarketId)},
                {nameof(TokenSummaryEntity.TokenId)},
                {nameof(TokenSummaryEntity.DailyPriceChangePercent)},
                {nameof(TokenSummaryEntity.PriceUsd)},
                {nameof(TokenSummaryEntity.CreatedBlock)},
                {nameof(TokenSummaryEntity.ModifiedBlock)}
            FROM token_summary
            WHERE {nameof(TokenSummaryEntity.MarketId)} = @{nameof(SqlParams.MarketId)} AND
                  {nameof(TokenSummaryEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTokenSummaryByMarketAndTokenIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<TokenSummary> Handle(SelectTokenSummaryByMarketAndTokenIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.MarketId, request.TokenId), cancellationToken);

        var result = await _context.ExecuteFindAsync<TokenSummaryEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(TokenSummary)} not found.");
        }

        return result == null ? null : _mapper.Map<TokenSummary>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong marketId, ulong tokenId)
        {
            MarketId = marketId;
            TokenId = tokenId;
        }

        public ulong MarketId { get; }
        public ulong TokenId { get; }
    }
}