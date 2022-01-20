using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Deployers;

public class PersistDeployerCommandHandler : IRequestHandler<PersistDeployerCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO market_deployer (
                {nameof(DeployerEntity.Address)},
                {nameof(DeployerEntity.PendingOwner)},
                {nameof(DeployerEntity.Owner)},
                {nameof(DeployerEntity.IsActive)},
                {nameof(DeployerEntity.CreatedBlock)},
                {nameof(DeployerEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(DeployerEntity.Address)},
                @{nameof(DeployerEntity.PendingOwner)},
                @{nameof(DeployerEntity.Owner)},
                @{nameof(DeployerEntity.IsActive)},
                @{nameof(DeployerEntity.CreatedBlock)},
                @{nameof(DeployerEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE market_deployer
                SET
                    {nameof(DeployerEntity.PendingOwner)} = @{nameof(DeployerEntity.PendingOwner)},
                    {nameof(DeployerEntity.Owner)} = @{nameof(DeployerEntity.Owner)},
                    {nameof(DeployerEntity.IsActive)} = @{nameof(DeployerEntity.IsActive)},
                    {nameof(DeployerEntity.ModifiedBlock)} = @{nameof(DeployerEntity.ModifiedBlock)}
                WHERE {nameof(DeployerEntity.Id)} = @{nameof(DeployerEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistDeployerCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistDeployerCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistDeployerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<DeployerEntity>(request.Deployer);

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
                { "Contract", request.Deployer.Address },
                { "BlockHeight", request.Deployer.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, $"Failure persisting market deployer.");
            }

            return 0;
        }
    }
}