using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningPools;

public class SelectMiningPoolsWithFilterQueryHandler : IRequestHandler<SelectMiningPoolsWithFilterQuery, IEnumerable<MiningPool>>
{
    private const string TableJoins = "{TableJoins}";
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        @$"SELECT
                pm.{nameof(MiningPoolEntity.Id)},
                pm.{nameof(MiningPoolEntity.LiquidityPoolId)},
                pm.{nameof(MiningPoolEntity.Address)},
                pm.{nameof(MiningPoolEntity.RewardPerBlock)},
                pm.{nameof(MiningPoolEntity.RewardPerLpt)},
                pm.{nameof(MiningPoolEntity.MiningPeriodEndBlock)},
                pm.{nameof(MiningPoolEntity.ModifiedBlock)},
                pm.{nameof(MiningPoolEntity.CreatedBlock)}
            FROM pool_mining pm
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string SortDirection = "{SortDirection}";

    private static readonly string PagingBackwardQuery =
        @$"SELECT * FROM ({InnerQuery}) results ORDER BY results.{nameof(MiningPoolEntity.Id)} {SortDirection};";

    private const string BlockHeightQuery = "SELECT MAX(b.Height) FROM block b";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectMiningPoolsWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<MiningPool>> Handle(SelectMiningPoolsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var miningPoolId = request.Cursor.Pointer;

        var queryParams = new SqlParams(miningPoolId, request.Cursor.LiquidityPools);

        var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<MiningPoolEntity>(query);

        return _mapper.Map<IEnumerable<MiningPool>>(results);
    }

    private static string QueryBuilder(SelectMiningPoolsWithFilterQuery request)
    {
        var whereFilter = string.Empty;
        var tableJoins = string.Empty;

        var filterOnLiquidityPools = request.Cursor.LiquidityPools.Any();

        if (filterOnLiquidityPools) tableJoins += $" JOIN pool_liquidity pl ON pl.{nameof(LiquidityPoolEntity.Id)} = pm.{nameof(MiningPoolEntity.LiquidityPoolId)}";

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

            whereFilter += $" WHERE pm.{nameof(MiningPoolEntity.Id)} {sortOperator} @{nameof(SqlParams.MiningPoolId)}";
        }

        if (filterOnLiquidityPools)
        {
            whereFilter += whereFilter == "" ? " WHERE" : " AND";
            whereFilter += $" pl.{nameof(LiquidityPoolEntity.Address)} IN @{nameof(SqlParams.LiquidityPools)}";
        }

        if (request.Cursor.MiningStatus == MiningStatusFilter.Active || request.Cursor.MiningStatus == MiningStatusFilter.Inactive)
        {
            var filterOperator = request.Cursor.MiningStatus == MiningStatusFilter.Active ? "<" : ">=";
            whereFilter += whereFilter == "" ? " WHERE" : " AND";
            whereFilter += $" ({BlockHeightQuery}) {filterOperator} pm.{nameof(MiningPoolEntity.MiningPeriodEndBlock)}";
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

        var orderBy = $" ORDER BY pm.{nameof(MiningPoolEntity.Id)} {direction}";

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
        internal SqlParams(ulong miningPoolId, IEnumerable<Address> liquidityPools)
        {
            MiningPoolId = miningPoolId;
            LiquidityPools = liquidityPools.Select(pool => pool.ToString());
        }

        public ulong MiningPoolId { get; }
        public IEnumerable<string> LiquidityPools { get; }
    }
}