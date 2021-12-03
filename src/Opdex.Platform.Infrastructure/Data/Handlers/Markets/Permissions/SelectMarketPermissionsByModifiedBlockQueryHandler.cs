using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Permissions;

public class SelectMarketPermissionsByModifiedBlockQueryHandler
    : IRequestHandler<SelectMarketPermissionsByModifiedBlockQuery, IEnumerable<MarketPermission>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(MarketPermissionEntity.Id)},
                {nameof(MarketPermissionEntity.MarketId)},
                {nameof(MarketPermissionEntity.User)},
                {nameof(MarketPermissionEntity.Permission)},
                {nameof(MarketPermissionEntity.IsAuthorized)},
                {nameof(MarketPermissionEntity.Blame)},
                {nameof(MarketPermissionEntity.CreatedBlock)},
                {nameof(MarketPermissionEntity.ModifiedBlock)}
            FROM market_permission
            WHERE {nameof(MarketPermissionEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectMarketPermissionsByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<MarketPermission>> Handle(SelectMarketPermissionsByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

        var result = await _context.ExecuteQueryAsync<MarketPermissionEntity>(query);

        return _mapper.Map<IEnumerable<MarketPermission>>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong modifiedBlock)
        {
            ModifiedBlock = modifiedBlock;
        }

        public ulong ModifiedBlock { get; }
    }
}