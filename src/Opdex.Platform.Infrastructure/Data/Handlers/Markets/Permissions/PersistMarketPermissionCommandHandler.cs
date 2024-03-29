using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets.Permissions;

public class PersistMarketPermissionCommandHandler : IRequestHandler<PersistMarketPermissionCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO market_permission (
                {nameof(MarketPermissionEntity.MarketId)},
                {nameof(MarketPermissionEntity.User)},
                {nameof(MarketPermissionEntity.Permission)},
                {nameof(MarketPermissionEntity.IsAuthorized)},
                {nameof(MarketPermissionEntity.Blame)},
                {nameof(MarketPermissionEntity.CreatedBlock)},
                {nameof(MarketPermissionEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(MarketPermissionEntity.MarketId)},
                @{nameof(MarketPermissionEntity.User)},
                @{nameof(MarketPermissionEntity.Permission)},
                @{nameof(MarketPermissionEntity.IsAuthorized)},
                @{nameof(MarketPermissionEntity.Blame)},
                @{nameof(MarketPermissionEntity.CreatedBlock)},
                @{nameof(MarketPermissionEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE market_permission
                SET
                    {nameof(MarketPermissionEntity.IsAuthorized)} = @{nameof(MarketPermissionEntity.IsAuthorized)},
                    {nameof(MarketPermissionEntity.Blame)} = @{nameof(MarketPermissionEntity.Blame)},
                    {nameof(MarketPermissionEntity.ModifiedBlock)} = @{nameof(MarketPermissionEntity.ModifiedBlock)}
                WHERE {nameof(MarketPermissionEntity.Id)} = @{nameof(MarketPermissionEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistMarketPermissionCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMarketPermissionCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistMarketPermissionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<MarketPermissionEntity>(request.MarketPermission);

            var isUpdate = entity.Id >= 1;

            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return isUpdate ? entity.Id : result;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "MarketId", request.MarketPermission.MarketId },
                { "User", request.MarketPermission.User },
                { "Permission", request.MarketPermission.Permission },
                { "BlockHeight", request.MarketPermission.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, $"Failure persisting market permission.");
            }

            return 0;
        }
    }
}