using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Deployers
{
    public class PersistDeployerCommandHandler : IRequestHandler<PersistDeployerCommand, long>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO market_deployer (
                {nameof(DeployerEntity.Address)},
                {nameof(DeployerEntity.Owner)},
                {nameof(DeployerEntity.IsActive)},
                {nameof(DeployerEntity.CreatedBlock)},
                {nameof(DeployerEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(DeployerEntity.Address)},
                @{nameof(DeployerEntity.Owner)},
                @{nameof(DeployerEntity.IsActive)},
                @{nameof(DeployerEntity.CreatedBlock)},
                @{nameof(DeployerEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();";

        private static readonly string UpdateSqlCommand =
            $@"UPDATE market_deployer
                SET
                    {nameof(DeployerEntity.Owner)} = @{nameof(DeployerEntity.Owner)},
                    {nameof(DeployerEntity.IsActive)} = @{nameof(DeployerEntity.IsActive)},
                    {nameof(DeployerEntity.ModifiedBlock)} = @{nameof(DeployerEntity.ModifiedBlock)}
                WHERE {nameof(DeployerEntity.Id)} = @{nameof(DeployerEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PersistDeployerCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistDeployerCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistDeployerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<DeployerEntity>(request.Deployer);

                var isUpdate = entity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

                var command = DatabaseQuery.Create(sql, entity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<long>(command);

                return isUpdate ? entity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting {request.Deployer}.");

                return 0;
            }
        }
    }
}
