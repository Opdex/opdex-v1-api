using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Market;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Market
{
    public class SelectLatestMarketSnapshotQueryHandler : IRequestHandler<SelectLatestMarketSnapshotQuery, MarketSnapshot>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(MarketSnapshotEntity.Id)},
                {nameof(MarketSnapshotEntity.TokenCount)},
                {nameof(MarketSnapshotEntity.PoolCount)},
                {nameof(MarketSnapshotEntity.DailyTransactionCount)},
                {nameof(MarketSnapshotEntity.CrsPrice)},
                {nameof(MarketSnapshotEntity.Liquidity)},
                {nameof(MarketSnapshotEntity.DailyFees)},
                {nameof(MarketSnapshotEntity.DailyVolume)},
                {nameof(MarketSnapshotEntity.Block)}
            FROM market_snapshot
            ORDER BY {nameof(MarketSnapshotEntity.Id)} DESC
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public SelectLatestMarketSnapshotQueryHandler(IDbContext context, IMapper mapper, 
            ILogger<SelectLatestMarketSnapshotQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<MarketSnapshot> Handle(SelectLatestMarketSnapshotQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, null, cancellationToken);
            
            var marketEntity =  await _context.ExecuteFindAsync<MarketSnapshotEntity>(command);

            return _mapper.Map<MarketSnapshot>(marketEntity);
        }
    }
}