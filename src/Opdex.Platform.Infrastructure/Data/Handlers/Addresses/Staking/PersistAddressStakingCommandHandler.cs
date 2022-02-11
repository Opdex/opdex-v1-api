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

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Staking;

public class PersistAddressStakingCommandHandler : IRequestHandler<PersistAddressStakingCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO address_staking (
                {nameof(AddressStakingEntity.Id)},
                {nameof(AddressStakingEntity.LiquidityPoolId)},
                {nameof(AddressStakingEntity.Owner)},
                {nameof(AddressStakingEntity.Weight)},
                {nameof(AddressStakingEntity.CreatedBlock)},
                {nameof(AddressStakingEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(AddressStakingEntity.Id)},
                @{nameof(AddressStakingEntity.LiquidityPoolId)},
                @{nameof(AddressStakingEntity.Owner)},
                @{nameof(AddressStakingEntity.Weight)},
                @{nameof(AddressStakingEntity.CreatedBlock)},
                @{nameof(AddressStakingEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE address_staking
                SET
                    {nameof(AddressStakingEntity.Weight)} = @{nameof(AddressStakingEntity.Weight)},
                    {nameof(AddressStakingEntity.ModifiedBlock)} = @{nameof(AddressStakingEntity.ModifiedBlock)}
                WHERE {nameof(AddressStakingEntity.Id)} = @{nameof(AddressStakingEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistAddressStakingCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistAddressStakingCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistAddressStakingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<AddressStakingEntity>(request.AddressStaking);

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
                ["LiquidityPoolId"] = request.AddressStaking.LiquidityPoolId,
                ["Owner"] = request.AddressStaking.Owner,
                ["Weight"] = request.AddressStaking.Weight,
                ["BlockHeight"] = request.AddressStaking.ModifiedBlock
            }))
            {
                _logger.LogError(ex, $"Failure persisting staking position.");
            }

            return 0;
        }
    }
}
