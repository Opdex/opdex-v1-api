using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances.Nominations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances.Nominations;

public class SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler
    : IRequestHandler<SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery, MiningGovernanceNomination>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(MiningGovernanceNominationEntity.Id)},
                {nameof(MiningGovernanceNominationEntity.MiningGovernanceId)},
                {nameof(MiningGovernanceNominationEntity.LiquidityPoolId)},
                {nameof(MiningGovernanceNominationEntity.MiningPoolId)},
                {nameof(MiningGovernanceNominationEntity.IsNominated)},
                {nameof(MiningGovernanceNominationEntity.Weight)},
                {nameof(MiningGovernanceNominationEntity.CreatedBlock)},
                {nameof(MiningGovernanceNominationEntity.ModifiedBlock)}
            FROM mining_governance_nomination
            WHERE {nameof(MiningGovernanceNominationEntity.MiningGovernanceId)} = @{nameof(SqlParams.MiningGovernanceId)}
                AND {nameof(MiningGovernanceNominationEntity.LiquidityPoolId)} = @{nameof(SqlParams.LiquidityPoolId)}
                AND {nameof(MiningGovernanceNominationEntity.MiningPoolId)} = @{nameof(SqlParams.MiningPoolId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<MiningGovernanceNomination> Handle(SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.MiningGovernanceId, request.LiquidityPoolId, request.MiningPoolId), cancellationToken);

        var result = await _context.ExecuteFindAsync<MiningGovernanceNominationEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"Mining governance nomination not found.");
        }

        return result == null ? null : _mapper.Map<MiningGovernanceNomination>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong miningGovernanceId, ulong liquidityPoolId, ulong miningPoolId)
        {
            MiningGovernanceId = miningGovernanceId;
            LiquidityPoolId = liquidityPoolId;
            MiningPoolId = miningPoolId;
        }

        public ulong MiningGovernanceId { get; }
        public ulong LiquidityPoolId { get; }
        public ulong MiningPoolId { get; }
    }
}