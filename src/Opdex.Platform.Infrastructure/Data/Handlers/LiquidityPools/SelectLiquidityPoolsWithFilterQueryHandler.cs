using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools
{
    public class SelectLiquidityPoolsWithFilterQueryHandler : IRequestHandler<SelectLiquidityPoolsWithFilterQuery, IEnumerable<LiquidityPool>>
    {
        private const string TableJoins = "{TableJoins}";
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

        private static readonly string SqlCommand =
            $@"SELECT
                pl.{nameof(LiquidityPoolEntity.Id)},
                pl.{nameof(LiquidityPoolEntity.Address)},
                pl.{nameof(LiquidityPoolEntity.SrcTokenId)},
                pl.{nameof(LiquidityPoolEntity.LpTokenId)},
                pl.{nameof(LiquidityPoolEntity.MarketId)},
                pl.{nameof(LiquidityPoolEntity.CreatedBlock)},
                pl.{nameof(LiquidityPoolEntity.ModifiedBlock)}
            FROM pool_liquidity pl
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectLiquidityPoolsWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LiquidityPool>> Handle(SelectLiquidityPoolsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(QueryBuilder(request), new SqlParams(request.MarketId, request.Pools), cancellationToken);

            var results = await _context.ExecuteQueryAsync<LiquidityPoolEntity>(command);

            return _mapper.Map<IEnumerable<LiquidityPool>>(results);
        }

        private static string QueryBuilder(SelectLiquidityPoolsWithFilterQuery request)
        {
            var whereFilter = $"WHERE pl.{nameof(LiquidityPoolEntity.MarketId)} = @{nameof(SqlParams.MarketId)}";
            var tableJoins = string.Empty;

            // Pools filter
            if (request.Pools.Any())
            {
                whereFilter += $" AND pl.{nameof(LiquidityPoolEntity.Address)} IN @{nameof(SqlParams.Pools)}";
            }

            // Mining filter
            if (request.Mining.HasValue)
            {
                tableJoins += $" JOIN pool_mining pm on pl.{nameof(LiquidityPoolEntity.Id)} = pm.{nameof(MiningPoolEntity.LiquidityPoolId)}";

                var conditional = request.Mining.Value ? ">=" : "<";

                whereFilter += $@" AND (pm.{nameof(MiningPoolEntity.MiningPeriodEndBlock)} {conditional}
                                (Select {nameof(BlockEntity.Height)} FROM block ORDER BY {nameof(BlockEntity.Height)} DESC LIMIT 1))";
            }

            // Staking filter
            if (request.Staking.HasValue)
            {
                tableJoins += $" JOIN market m ON m.{nameof(MarketEntity.Id)} = pl.{nameof(LiquidityPoolEntity.MarketId)}";

                whereFilter += request.Staking.Value
                    // IsStakingMarket && Pool.SrcTokenId != Market.StakingTokenId
                    ? $@" AND
                        (m.{nameof(MarketEntity.StakingTokenId)} IS NOT NULL AND
                         m.{nameof(MarketEntity.StakingTokenId)} > 0 AND
                         pl.SrcTokenId != m.{nameof(MarketEntity.StakingTokenId)})"
                    // IsNotStakingMarket || Pool.SrcTokenId == Market.StakingTokenId
                    : $@" AND
                        (m.{nameof(MarketEntity.StakingTokenId)} IS NULL OR
                        (m.{nameof(MarketEntity.StakingTokenId)} > 0 AND
                        pl.{nameof(LiquidityPoolEntity.SrcTokenId)} = m.{nameof(MarketEntity.StakingTokenId)}))";
            }

            // Nominated filter
            if (request.Nominated.HasValue)
            {
                tableJoins += $@" JOIN governance_nomination gn
                                ON gn.{nameof(MiningGovernanceNominationEntity.LiquidityPoolId)} = pl.{nameof(LiquidityPoolEntity.Id)}";

                whereFilter += $" AND gn.{nameof(MiningGovernanceNominationEntity.IsNominated)} = {request.Nominated.Value}";
            }

            // Sort Found Pools
            var orderBy = OrderByBuilder(request.SortBy, request.OrderBy);
            if (orderBy.HasValue())
            {
                tableJoins += $@" JOIN pool_liquidity_snapshot pls ON
                                pls.{nameof(LiquidityPoolSnapshotEntity.LiquidityPoolId)} = pl.{nameof(LiquidityPoolEntity.Id)}
                                AND pls.{nameof(LiquidityPoolSnapshotEntity.EndDate)} > UTC_TIMESTAMP()
                                AND pls.{nameof(LiquidityPoolSnapshotEntity.SnapshotTypeId)} = {(int)SnapshotType.Daily}";
            }

            // Build Limit string for pagination
            var limit = LimitBuilder(request.Skip, request.Take);

            return SqlCommand
                .Replace(TableJoins, tableJoins)
                .Replace(WhereFilter, whereFilter)
                .Replace(OrderBy, orderBy)
                .Replace(Limit, limit);
        }

        private static string OrderByBuilder(string sortRequest, string orderRequest)
        {
            if (!sortRequest.HasValue())
            {
                return string.Empty;
            }

            var baseSort = " ORDER BY CAST(JSON_EXTRACT(pls.Details, '$.{0}') as Decimal) {1}";

            orderRequest = orderRequest.HasValue() && (orderRequest.EqualsIgnoreCase("ASC") || orderRequest.EqualsIgnoreCase("DESC"))
                ? orderRequest.ToUpper()
                : "DESC";

            return sortRequest switch
            {
                "Liquidity" => string.Format(baseSort, "reserves.usd", orderRequest),
                "Volume" => string.Format(baseSort, "volume.usd", orderRequest),
                "StakingWeight" => string.Format(baseSort, "staking.weight", orderRequest),
                "StakingUsd" => string.Format(baseSort, "staking.usd", orderRequest),
                // Todo: Consider persisting the TotalRewards for this sort
                "ProviderRewards" => string.Format(baseSort, "rewards.providerUsd", orderRequest),
                "MarketRewards" => string.Format(baseSort, "rewards.marketUsd", orderRequest),
                _ => throw new ArgumentOutOfRangeException(nameof(sortRequest), "Invalid liquidity pool sort type.")
            };
        }

        private static string LimitBuilder(uint skip, uint take)
        {
            return skip == 0 && take == 0 ? string.Empty : $" LIMIT {skip}, {take}";
        }

        private sealed class SqlParams
        {
            internal SqlParams(long marketId, IEnumerable<Address> pools)
            {
                MarketId = marketId;
                Pools = pools.Select(pool => pool.ToString());
            }

            public long MarketId { get; }
            public IEnumerable<string> Pools { get; }
        }
    }
}
