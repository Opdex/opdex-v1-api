using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Permissions;

public class SelectMarketPermissionsByUserQueryHandler : IRequestHandler<SelectMarketPermissionsByUserQuery, IEnumerable<MarketPermissionType>>
{
    private static readonly string SqlCommand =
        $@"SELECT
                {nameof(MarketPermissionEntity.Permission)}
            FROM market_permission
            WHERE {nameof(MarketPermissionEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
                AND {nameof(MarketPermissionEntity.User)} = @{nameof(SqlParams.User)}
                AND {nameof(MarketPermissionEntity.IsAuthorized)} = true
            LIMIT 4;".RemoveExcessWhitespace();

    private readonly IDbContext _context;

    public SelectMarketPermissionsByUserQueryHandler(IDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<MarketPermissionType>> Handle(SelectMarketPermissionsByUserQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.MarketId, request.User);

        var command = DatabaseQuery.Create(SqlCommand, queryParams, cancellationToken);

        return await _context.ExecuteQueryAsync<MarketPermissionType>(command);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong marketId, Address user)
        {
            MarketId = marketId;
            User = user;
        }

        public ulong MarketId { get; }
        public Address User { get; }
    }
}