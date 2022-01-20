using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;

public class SelectLiquidityPoolByIdQueryHandler : IRequestHandler<SelectLiquidityPoolByIdQuery, LiquidityPool>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(LiquidityPoolEntity.Id)},
                {nameof(LiquidityPoolEntity.Name)},
                {nameof(LiquidityPoolEntity.Address)},
                {nameof(LiquidityPoolEntity.SrcTokenId)},
                {nameof(LiquidityPoolEntity.LpTokenId)},
                {nameof(LiquidityPoolEntity.MarketId)},
                {nameof(LiquidityPoolEntity.CreatedBlock)},
                {nameof(LiquidityPoolEntity.ModifiedBlock)}
            FROM pool_liquidity
            WHERE {nameof(LiquidityPoolEntity.Id)} = @{nameof(SqlParams.LiquidityPoolId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectLiquidityPoolByIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<LiquidityPool> Handle(SelectLiquidityPoolByIdQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.LiquidityPoolId);
        var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<LiquidityPoolEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(LiquidityPool)} not found.");
        }

        return result == null ? null : _mapper.Map<LiquidityPool>(result);
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