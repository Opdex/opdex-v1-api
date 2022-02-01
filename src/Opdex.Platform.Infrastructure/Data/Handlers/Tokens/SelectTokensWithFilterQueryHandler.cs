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
using System.Text;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens;

public class SelectTokensWithFilterQueryHandler : IRequestHandler<SelectTokensWithFilterQuery, IEnumerable<Token>>
{
    private const string Split = "Split";

    private const string TableJoins = "{TableJoins}";
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        $@"SELECT DISTINCT
                t.{nameof(TokenEntity.Id)},
                t.{nameof(TokenEntity.Address)},
                t.{nameof(TokenEntity.Name)},
                t.{nameof(TokenEntity.Symbol)},
                t.{nameof(TokenEntity.Decimals)},
                t.{nameof(TokenEntity.Sats)},
                t.{nameof(TokenEntity.TotalSupply)},
                MAX(t.{nameof(TokenEntity.CreatedBlock)}) AS {nameof(TokenEntity.CreatedBlock)},
                MAX(t.{nameof(TokenEntity.ModifiedBlock)}) AS {nameof(TokenEntity.ModifiedBlock)},
                true as {Split},
                ROUND(AVG(ts.{nameof(TokenSummaryEntity.PriceUsd)}), 8) AS Summary{nameof(TokenSummaryEntity.PriceUsd)},
                ROUND(AVG(ts.{nameof(TokenSummaryEntity.DailyPriceChangePercent)}), 8) AS Summary{nameof(TokenSummaryEntity.DailyPriceChangePercent)},
                MAX(ts.{nameof(TokenSummaryEntity.CreatedBlock)}) AS Summary{nameof(TokenSummaryEntity.CreatedBlock)},
                MAX(ts.{nameof(TokenSummaryEntity.ModifiedBlock)}) AS Summary{nameof(TokenSummaryEntity.ModifiedBlock)}
            FROM token t
            LEFT JOIN token_summary ts
                ON ts.{nameof(TokenSummaryEntity.TokenId)} = t.{nameof(TokenEntity.Id)}
            {TableJoins}
            {WhereFilter}
            GROUP BY t.{nameof(TokenEntity.Id)}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string OrderBySort = "{OrderBySort}";

    private static readonly string PagingQuery =
        @$"SELECT
                {nameof(TokenEntity.Id)},
                {nameof(TokenEntity.Address)},
                {nameof(TokenEntity.Name)},
                {nameof(TokenEntity.Symbol)},
                {nameof(TokenEntity.Decimals)},
                {nameof(TokenEntity.Sats)},
                {nameof(TokenEntity.TotalSupply)},
                {nameof(TokenEntity.CreatedBlock)},
                {nameof(TokenEntity.ModifiedBlock)},
                {Split},
                Summary{nameof(TokenSummaryEntity.PriceUsd)} AS {nameof(TokenSummaryEntity.PriceUsd)},
                Summary{nameof(TokenSummaryEntity.DailyPriceChangePercent)} AS {nameof(TokenSummaryEntity.DailyPriceChangePercent)},
                Summary{nameof(TokenSummaryEntity.CreatedBlock)} AS {nameof(TokenSummaryEntity.CreatedBlock)},
                Summary{nameof(TokenSummaryEntity.ModifiedBlock)} AS {nameof(TokenSummaryEntity.ModifiedBlock)}
            FROM ({InnerQuery}) r";
    private static readonly string PagingBackwardQuery = @$"{PagingQuery} {OrderBySort}";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTokensWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<Token>> Handle(SelectTokensWithFilterQuery request, CancellationToken cancellationToken)
    {
        var sqlParams = new SqlParams(request.MarketId,request.Cursor.Pointer, request.Cursor.Keyword,
                                      request.Cursor.Tokens, request.Cursor.TokenAttributes,
                                      request.Cursor.ExternalChains.Select(c => (ExternalChainType)c));

        var query = DatabaseQuery.Create(QueryBuilder(request), sqlParams, cancellationToken);

        var tokens = await _context.ExecuteQueryAsync<TokenEntity, TokenSummaryEntity, Token>(query,
            (tokenEntity, summary) =>
            {
                var token = _mapper.Map<Token>(tokenEntity);
                // if there is no summary CreatedBlock will be 0
                if (summary.CreatedBlock > 0) token.SetSummary(_mapper.Map<TokenSummary>(summary));
                return token;
            }, splitOn: Split);

        return tokens;
    }

