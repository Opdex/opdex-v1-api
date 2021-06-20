using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class SelectAllLiquidityPoolsByMarketIdQueryHandler : IRequestHandler<SelectAllLiquidityPoolsByMarketIdQuery, IEnumerable<LiquidityPool>>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(LiquidityPoolEntity.Id)},
                {nameof(LiquidityPoolEntity.Address)},
                {nameof(LiquidityPoolEntity.SrcTokenId)},
                {nameof(LiquidityPoolEntity.LpTokenId)},
                {nameof(LiquidityPoolEntity.MarketId)},
                {nameof(LiquidityPoolEntity.CreatedBlock)},
                {nameof(LiquidityPoolEntity.ModifiedBlock)}
            FROM pool_liquidity
            WHERE {nameof(LiquidityPoolEntity.MarketId)} = @{nameof(SqlParams.MarketId)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAllLiquidityPoolsByMarketIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LiquidityPool>> Handle(SelectAllLiquidityPoolsByMarketIdQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, new SqlParams(request.MarketId), cancellationToken);

            var tokenEntities =  await _context.ExecuteQueryAsync<LiquidityPoolEntity>(command);

            return _mapper.Map<IEnumerable<LiquidityPool>>(tokenEntities);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long marketId)
            {
                MarketId = marketId;
            }

            public long MarketId { get; }
        }
    }
}