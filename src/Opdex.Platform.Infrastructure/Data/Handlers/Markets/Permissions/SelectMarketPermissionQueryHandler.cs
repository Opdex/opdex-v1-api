using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Permissions
{
    public class SelectMarketPermissionQueryHandler : IRequestHandler<SelectMarketPermissionQuery, MarketPermission>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(MarketPermissionEntity.Id)},
                {nameof(MarketPermissionEntity.MarketId)},
                {nameof(MarketPermissionEntity.User)},
                {nameof(MarketPermissionEntity.Permission)},
                {nameof(MarketPermissionEntity.IsAuthorized)},
                {nameof(MarketPermissionEntity.Blame)},
                {nameof(MarketPermissionEntity.CreatedBlock)},
                {nameof(MarketPermissionEntity.ModifiedBlock)}
            FROM market_permission
            WHERE {nameof(MarketPermissionEntity.MarketId)} = @{nameof(SqlParams.MarketId)}
                AND {nameof(MarketPermissionEntity.User)} = @{nameof(SqlParams.User)}
                AND {nameof(MarketPermissionEntity.Permission)} = @{nameof(SqlParams.Permission)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectMarketPermissionQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        public async Task<MarketPermission> Handle(SelectMarketPermissionQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.MarketId, request.Address, (int)request.Permission);

            var command = DatabaseQuery.Create(SqlCommand, queryParams, cancellationToken);

            var result = await _context.ExecuteFindAsync<MarketPermissionEntity>(command);

            if (request.FindOrThrow && result is null)
            {
                throw new NotFoundException($"{nameof(MarketPermission)} not found.");
            }

            return result is null ? null : _mapper.Map<MarketPermission>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long marketId, Address user, int permission)
            {
                MarketId = marketId;
                User = user;
                Permission = permission;
            }

            public long MarketId { get; }
            public Address User { get; }
            public int Permission { get; }
        }
    }
}
