using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets
{
    public class SelectActiveMarketRouterByMarketIdQueryHandler : IRequestHandler<SelectActiveMarketRouterByMarketIdQuery, MarketRouter>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(MarketRouterEntity.Id)},
                {nameof(MarketRouterEntity.Address)},
                {nameof(MarketRouterEntity.MarketId)},
                {nameof(MarketRouterEntity.IsActive)},
                {nameof(MarketRouterEntity.CreatedBlock)},
                {nameof(MarketRouterEntity.ModifiedBlock)}
            FROM market_router
            WHERE {nameof(MarketRouterEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
                AND {nameof(MarketRouterEntity.IsActive)} = true
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public SelectActiveMarketRouterByMarketIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<MarketRouter> Handle(SelectActiveMarketRouterByMarketIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.MarketId);
            
            var command = DatabaseQuery.Create(SqlCommand,  queryParams, cancellationToken);
            
            var result =  await _context.ExecuteFindAsync<MarketRouterEntity>(command);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(Market)} not found.");
            }

            return result == null ? null : _mapper.Map<MarketRouter>(result);
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