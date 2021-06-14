using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Markets
{
    public class PersistMarketPermissionCommandHandler : IRequestHandler<PersistMarketPermissionCommand, long>
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
              SELECT LAST_INSERT_ID();";

        private static readonly string UpdateSqlCommand =
            $@"UPDATE market 
                SET 
                    {nameof(MarketPermissionEntity.IsAuthorized)} = @{nameof(MarketPermissionEntity.IsAuthorized)},
                    {nameof(MarketPermissionEntity.Blame)} = @{nameof(MarketPermissionEntity.Blame)},
                    {nameof(MarketPermissionEntity.ModifiedBlock)} = @{nameof(MarketPermissionEntity.ModifiedBlock)}
                WHERE {nameof(MarketPermissionEntity.Id)} = @{nameof(MarketPermissionEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PersistMarketPermissionCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMarketPermissionCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistMarketPermissionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<MarketPermissionEntity>(request.MarketPermission);

                var isUpdate = entity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

                var command = DatabaseQuery.Create(sql, entity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<long>(command);

                return isUpdate ? entity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting {request.MarketPermission}.");

                return 0;
            }
        }
    }
}