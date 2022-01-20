using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningPools;

public class PersistMiningPoolCommandHandler : IRequestHandler<PersistMiningPoolCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO pool_mining (
                {nameof(MiningPoolEntity.LiquidityPoolId)},
                {nameof(MiningPoolEntity.Address)},
                {nameof(MiningPoolEntity.RewardPerBlock)},
                {nameof(MiningPoolEntity.RewardPerLpt)},
                {nameof(MiningPoolEntity.MiningPeriodEndBlock)},
                {nameof(MiningPoolEntity.CreatedBlock)},
                {nameof(MiningPoolEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(MiningPoolEntity.LiquidityPoolId)},
                @{nameof(MiningPoolEntity.Address)},
                @{nameof(MiningPoolEntity.RewardPerBlock)},
                @{nameof(MiningPoolEntity.RewardPerLpt)},
                @{nameof(MiningPoolEntity.MiningPeriodEndBlock)},
                @{nameof(MiningPoolEntity.CreatedBlock)},
                @{nameof(MiningPoolEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE pool_mining
                SET
                  {nameof(MiningPoolEntity.RewardPerBlock)} = @{nameof(MiningPoolEntity.RewardPerBlock)},
                  {nameof(MiningPoolEntity.RewardPerLpt)} = @{nameof(MiningPoolEntity.RewardPerLpt)},
                  {nameof(MiningPoolEntity.MiningPeriodEndBlock)} = @{nameof(MiningPoolEntity.MiningPeriodEndBlock)},
                  {nameof(MiningPoolEntity.ModifiedBlock)} = @{nameof(MiningPoolEntity.ModifiedBlock)}
                WHERE
                  {nameof(MiningPoolEntity.Id)} = @{nameof(MiningPoolEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistMiningPoolCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMiningPoolCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistMiningPoolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var poolEntity = _mapper.Map<MiningPoolEntity>(request.MiningPool);

            var isUpdate = poolEntity.Id >= 1;

            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

            var command = DatabaseQuery.Create(sql, poolEntity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return isUpdate ? poolEntity.Id : result;
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>()
            {
                { "Contract", request.MiningPool.Address },
                { "LiquidityPoolId", request.MiningPool.LiquidityPoolId },
                { "BlockHeight", request.MiningPool.ModifiedBlock }
            }))
            {
                _logger.LogError(ex, $"Unable to persist mining pool.");
            }

            return 0;
        }
    }
}