    private static string QueryBuilder(SelectTokensWithFilterQuery request)
    {
        var filterOnMarket = request.MarketId > 0;

        var tableJoins = new StringBuilder();
        var whereFilterBuilder = new StringBuilder();

        if (filterOnMarket) whereFilterBuilder.Append($" WHERE ts.{nameof(TokenSummaryEntity.MarketId)} = @{nameof(SqlParams.MarketId)}");

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

            whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
            var orderByFilter = request.Cursor.OrderBy switch
            {
                TokenOrderByType.Name => $" (t.{nameof(TokenEntity.Name)}, {finisher}",
                TokenOrderByType.Symbol => $" (t.{nameof(TokenEntity.Symbol)}, {finisher}",
                TokenOrderByType.PriceUsd => $" (ts.{nameof(TokenSummaryEntity.PriceUsd)}, {finisher}",
                TokenOrderByType.DailyPriceChangePercent => $" (ts.{nameof(TokenSummaryEntity.DailyPriceChangePercent)}, {finisher}",
                _ => $" t.{nameof(TokenEntity.Id)} {sortOperator} @{nameof(SqlParams.TokenId)}"
            };
            whereFilterBuilder.Append(orderByFilter);
        }

        if (request.Cursor.Tokens.Any())
        {
            whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
            whereFilterBuilder.Append($" t.{nameof(TokenEntity.Address)} IN @{nameof(SqlParams.Tokens)}");
        }

        if (request.Cursor.TokenAttributes.Any())
        {
            tableJoins.Append($" LEFT JOIN token_attribute ta ON ta.{nameof(TokenAttributeEntity.TokenId)} = t.{nameof(TokenEntity.Id)}");
            whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
            whereFilterBuilder.Append($" ta.{nameof(TokenAttributeEntity.AttributeTypeId)} IN @{nameof(SqlParams.TokenAttributes)}");
        }

        if (request.Cursor.NativeChains.Any())
        {
            tableJoins.Append($" LEFT JOIN token_chain tc ON tc.{nameof(TokenChainEntity.TokenId)} = t.{nameof(TokenEntity.Id)}");

            whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
            whereFilterBuilder.Append('(');
            var filterOnNativeToCirrus = request.Cursor.NativeChains.Contains(ChainType.Cirrus);
            if (filterOnNativeToCirrus)
            {
                whereFilterBuilder.Append($" tc.{nameof(TokenChainEntity.NativeChainTypeId)} IS NULL");
            }

            if (request.Cursor.ExternalChains.Any())
            {
                if (filterOnNativeToCirrus) whereFilterBuilder.Append(" OR");
                whereFilterBuilder.Append($" tc.{nameof(TokenChainEntity.NativeChainTypeId)} IN @{nameof(SqlParams.ExternalChainTypes)}");
            }
            whereFilterBuilder.Append(')');
        }

        if (request.Cursor.Keyword.HasValue())
        {
            whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
            whereFilterBuilder.Append(@$" (t.{nameof(TokenEntity.Name)} LIKE CONCAT('%', @{nameof(SqlParams.Keyword)}, '%') OR
                                           t.{nameof(TokenEntity.Symbol)} LIKE CONCAT('%', @{nameof(SqlParams.Keyword)}, '%') OR
                                           t.{nameof(TokenEntity.Address)} LIKE CONCAT('%', @{nameof(SqlParams.Keyword)}, '%'))");
        }

        if (!request.Cursor.IncludeZeroLiquidity)
        {
            whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
            whereFilterBuilder.Append($@" ts.{nameof(TokenSummaryEntity.PriceUsd)} > 0");
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

        var query = SqlQuery
            .Replace(WhereFilter, whereFilterBuilder.ToString())
            .Replace(TableJoins, tableJoins.ToString())
            .Replace(OrderBy, orderBy)
            .Replace(Limit, limit);

        if (request.Cursor.PagingDirection == PagingDirection.Forward)
        {
            return PagingQuery.Replace(InnerQuery, query);
        }

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
            TokenOrderByType.PriceUsd => $" ORDER BY AVG({summaryPrefix}.{nameof(TokenSummaryEntity.PriceUsd)}) {direction}, {tokenIdWithDirection}",
            TokenOrderByType.DailyPriceChangePercent => $" ORDER BY AVG({summaryPrefix}.{nameof(TokenSummaryEntity.DailyPriceChangePercent)}) {direction}, {tokenIdWithDirection}",
            _ => $" ORDER BY {tokenIdWithDirection}"
        };
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong marketId, (string, ulong) pointer, string keyword, IEnumerable<Address> tokens, IEnumerable<TokenAttributeFilter> tokenAttributes, IEnumerable<ExternalChainType> externalChainTypes)
        {
            MarketId = marketId;
            OrderByValue = pointer.Item1;
            TokenId = pointer.Item2;
            Keyword = keyword;
            Tokens = tokens.Select(token => token.ToString());
            TokenAttributes = tokenAttributes;
            ExternalChainTypes = externalChainTypes;
        }

        public ulong MarketId { get; }
        public string OrderByValue { get; }
        public ulong TokenId { get; }
        public string Keyword { get; }
        public IEnumerable<string> Tokens { get; }
        public IEnumerable<TokenAttributeFilter> TokenAttributes { get; }
        public IEnumerable<ExternalChainType> ExternalChainTypes { get; }
    }
}
