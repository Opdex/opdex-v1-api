using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningPools;

public class SelectMiningPoolsByModifiedBlockQueryHandler : IRequestHandler<SelectMiningPoolsByModifiedBlockQuery, IEnumerable<MiningPool>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(MiningPoolEntity.Id)},
                {nameof(MiningPoolEntity.LiquidityPoolId)},
                {nameof(MiningPoolEntity.Address)},
                {nameof(MiningPoolEntity.RewardPerBlock)},
                {nameof(MiningPoolEntity.RewardPerLpt)},
                {nameof(MiningPoolEntity.MiningPeriodEndBlock)},
                {nameof(MiningPoolEntity.ModifiedBlock)},
                {nameof(MiningPoolEntity.CreatedBlock)}
            FROM pool_mining
            WHERE {nameof(MiningPoolEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectMiningPoolsByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<MiningPool>> Handle(SelectMiningPoolsByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

        var result = await _context.ExecuteQueryAsync<MiningPoolEntity>(query);

        return _mapper.Map<IEnumerable<MiningPool>>(result);
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