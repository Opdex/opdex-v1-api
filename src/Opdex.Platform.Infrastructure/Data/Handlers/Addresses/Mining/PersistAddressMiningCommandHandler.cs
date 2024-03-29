using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Mining;

public class PersistAddressMiningCommandHandler : IRequestHandler<PersistAddressMiningCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO address_mining (
                {nameof(AddressMiningEntity.Id)},
                {nameof(AddressMiningEntity.MiningPoolId)},
                {nameof(AddressMiningEntity.Owner)},
                {nameof(AddressMiningEntity.Balance)},
                {nameof(AddressMiningEntity.CreatedBlock)},
                {nameof(AddressMiningEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(AddressMiningEntity.Id)},
                @{nameof(AddressMiningEntity.MiningPoolId)},
                @{nameof(AddressMiningEntity.Owner)},
                @{nameof(AddressMiningEntity.Balance)},
                @{nameof(AddressMiningEntity.CreatedBlock)},
                @{nameof(AddressMiningEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE address_mining
                SET
                    {nameof(AddressMiningEntity.Balance)} = @{nameof(AddressMiningEntity.Balance)},
                    {nameof(AddressMiningEntity.ModifiedBlock)} = @{nameof(AddressMiningEntity.ModifiedBlock)}
                WHERE {nameof(AddressMiningEntity.Id)} = @{nameof(AddressMiningEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistAddressMiningCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistAddressMiningCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistAddressMiningCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<AddressMiningEntity>(request.AddressMining);

            var isUpdate = entity.Id >= 1;

            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return isUpdate ? entity.Id : result;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["MiningPoolId"] = request.AddressMining.MiningPoolId,
                ["Owner"] = request.AddressMining.Owner,
                ["Balance"] = request.AddressMining.Balance,
                ["BlockHeight"] = request.AddressMining.ModifiedBlock
            }))
            {
                _logger.LogError(ex, $"Failure persisting mining position.");
            }

            return 0;
        }
    }
}
