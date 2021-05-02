using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class SelectAllLiquidityPoolsQueryHandler : IRequestHandler<SelectAllLiquidityPoolsQuery, IEnumerable<LiquidityPool>>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(LiquidityPoolEntity.Id)},
                {nameof(LiquidityPoolEntity.Address)},
                {nameof(LiquidityPoolEntity.TokenId)},
                {nameof(LiquidityPoolEntity.MarketId)},
                {nameof(LiquidityPoolEntity.ReserveCrs)},
                {nameof(LiquidityPoolEntity.ReserveSrc)},
                {nameof(LiquidityPoolEntity.CreatedDate)}
            FROM pool_liquidity;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public SelectAllLiquidityPoolsQueryHandler(IDbContext context, IMapper mapper, 
            ILogger<SelectAllLiquidityPoolsQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<LiquidityPool>> Handle(SelectAllLiquidityPoolsQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, null, cancellationToken);
            
            var tokenEntities =  await _context.ExecuteQueryAsync<LiquidityPoolEntity>(command);

            return _mapper.Map<IEnumerable<LiquidityPool>>(tokenEntities);
        }
    }
}