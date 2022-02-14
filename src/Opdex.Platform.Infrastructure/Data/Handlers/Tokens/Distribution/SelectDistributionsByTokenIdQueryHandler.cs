using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;

public class SelectDistributionsByTokenIdQueryHandler : IRequestHandler<SelectDistributionsByTokenIdQuery, IEnumerable<TokenDistribution>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(TokenDistributionEntity.Id)},
                {nameof(TokenDistributionEntity.TokenId)},
                {nameof(TokenDistributionEntity.VaultDistribution)},
                {nameof(TokenDistributionEntity.MiningGovernanceDistribution)},
                {nameof(TokenDistributionEntity.PeriodIndex)},
                {nameof(TokenDistributionEntity.DistributionBlock)},
                {nameof(TokenDistributionEntity.NextDistributionBlock)}
            FROM token_distribution
            WHERE {nameof(TokenDistributionEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            ORDER BY {nameof(TokenDistributionEntity.NextDistributionBlock)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectDistributionsByTokenIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<TokenDistribution>> Handle(SelectDistributionsByTokenIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.TokenId), cancellationToken);

        var result = await _context.ExecuteQueryAsync<TokenDistributionEntity>(query);

        return _mapper.Map<IEnumerable<TokenDistribution>>(result);
    }

    private class SqlParams
    {
        public SqlParams(ulong tokenId)
        {
            TokenId = tokenId;
        }

        public ulong TokenId { get; }
    }
}
