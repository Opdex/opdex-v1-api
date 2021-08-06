using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Governances
{
    public class SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler
        : IRequestHandler<SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery, MiningGovernanceNomination>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(MiningGovernanceNominationEntity.Id)},
                {nameof(MiningGovernanceNominationEntity.LiquidityPoolId)},
                {nameof(MiningGovernanceNominationEntity.MiningPoolId)},
                {nameof(MiningGovernanceNominationEntity.IsNominated)},
                {nameof(MiningGovernanceNominationEntity.Weight)},
                {nameof(MiningGovernanceNominationEntity.CreatedBlock)},
                {nameof(MiningGovernanceNominationEntity.ModifiedBlock)}
            FROM governance_nomination
            WHERE {nameof(MiningGovernanceNominationEntity.LiquidityPoolId)} = @{nameof(SqlParams.LiquidityPoolId)}
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
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.LiquidityPoolId, request.MiningPoolId), cancellationToken);

            var result = await _context.ExecuteFindAsync<MiningGovernanceNominationEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(MiningGovernanceNomination)} not found.");
            }

            return result == null ? null : _mapper.Map<MiningGovernanceNomination>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long liquidityPoolId, long miningPoolId)
            {
                LiquidityPoolId = liquidityPoolId;
                MiningPoolId = miningPoolId;
            }

            public long LiquidityPoolId { get; }
            public long MiningPoolId { get; }
        }
    }
}
