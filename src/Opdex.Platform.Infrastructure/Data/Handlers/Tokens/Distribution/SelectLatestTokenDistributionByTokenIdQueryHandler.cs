using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;

public class SelectLatestTokenDistributionByTokenIdQueryHandler : IRequestHandler<SelectLatestTokenDistributionByTokenIdQuery, TokenDistribution>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(TokenDistributionEntity.Id)},
                {nameof(TokenDistributionEntity.TokenId)},
                {nameof(TokenDistributionEntity.VaultDistribution)},
                {nameof(TokenDistributionEntity.MiningGovernanceDistribution)},
                {nameof(TokenDistributionEntity.PeriodIndex)},
                {nameof(TokenDistributionEntity.DistributionBlock)},
                {nameof(TokenDistributionEntity.NextDistributionBlock)},
                {nameof(TokenDistributionEntity.CreatedBlock)},
                {nameof(TokenDistributionEntity.ModifiedBlock)}
            FROM token_distribution
            WHERE {nameof(TokenDistributionEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            ORDER BY {nameof(TokenDistributionEntity.NextDistributionBlock)} DESC
            LIMIT 1;";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectLatestTokenDistributionByTokenIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<TokenDistribution> Handle(SelectLatestTokenDistributionByTokenIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.TokenId), cancellationToken);

        var result = await _context.ExecuteFindAsync<TokenDistributionEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(TokenDistribution)} not found.");
        }

        return result == null ? null : _mapper.Map<TokenDistribution>(result);
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
