using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses
{
    public class SelectAddressBalancesWithFilterQueryHandler : IRequestHandler<SelectAddressBalancesWithFilterQuery, IEnumerable<AddressBalance>>
    {
        private const string TableJoins = "{TableJoins}";
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

        private static readonly string SqlQuery =
            @$"SELECT
                ab.{nameof(AddressBalanceEntity.Id)},
                ab.{nameof(AddressBalanceEntity.TokenId)},
                ab.{nameof(AddressBalanceEntity.Owner)},
                ab.{nameof(AddressBalanceEntity.Balance)},
                ab.{nameof(AddressBalanceEntity.CreatedBlock)},
                ab.{nameof(AddressBalanceEntity.ModifiedBlock)}
            FROM address_balance ab
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAddressBalancesWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<AddressBalance>> Handle(SelectAddressBalancesWithFilterQuery request, CancellationToken cancellationTransaction)
        {
            var balanceId = request.Cursor.Pointer;

            var queryParams = new SqlParams(balanceId, request.Address, request.Cursor.Tokens, request.Cursor.IncludeLpTokens);

            var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationTransaction);

            var results = await _context.ExecuteQueryAsync<AddressBalanceEntity>(query);

            // re-sort back into correct order
            if (request.Cursor.PagingDirection == PagingDirection.Backward)
            {
                results = request.Cursor.SortDirection == SortDirectionType.ASC
                    ? results.OrderBy(t => t.Id)
                    : results.OrderByDescending(t => t.Id);
            }

            return _mapper.Map<IEnumerable<AddressBalance>>(results);
        }

        private static string QueryBuilder(SelectAddressBalancesWithFilterQuery request)
        {
            var whereFilter = $"WHERE ab.{nameof(AddressBalanceEntity.Owner)} = @{nameof(SqlParams.Wallet)}";
            var tableJoins = string.Empty;

            if (request.Cursor.Tokens.Any() || !request.Cursor.IncludeLpTokens)
            {
                tableJoins += $" JOIN token t ON t.{nameof(TokenEntity.Id)} = ab.{nameof(AddressBalanceEntity.TokenId)}";
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

                whereFilter += $" AND ab.{nameof(AddressBalanceEntity.Id)} {sortOperator} @{nameof(SqlParams.BalanceId)}";
            }

            if (request.Cursor.Tokens.Any())
            {
                whereFilter += $" AND t.{nameof(TokenEntity.Address)} IN @{nameof(SqlParams.Tokens)}";
            }

            if (!request.Cursor.IncludeLpTokens)
            {
                whereFilter += $" AND t.{nameof(TokenEntity.IsLpt)} = @{nameof(SqlParams.IncludeLpTokens)}";
            }

            if (!request.Cursor.IncludeZeroBalances)
            {
                whereFilter += $" AND ab.{nameof(AddressBalanceEntity.Balance)} != '0'";
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

            var orderBy = $" ORDER BY ab.{nameof(AddressBalanceEntity.Id)} {direction}";

            var limit = $" LIMIT {request.Cursor.Limit + 1}";

            return SqlQuery
                .Replace(TableJoins, tableJoins)
                .Replace(WhereFilter, whereFilter)
                .Replace(OrderBy, orderBy)
                .Replace(Limit, limit);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long balanceId, string wallet, IEnumerable<string> tokens, bool includeLpTokens)
            {
                BalanceId = balanceId;
                Wallet = wallet;
                Tokens = tokens;
                IncludeLpTokens = includeLpTokens;
            }

            public long BalanceId { get; }
            public string Wallet { get; }
            public IEnumerable<string> Tokens { get; }
            public bool IncludeLpTokens { get; }
        }
    }
}