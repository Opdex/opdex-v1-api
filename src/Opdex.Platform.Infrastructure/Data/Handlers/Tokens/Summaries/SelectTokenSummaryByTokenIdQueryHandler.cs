using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Summaries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Summaries;

public class SelectTokenSummaryByTokenIdQueryHandler : IRequestHandler<SelectTokenSummaryByTokenIdQuery, TokenSummary>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(TokenSummaryEntity.TokenId)},
                ROUND(AVG({nameof(TokenSummaryEntity.DailyPriceChangePercent)}), 8) AS {nameof(TokenSummaryEntity.DailyPriceChangePercent)},
                ROUND(AVG({nameof(TokenSummaryEntity.PriceUsd)}), 8) AS {nameof(TokenSummaryEntity.PriceUsd)},
                MIN({nameof(TokenSummaryEntity.CreatedBlock)}) AS {nameof(TokenSummaryEntity.CreatedBlock)},
                MAX({nameof(TokenSummaryEntity.ModifiedBlock)}) AS {nameof(TokenSummaryEntity.ModifiedBlock)}
            FROM token_summary
            WHERE {nameof(TokenSummaryEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            GROUP BY {nameof(TokenSummaryEntity.TokenId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTokenSummaryByTokenIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<TokenSummary> Handle(SelectTokenSummaryByTokenIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.TokenId), cancellationToken);

        var result = await _context.ExecuteFindAsync<TokenSummaryEntity>(query);

        return result is not null ? _mapper.Map<TokenSummary>(result) : null;
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong tokenId)
        {
            TokenId = tokenId;
        }

        public ulong TokenId { get; }
    }
}
