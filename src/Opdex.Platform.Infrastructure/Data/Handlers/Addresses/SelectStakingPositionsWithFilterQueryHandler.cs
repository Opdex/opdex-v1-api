using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses
{
    public class SelectStakingPositionsWithFilterQueryHandler : IRequestHandler<SelectStakingPositionsWithFilterQuery, IEnumerable<AddressStaking>>
    {
        private const string TableJoins = "{TableJoins}";
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

        private static readonly string SqlQuery =
            @$"SELECT
                s.{nameof(AddressStakingEntity.Id)},
                s.{nameof(AddressStakingEntity.LiquidityPoolId)},
                s.{nameof(AddressStakingEntity.Owner)},
                s.{nameof(AddressStakingEntity.Weight)},
                s.{nameof(AddressStakingEntity.CreatedBlock)},
                s.{nameof(AddressStakingEntity.ModifiedBlock)}
            FROM address_staking s
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectStakingPositionsWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<AddressStaking>> Handle(SelectStakingPositionsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var positionId = request.Cursor.Pointer;

            var queryParams = new SqlParams(positionId, request.Address, request.Cursor.LiquidityPools);

            var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

            var results = await _context.ExecuteQueryAsync<AddressStakingEntity>(query);

            return _mapper.Map<IEnumerable<AddressStaking>>(results);
        }

        private static string QueryBuilder(SelectStakingPositionsWithFilterQuery request)
        {
            var whereFilter = $"WHERE s.{nameof(AddressStakingEntity.Owner)} = @{nameof(SqlParams.Address)}";
            var tableJoins = string.Empty;

            if (request.Cursor.LiquidityPools.Any())
            {
                tableJoins += $" JOIN pool_liquidity pl ON pl.{nameof(LiquidityPoolEntity.Id)} = s.{nameof(AddressStakingEntity.LiquidityPoolId)}";
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

                whereFilter += $" AND s.{nameof(AddressStakingEntity.Id)} {sortOperator} @{nameof(SqlParams.PositionId)}";
            }

            if (request.Cursor.LiquidityPools.Any())
            {
                whereFilter += $" AND pl.{nameof(LiquidityPoolEntity.Address)} IN @{nameof(SqlParams.LiquidityPools)}";
            }

            if (!request.Cursor.IncludeZeroAmounts)
            {
                whereFilter += $" AND s.{nameof(AddressStakingEntity.Weight)} != '0'";
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

            var orderBy = $" ORDER BY s.{nameof(AddressStakingEntity.Id)} {direction}";

            var limit = $" LIMIT {request.Cursor.Limit + 1}";

            var query = SqlQuery
                .Replace(TableJoins, tableJoins)
                .Replace(WhereFilter, whereFilter)
                .Replace(OrderBy, orderBy)
                .Replace(Limit, limit);

            if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";
            // re-sort back into requested order
            else return $"SELECT * FROM ({query}) r ORDER BY r.{nameof(AddressStakingEntity.Id)} {Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection)};";
        }

        private sealed class SqlParams
        {
            public SqlParams(long positionId, string address, IEnumerable<string> liquidityPools)
            {
                PositionId = positionId;
                Address = address;
                LiquidityPools = liquidityPools;
            }

            public long PositionId { get; }
            public string Address { get; }
            public IEnumerable<string> LiquidityPools { get; }
        }
    }
}
