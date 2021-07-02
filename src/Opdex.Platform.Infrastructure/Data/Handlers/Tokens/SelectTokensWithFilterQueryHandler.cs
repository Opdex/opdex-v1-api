using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System.Linq;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens
{
    public class SelectTokensWithFilterQueryHandler : IRequestHandler<SelectTokensWithFilterQuery, IEnumerable<Token>>
    {
        private const string TableJoins = "{TableJoins}";
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

        private static readonly string SqlCommand =
            $@"SELECT
                t.{nameof(TokenEntity.Id)},
                t.{nameof(TokenEntity.IsLpt)},
                t.{nameof(TokenEntity.Address)},
                t.{nameof(TokenEntity.Name)},
                t.{nameof(TokenEntity.Symbol)},
                t.{nameof(TokenEntity.Decimals)},
                t.{nameof(TokenEntity.Sats)},
                t.{nameof(TokenEntity.TotalSupply)},
                t.{nameof(TokenEntity.CreatedBlock)},
                t.{nameof(TokenEntity.ModifiedBlock)}
            FROM token t
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public SelectTokensWithFilterQueryHandler(IDbContext context, IMapper mapper,
            ILogger<SelectTokensWithFilterQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Token>> Handle(SelectTokensWithFilterQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(QueryBuilder(request), new SqlParams(request.MarketId, request.LpToken, request.Tokens), cancellationToken);

            var tokenEntities =  await _context.ExecuteQueryAsync<TokenEntity>(command);

            return _mapper.Map<IEnumerable<Token>>(tokenEntities);
        }

        private static string QueryBuilder(SelectTokensWithFilterQuery request)
        {
            var whereFilter = string.Empty;
            var tableJoins = string.Empty;

            // Tokens Filter
            if (request.Tokens?.Any() == true)
            {
                whereFilter += $" WHERE t.{nameof(TokenEntity.Address)} IN @{nameof(SqlParams.Tokens)}";
            }

            // LptFilter
            if (request.LpToken.HasValue)
            {
                var inclusive = whereFilter.HasValue() ? "AND" : "WHERE";
                whereFilter += $" {inclusive} t.{nameof(TokenEntity.IsLpt)} = @{nameof(SqlParams.IsLpt)}";
            }

            // Sort Found Pools
            var orderBy = OrderByBuilder(request.SortBy, request.OrderBy);
            if (orderBy.HasValue() && orderBy.Contains("ts."))
            {
                tableJoins += $@" JOIN token_snapshot ts ON
                                ts.{nameof(TokenSnapshotEntity.TokenId)} = t.{nameof(TokenEntity.Id)}
                                AND ts.{nameof(TokenSnapshotEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
                                AND ts.{nameof(TokenSnapshotEntity.EndDate)} > UTC_TIMESTAMP()
                                AND ts.{nameof(TokenSnapshotEntity.SnapshotTypeId)} = {(int)SnapshotType.Daily}";
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

            orderRequest = orderRequest.HasValue() && (orderRequest.EqualsIgnoreCase("ASC") || orderRequest.EqualsIgnoreCase("DESC"))
                ? orderRequest.ToUpper()
                : "DESC";

            return sortRequest switch
            {
                "Price" => $" ORDER BY CAST(JSON_EXTRACT(ts.Details, '$.Close') as Decimal) {orderRequest}",
                "Name" => $" ORDER BY t.{nameof(TokenEntity.Name)} {orderRequest}",
                "Symbol" => $" ORDER BY t.{nameof(TokenEntity.Symbol)} {orderRequest}",
                _ => throw new ArgumentOutOfRangeException(nameof(sortRequest), "Invalid token sort type.")
            };
        }

        private static string LimitBuilder(uint skip, uint take)
        {
            return skip == 0 && take == 0 ? string.Empty : $" LIMIT {skip}, {take}";
        }

        private sealed class SqlParams
        {
            internal SqlParams(long marketId, bool? isLpt, IEnumerable<string> tokens)
            {
                MarketId = marketId;
                IsLpt = isLpt;
                Tokens = tokens;
            }

            public long MarketId { get; }
            public bool? IsLpt { get; }
            public IEnumerable<string> Tokens { get; }
        }
    }
}
