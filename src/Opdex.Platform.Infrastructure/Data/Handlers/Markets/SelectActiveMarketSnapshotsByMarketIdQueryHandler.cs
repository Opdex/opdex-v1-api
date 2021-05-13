using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets
{
    public class SelectActiveMarketSnapshotsByMarketIdQueryHandler 
        : IRequestHandler<SelectActiveMarketSnapshotsByMarketIdQuery, IEnumerable<MarketSnapshot>>
    {
        private static readonly string SqlQuery =
            @$"Select 
                {nameof(MarketSnapshotEntity.Id)},
                {nameof(MarketSnapshotEntity.MarketId)},
                {nameof(MarketSnapshotEntity.TransactionCount)},
                {nameof(MarketSnapshotEntity.Liquidity)},
                {nameof(MarketSnapshotEntity.Volume)},
                {nameof(MarketSnapshotEntity.StakingWeight)},
                {nameof(MarketSnapshotEntity.StakingUsd)},
                {nameof(MarketSnapshotEntity.ProviderRewards)},
                {nameof(MarketSnapshotEntity.StakerRewards)},
                {nameof(MarketSnapshotEntity.SnapshotTypeId)},
                {nameof(MarketSnapshotEntity.StartDate)},
                {nameof(MarketSnapshotEntity.EndDate)},
                {nameof(MarketSnapshotEntity.CreatedDate)}
            FROM market_snapshot
            WHERE {nameof(MarketSnapshotEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
                AND @{nameof(SqlParams.Time)} BETWEEN 
                    {nameof(MarketSnapshotEntity.StartDate)} AND {nameof(MarketSnapshotEntity.EndDate)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectActiveMarketSnapshotsByMarketIdQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<MarketSnapshot>> Handle(SelectActiveMarketSnapshotsByMarketIdQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.MarketId, request.Time);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<MarketSnapshotEntity>(query);

            return _mapper.Map<IEnumerable<MarketSnapshot>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long tokenId, DateTime time)
            {
                MarketId = tokenId;
                Time = time;
            }

            public long MarketId { get; }
            public DateTime Time { get; }
        }
    }
}