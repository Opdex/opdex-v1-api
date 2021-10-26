using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System.Linq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens
{
    public class SelectTokensWithFilterQueryHandler : IRequestHandler<SelectTokensWithFilterQuery, IEnumerable<Token>>
    {
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

        private static readonly string SqlQuery =
            $@"SELECT DISTINCT
                t.{nameof(TokenEntity.Id)},
                t.{nameof(TokenEntity.IsLpt)},
                t.{nameof(TokenEntity.Address)},
                t.{nameof(TokenEntity.Name)},
                t.{nameof(TokenEntity.Symbol)},
                t.{nameof(TokenEntity.Decimals)},
                t.{nameof(TokenEntity.Sats)},
                t.{nameof(TokenEntity.TotalSupply)},
                t.{nameof(TokenEntity.CreatedBlock)},
                t.{nameof(TokenEntity.ModifiedBlock)},
                ts.{nameof(TokenSummaryEntity.PriceUsd)},
                ts.{nameof(TokenSummaryEntity.DailyPriceChangePercent)}
            FROM token t
            LEFT JOIN token_attribute ta ON ta.TokenId = t.{nameof(TokenEntity.Id)}
            LEFT JOIN token_summary ts
                ON ts.{nameof(TokenSummaryEntity.MarketId)} = @{nameof(SqlParams.MarketId)} AND
                   ts.{nameof(TokenSummaryEntity.TokenId)} = t.{nameof(TokenEntity.Id)}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

        private const string InnerQuery = "{InnerQuery}";
        private const string OrderBySort = "{OrderBySort}";

        private static readonly string PagingBackwardQuery = @$"SELECT * FROM ({InnerQuery}) r {OrderBySort};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectTokensWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Token>> Handle(SelectTokensWithFilterQuery request, CancellationToken cancellationToken)
        {
            var sqlParams = new SqlParams(request.MarketId, request.Cursor.Pointer, request.Cursor.Keyword, request.Cursor.Tokens);

            var command = DatabaseQuery.Create(QueryBuilder(request), sqlParams, cancellationToken);

            var tokenEntities = await _context.ExecuteQueryAsync<TokenEntity>(command);

            return _mapper.Map<IEnumerable<Token>>(tokenEntities);
        }

        private static string QueryBuilder(SelectTokensWithFilterQuery request)
        {
            var whereFilter = request.MarketId > 0
                ? $" WHERE ts.{nameof(TokenSummaryEntity.MarketId)} = @{nameof(SqlParams.MarketId)}"
                : $" WHERE (ts.{nameof(TokenSummaryEntity.MarketId)} IS NULL OR ts.{nameof(TokenSummaryEntity.MarketId)} = 0)";

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

                var finisher = $"t.{nameof(TokenEntity.Id)}) {sortOperator} (@{nameof(SqlParams.OrderByValue)}, @{nameof(SqlParams.TokenId)})";

                whereFilter += request.Cursor.OrderBy switch
                {
                    TokenOrderByType.Name => $" AND (t.{nameof(TokenEntity.Name)}, {finisher}",
                    TokenOrderByType.Symbol => $" AND (t.{nameof(TokenEntity.Symbol)}, {finisher}",
                    TokenOrderByType.PriceUsd => $" AND (ts.{nameof(TokenSummaryEntity.PriceUsd)}, {finisher}",
                    TokenOrderByType.DailyPriceChangePercent => $" AND (ts.{nameof(TokenSummaryEntity.DailyPriceChangePercent)}, {finisher}",
                    _ => $" AND t.{nameof(TokenEntity.Id)} {sortOperator} @{nameof(SqlParams.TokenId)}"
                };
            }

            if (request.Cursor.Tokens.Any())
            {
                whereFilter += $" AND t.{nameof(TokenEntity.Address)} IN @{nameof(SqlParams.Tokens)}";
            }

            if (request.Cursor.ProvisionalFilter != TokenProvisionalFilter.All)
            {
                var isProvisional = request.Cursor.ProvisionalFilter == TokenProvisionalFilter.Provisional;
                whereFilter += $" AND t.{nameof(TokenEntity.IsLpt)} = {isProvisional}";
            }

            if (request.Cursor.Keyword.HasValue())
            {
                whereFilter += @$" AND (t.{nameof(TokenEntity.Name)} LIKE CONCAT('%', @{nameof(SqlParams.Keyword)}, '%') OR
                                        t.{nameof(TokenEntity.Symbol)} LIKE CONCAT('%', @{nameof(SqlParams.Keyword)}, '%') OR
                                        t.{nameof(TokenEntity.Address)} LIKE CONCAT('%', @{nameof(SqlParams.Keyword)}, '%'))";
            }

            // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
            string direction;

            if (request.Cursor.PagingDirection == PagingDirection.Backward)
                direction = request.Cursor.SortDirection == SortDirectionType.DESC ? nameof(SortDirectionType.ASC) : nameof(SortDirectionType.DESC);
            else
                direction = Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection);

            // Order the rows by the preferred, indexed column or tokenId by default
            var orderBy = OrderByBuilder(request.Cursor.OrderBy, direction, reverse: false);

            var limit = $" LIMIT {request.Cursor.Limit + 1}";

            var query = SqlQuery.Replace(WhereFilter, whereFilter)
                .Replace(OrderBy, orderBy)
                .Replace(Limit, limit);

            if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";

            // re-sort back into requested order
            return PagingBackwardQuery.Replace(InnerQuery, query)
                .Replace(OrderBySort, OrderByBuilder(request.Cursor.OrderBy,
                                                     Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection),
                                                     reverse: true));
        }

        private static string OrderByBuilder(TokenOrderByType cursorOrderBy, string direction, bool reverse)
        {
            var summaryPrefix = reverse ? "r" : "ts";
            var tokenPrefix = reverse ? "r" : "t";

            var tokenIdWithDirection = $"{tokenPrefix}.{nameof(TokenEntity.Id)} {direction}";

            return cursorOrderBy switch
            {
                TokenOrderByType.Name => $" ORDER BY {tokenPrefix}.{nameof(TokenEntity.Name)} {direction}, {tokenIdWithDirection}",
                TokenOrderByType.Symbol => $" ORDER BY {tokenPrefix}.{nameof(TokenEntity.Symbol)} {direction}, {tokenIdWithDirection}",
                TokenOrderByType.PriceUsd => $" ORDER BY {summaryPrefix}.{nameof(TokenSummaryEntity.PriceUsd)} {direction}, {tokenIdWithDirection}",
                TokenOrderByType.DailyPriceChangePercent => $" ORDER BY {summaryPrefix}.{nameof(TokenSummaryEntity.DailyPriceChangePercent)} {direction}, {tokenIdWithDirection}",
                _ => $" ORDER BY {tokenIdWithDirection}"
            };
        }

        private sealed class SqlParams
        {
            internal SqlParams(ulong marketId, (string, ulong) pointer, string keyword, IEnumerable<Address> tokens)
            {
                MarketId = marketId;
                OrderByValue = pointer.Item1;
                TokenId = pointer.Item2;
                Keyword = keyword;
                Tokens = tokens.Select(token => token.ToString());
            }

            public ulong MarketId { get; }
            public string OrderByValue { get; }
            public ulong TokenId { get; }
            public string Keyword { get; }
            public IEnumerable<string> Tokens { get; }
        }
    }
}
