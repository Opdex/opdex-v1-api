using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances.Nominations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances.Nominations;

public class SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandler
    : IRequestHandler<SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQuery, IEnumerable<MiningGovernanceNomination>>
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
            WHERE {nameof(MiningGovernanceNominationEntity.IsNominated)} = true AND
                  {nameof(MiningGovernanceNominationEntity.MiningGovernanceId)} = @{nameof(SqlParams.MiningGovernanceId)}
            LIMIT {MiningGovernanceConstants.MaxNominations};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<MiningGovernanceNomination>> Handle(SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.MiningGovernanceId), cancellationToken);

        var result = await _context.ExecuteQueryAsync<MiningGovernanceNominationEntity>(query);

        return _mapper.Map<IEnumerable<MiningGovernanceNomination>>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong miningGovernanceId)
        {
            MiningGovernanceId = miningGovernanceId;
        }

        public ulong MiningGovernanceId { get; }
    }
}