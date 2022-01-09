using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults;

public class SelectVaultsWithFilterQueryHandler : IRequestHandler<SelectVaultsWithFilterQuery, IEnumerable<Vault>>
{
    private const string TableJoins = "{TableJoins}";
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        @$"SELECT
                v.{nameof(VaultEntity.Id)},
                v.{nameof(VaultEntity.TokenId)},
                v.{nameof(VaultEntity.Address)},
                v.{nameof(VaultEntity.UnassignedSupply)},
                v.{nameof(VaultEntity.ProposedSupply)},
                v.{nameof(VaultEntity.VestingDuration)},
                v.{nameof(VaultEntity.TotalPledgeMinimum)},
                v.{nameof(VaultEntity.TotalVoteMinimum)},
                v.{nameof(VaultEntity.CreatedBlock)},
                v.{nameof(VaultEntity.ModifiedBlock)}
            FROM vault v
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string SortDirection = "{SortDirection}";

    private static readonly string PagingBackwardQuery =
        @$"SELECT * FROM ({InnerQuery}) results ORDER BY results.{nameof(VaultEntity.Id)} {SortDirection};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultsWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<Vault>> Handle(SelectVaultsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.Cursor.Pointer, request.Cursor.LockedToken);

        var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<VaultEntity>(query);

        return _mapper.Map<IEnumerable<Vault>>(results);
    }

    private static string QueryBuilder(SelectVaultsWithFilterQuery request)
    {
        var filterOnLockedToken = request.Cursor.LockedToken != Address.Empty;

        var whereFilterBuilder = new StringBuilder();
        var tableJoinBuilder = new StringBuilder();

        if (!request.Cursor.IsFirstRequest)
        {
            var sortOperator = request.Cursor.PagingDirection switch
            {
                // going forward in ascending order, use greater than
                PagingDirection.Forward when request.Cursor.SortDirection == SortDirectionType.ASC => ">",
                // going backward in descending order, use greater than
                PagingDirection.Backward when request.Cursor.SortDirection == SortDirectionType.DESC => ">",
                // going forward in descending order, use less than or equal to
                PagingDirection.Forward when request.Cursor.SortDirection == SortDirectionType.DESC => "<",
                // going backward in ascending order, use less than
                PagingDirection.Backward when request.Cursor.SortDirection == SortDirectionType.ASC => "<",
                _ => throw new ArgumentException("Unrecognised paging and sorting direction.", nameof(request))
            };

            whereFilterBuilder.Append($" WHERE v.{nameof(VaultEntity.Id)} {sortOperator} @{nameof(SqlParams.Pointer)}");
        }

        if (filterOnLockedToken)
        {
            tableJoinBuilder.Append( $" JOIN token t ON t.{nameof(TokenEntity.Id)} = v.{nameof(VaultEntity.TokenId)}");
            whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
            whereFilterBuilder.Append($" t.{nameof(TokenEntity.Address)} = @{nameof(SqlParams.LockedToken)}");
        }

        // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
        string direction = request.Cursor.PagingDirection switch
        {
            PagingDirection.Backward => request.Cursor.SortDirection == SortDirectionType.DESC
                ? nameof(SortDirectionType.ASC)
                : nameof(SortDirectionType.DESC),
            PagingDirection.Forward => Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection),
            _ => throw new ArgumentException("Unrecognised paging direction.", nameof(request))
        };

        var orderBy = $" ORDER BY v.{nameof(VaultEntity.Id)} {direction}";

        var limit = $" LIMIT {request.Cursor.Limit + 1}";

        var query = SqlQuery.Replace(TableJoins, tableJoinBuilder.ToString())
                                 .Replace(WhereFilter, whereFilterBuilder.ToString())
                                 .Replace(OrderBy, orderBy)
                                 .Replace(Limit, limit);

        return request.Cursor.PagingDirection switch
        {
            PagingDirection.Forward => $"{query};",
            _ => PagingBackwardQuery.Replace(InnerQuery, query)
                                    .Replace(SortDirection, Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection))
        };
    }

    class SqlParams
    {
        public SqlParams(ulong pointer, Address lockedToken)
        {
            Pointer = pointer;
            LockedToken = lockedToken;
        }

        public ulong Pointer { get; }
        public Address LockedToken { get; }
    }
}
