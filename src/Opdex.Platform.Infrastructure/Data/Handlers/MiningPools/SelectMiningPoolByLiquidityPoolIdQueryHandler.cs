using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningPools;

public class SelectMiningPoolByLiquidityPoolIdQueryHandler : IRequestHandler<SelectMiningPoolByLiquidityPoolIdQuery, MiningPool>
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
            WHERE {nameof(MiningPoolEntity.LiquidityPoolId)} = @{nameof(SqlParams.LiquidityPoolId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectMiningPoolByLiquidityPoolIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<MiningPool> Handle(SelectMiningPoolByLiquidityPoolIdQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.LiquidityPoolId);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<MiningPoolEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException("Mining pool not found.");
        }

        return result == null ? null : _mapper.Map<MiningPool>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong liquidityPoolId)
        {
            LiquidityPoolId = liquidityPoolId;
        }

        public ulong LiquidityPoolId { get; }
    }
}
