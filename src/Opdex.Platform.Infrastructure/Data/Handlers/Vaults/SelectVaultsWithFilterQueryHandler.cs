using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults
{
    public class SelectVaultsWithFilterQueryHandler : IRequestHandler<SelectVaultsWithFilterQuery, IEnumerable<Vault>>
    {
        private const string TableJoins = "{TableJoins}";
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

        private static readonly string SqlQuery =
            @$"SELECT
                v.{nameof(VaultEntity.Id)},
                v.{nameof(VaultEntity.Address)},
                v.{nameof(VaultEntity.TokenId)},
                v.{nameof(VaultEntity.PendingOwner)},
                v.{nameof(VaultEntity.Owner)},
                v.{nameof(VaultEntity.Genesis)},
                v.{nameof(VaultEntity.UnassignedSupply)},
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
            @$"SELECT * FROM ({InnerQuery}) results ORDER BY results.{nameof(VaultCertificateEntity.Id)} {SortDirection};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectVaultsWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Vault>> Handle(SelectVaultsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var vaultId = request.Cursor.Pointer;

            var queryParams = new SqlParams(vaultId, request.Cursor.LockedToken);

            var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

            var results = await _context.ExecuteQueryAsync<VaultEntity>(query);

            return _mapper.Map<IEnumerable<Vault>>(results);
        }

        private static string QueryBuilder(SelectVaultsWithFilterQuery request)
        {
            var whereFilter = string.Empty;
            var tableJoins = string.Empty;

            var filterOnLockedToken = request.Cursor.LockedToken != Address.Empty;

            if (filterOnLockedToken) tableJoins += $" JOIN token t ON t.{nameof(TokenEntity.Id)} = v.{nameof(VaultEntity.TokenId)}";

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

                whereFilter += $" WHERE v.{nameof(VaultEntity.Id)} {sortOperator} @{nameof(SqlParams.VaultId)}";
            }

            if (filterOnLockedToken)
            {
                whereFilter += whereFilter == "" ? " WHERE" : " AND";
                whereFilter += $" t.{nameof(TokenEntity.Address)} = @{nameof(SqlParams.LockedToken)}";
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

            var orderBy = $" ORDER BY v.{nameof(VaultEntity.Id)} {direction}";

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
            internal SqlParams(ulong vaultId, Address lockedToken)
            {
                VaultId = vaultId;
                LockedToken = lockedToken;
            }

            public ulong VaultId { get; }
            public Address LockedToken { get; }
        }
    }
}
