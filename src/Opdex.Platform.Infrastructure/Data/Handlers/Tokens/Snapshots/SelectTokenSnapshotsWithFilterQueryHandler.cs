using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots;

public class SelectTokenSnapshotsWithFilterQueryHandler : IRequestHandler<SelectTokenSnapshotsWithFilterQuery, IEnumerable<TokenSnapshot>>
{
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(TokenSnapshotEntity.Id)},
                {nameof(TokenSnapshotEntity.TokenId)},
                {nameof(TokenSnapshotEntity.MarketId)},
                {nameof(TokenSnapshotEntity.Details)},
                {nameof(TokenSnapshotEntity.StartDate)},
                {nameof(TokenSnapshotEntity.EndDate)},
                {nameof(TokenSnapshotEntity.SnapshotTypeId)},
                {nameof(TokenSnapshotEntity.ModifiedDate)}
            FROM token_snapshot
            WHERE
                {nameof(TokenSnapshotEntity.TokenId)} = @{nameof(SqlParams.TokenId)} AND
                {nameof(TokenSnapshotEntity.MarketId)} = @{nameof(SqlParams.MarketId)} AND
                {nameof(TokenSnapshotEntity.StartDate)} BETWEEN @{nameof(SqlParams.StartDate)} AND @{nameof(SqlParams.EndDate)} AND
                {nameof(TokenSnapshotEntity.SnapshotTypeId)} = @{nameof(SqlParams.SnapshotTypeId)}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string SortDirection = "{SortDirection}";
    private static readonly string PagingBackwardQuery =
        @$"SELECT * FROM ({InnerQuery}) results ORDER BY results.{nameof(TokenSnapshotEntity.StartDate)} {SortDirection};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTokenSnapshotsWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<TokenSnapshot>> Handle(SelectTokenSnapshotsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.TokenId, request.MarketId, request.Cursor.StartTime, request.Cursor.EndTime,
                                        _mapper.Map<SnapshotType>(request.Cursor.Interval), request.Cursor.Pointer);

        var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

        var result = await _context.ExecuteQueryAsync<TokenSnapshotEntity>(query);

        return _mapper.Map<IEnumerable<TokenSnapshot>>(result);
    }

    private static string QueryBuilder(SelectTokenSnapshotsWithFilterQuery request)
    {
        var whereFilter = string.Empty;

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

            whereFilter += $" AND ({nameof(TokenSnapshotEntity.StartDate)}, {nameof(TokenSnapshotEntity.Id)}) {sortOperator} (@{nameof(SqlParams.DatePointer)}, @{nameof(SqlParams.IdPointer)})";
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

        var orderBy = $" ORDER BY {nameof(TokenSnapshotEntity.StartDate)} {direction}, {nameof(TokenSnapshotEntity.Id)} {direction}";

        var limit = $" LIMIT {request.Cursor.Limit + 1}";

        var query = SqlQuery.Replace(WhereFilter, whereFilter)
            .Replace(OrderBy, orderBy)
            .Replace(Limit, limit);

        if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";

        // re-sort back into requested order
        return PagingBackwardQuery.Replace(InnerQuery, query)
            .Replace(SortDirection, Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection));
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong tokenId, ulong marketId, DateTime startDate, DateTime endDate, SnapshotType snapshotTypeId, (DateTime, ulong) pointer)
        {
            TokenId = tokenId;
            MarketId = marketId;
            StartDate = startDate;
            EndDate = endDate;
            DatePointer = pointer.Item1;
            IdPointer = pointer.Item2;
            SnapshotTypeId = (int)snapshotTypeId;
        }

        public ulong TokenId { get; }
        public ulong MarketId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public DateTime DatePointer { get; }
        public ulong IdPointer { get; }
        public int SnapshotTypeId { get; }
    }
}