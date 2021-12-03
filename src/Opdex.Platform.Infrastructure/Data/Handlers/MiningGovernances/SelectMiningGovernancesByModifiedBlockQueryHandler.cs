using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances;

public class SelectMiningGovernancesByModifiedBlockQueryHandler : IRequestHandler<SelectMiningGovernancesByModifiedBlockQuery, IEnumerable<MiningGovernance>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(MiningGovernanceEntity.Id)},
                {nameof(MiningGovernanceEntity.Address)},
                {nameof(MiningGovernanceEntity.TokenId)},
                {nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                {nameof(MiningGovernanceEntity.MiningDuration)},
                {nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                {nameof(MiningGovernanceEntity.MiningPoolReward)},
                {nameof(MiningGovernanceEntity.CreatedBlock)},
                {nameof(MiningGovernanceEntity.ModifiedBlock)}
            FROM mining_governance
            WHERE {nameof(MiningGovernanceEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectMiningGovernancesByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<MiningGovernance>> Handle(SelectMiningGovernancesByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

        var result = await _context.ExecuteQueryAsync<MiningGovernanceEntity>(query);

        return _mapper.Map<IEnumerable<MiningGovernance>>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong modifiedBlock)
        {
            ModifiedBlock = modifiedBlock;
        }

        public ulong ModifiedBlock { get; }
    }
}