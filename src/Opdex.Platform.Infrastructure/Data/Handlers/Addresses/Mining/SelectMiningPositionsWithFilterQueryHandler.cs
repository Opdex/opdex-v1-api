using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Mining
{
    public class SelectMiningPositionsWithFilterQueryHandler : IRequestHandler<SelectMiningPositionsWithFilterQuery, IEnumerable<AddressMining>>
    {
        private const string TableJoins = "{TableJoins}";
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

        private static readonly string SqlQuery =
            @$"SELECT
                m.{nameof(AddressMiningEntity.Id)},
                m.{nameof(AddressMiningEntity.MiningPoolId)},
                m.{nameof(AddressMiningEntity.Owner)},
                m.{nameof(AddressMiningEntity.Balance)},
                m.{nameof(AddressMiningEntity.CreatedBlock)},
                m.{nameof(AddressMiningEntity.ModifiedBlock)}
            FROM address_mining m
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

        private const string InnerQuery = "{InnerQuery}";
        private const string SortDirection = "{SortDirection}";

        private static readonly string PagingBackwardQuery =
            @$"SELECT * FROM ({InnerQuery}) r ORDER BY r.{nameof(AddressMiningEntity.Id)} {SortDirection};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectMiningPositionsWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<AddressMining>> Handle(SelectMiningPositionsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var positionId = request.Cursor.Pointer;

            var queryParams = new SqlParams(positionId, request.Address, request.Cursor.LiquidityPools, request.Cursor.MiningPools);

            var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

            var results = await _context.ExecuteQueryAsync<AddressMiningEntity>(query);

            return _mapper.Map<IEnumerable<AddressMining>>(results);
        }

        private static string QueryBuilder(SelectMiningPositionsWithFilterQuery request)
        {
            var whereFilter = $"WHERE m.{nameof(AddressMiningEntity.Owner)} = @{nameof(SqlParams.Address)}";
            var tableJoins = string.Empty;

            var filterByLiquidityPools = request.Cursor.LiquidityPools.Any();
            var filterByMiningPools = request.Cursor.MiningPools.Any();

            if (filterByLiquidityPools || filterByMiningPools)
            {
                tableJoins += $" JOIN pool_mining pm ON pm.{nameof(MiningPoolEntity.Id)} = m.{nameof(AddressMiningEntity.MiningPoolId)}";
            }

            if (filterByLiquidityPools)
            {
                tableJoins += $" JOIN pool_liquidity pl ON pl.{nameof(LiquidityPoolEntity.Id)} = pm.{nameof(MiningPoolEntity.LiquidityPoolId)}";
            }

            if (!request.Cursor.IsFirstRequest)
            {
                var sortOperator = string.Empty;

                // going forward in ascending order, use greater than
                if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = ">";

                // going forward in descending order, use less than or equal to
                if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = "<";

                // going backward in ascending order, use less than
                if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = "<";

                // going backward in descending order, use greater than
                if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = ">";

                whereFilter += $" AND m.{nameof(AddressMiningEntity.Id)} {sortOperator} @{nameof(SqlParams.PositionId)}";
            }

            if (filterByMiningPools)
            {
                whereFilter += $" AND pm.{nameof(MiningPoolEntity.Address)} IN @{nameof(SqlParams.MiningPools)}";
            }

            if (filterByLiquidityPools)
            {
                whereFilter += $" AND pl.{nameof(LiquidityPoolEntity.Address)} IN @{nameof(SqlParams.LiquidityPools)}";
            }

            if (!request.Cursor.IncludeZeroAmounts)
            {
                whereFilter += $" AND m.{nameof(AddressMiningEntity.Balance)} != '0'";
            }

            // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
            string direction;

            if (request.Cursor.PagingDirection == PagingDirection.Backward)
            {
                direction = request.Cursor.SortDirection == SortDirectionType.DESC ? nameof(SortDirectionType.ASC) : nameof(SortDirectionType.DESC);
            }
            else
            {
                direction = Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection);
            }

            var orderBy = $" ORDER BY m.{nameof(AddressMiningEntity.Id)} {direction}";

            var limit = $" LIMIT {request.Cursor.Limit + 1}";

            var query = SqlQuery.Replace(TableJoins, tableJoins)
                                .Replace(WhereFilter, whereFilter)
                                .Replace(OrderBy, orderBy)
                                .Replace(Limit, limit);

            if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";
            // re-sort back into requested order
            else return PagingBackwardQuery.Replace(InnerQuery, query)
                                           .Replace(SortDirection, Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection));
        }

        private sealed class SqlParams
        {
            public SqlParams(long positionId, Address address, IEnumerable<Address> liquidityPools, IEnumerable<Address> miningPools)
            {
                PositionId = positionId;
                Address = address;
                LiquidityPools = liquidityPools.Select(pool => pool.ToString());
                MiningPools = miningPools.Select(pool => pool.ToString());
            }

            public long PositionId { get; }
            public Address Address { get; }
            public IEnumerable<string> LiquidityPools { get; }
            public IEnumerable<string> MiningPools { get; }
        }
    }
}
