using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances.Nominations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Governances.Nominations
{
    public class SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler
        : IRequestHandler<SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery, MiningGovernanceNomination>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(MiningGovernanceNominationEntity.Id)},
                {nameof(MiningGovernanceNominationEntity.GovernanceId)},
                {nameof(MiningGovernanceNominationEntity.LiquidityPoolId)},
                {nameof(MiningGovernanceNominationEntity.MiningPoolId)},
                {nameof(MiningGovernanceNominationEntity.IsNominated)},
                {nameof(MiningGovernanceNominationEntity.Weight)},
                {nameof(MiningGovernanceNominationEntity.CreatedBlock)},
                {nameof(MiningGovernanceNominationEntity.ModifiedBlock)}
            FROM governance_nomination
            WHERE {nameof(MiningGovernanceNominationEntity.GovernanceId)} = @{nameof(SqlParams.GovernanceId)}
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
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.GovernanceId, request.LiquidityPoolId, request.MiningPoolId), cancellationToken);

            var result = await _context.ExecuteFindAsync<MiningGovernanceNominationEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(MiningGovernanceNomination)} not found.");
            }

            return result == null ? null : _mapper.Map<MiningGovernanceNomination>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long governanceId, long liquidityPoolId, long miningPoolId)
            {
                GovernanceId = governanceId;
                LiquidityPoolId = liquidityPoolId;
                MiningPoolId = miningPoolId;
            }

            public long GovernanceId { get; }
            public long LiquidityPoolId { get; }
            public long MiningPoolId { get; }
        }
    }
}