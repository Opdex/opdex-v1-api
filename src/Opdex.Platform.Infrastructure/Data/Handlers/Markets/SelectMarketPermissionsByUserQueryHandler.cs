using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets
{
    public class SelectMarketPermissionsByUserQueryHandler : IRequestHandler<SelectMarketPermissionsByUserQuery, IEnumerable<Permissions>>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(MarketPermissionEntity.Permission)}
            FROM market_permission
            WHERE {nameof(MarketPermissionEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
                AND {nameof(MarketPermissionEntity.User)} = @{nameof(SqlParams.User)}
                AND {nameof(MarketPermissionEntity.IsAuthorized)} = true
            LIMIT 4;";

        private readonly IDbContext _context;

        public SelectMarketPermissionsByUserQueryHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Permissions>> Handle(SelectMarketPermissionsByUserQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.MarketId, request.User);

            var command = DatabaseQuery.Create(SqlCommand, queryParams, cancellationToken);

            return await _context.ExecuteQueryAsync<Permissions>(command);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long marketId, string user)
            {
                MarketId = marketId;
                User = user;
            }

            public long MarketId { get; }
            public string User { get; }
        }
    }
}