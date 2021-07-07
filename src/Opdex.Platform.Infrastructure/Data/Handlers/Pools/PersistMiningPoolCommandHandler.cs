using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class PersistMiningPoolCommandHandler : IRequestHandler<PersistMiningPoolCommand, long>
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
              SELECT LAST_INSERT_ID();";

        private static readonly string UpdateSqlCommand =
            $@"UPDATE pool_mining
                SET
                  {nameof(MiningPoolEntity.RewardPerBlock)} = @{nameof(MiningPoolEntity.RewardPerBlock)},
                  {nameof(MiningPoolEntity.RewardPerLpt)} = @{nameof(MiningPoolEntity.RewardPerLpt)},
                  {nameof(MiningPoolEntity.MiningPeriodEndBlock)} = @{nameof(MiningPoolEntity.MiningPeriodEndBlock)},
                  {nameof(MiningPoolEntity.ModifiedBlock)} = @{nameof(MiningPoolEntity.ModifiedBlock)}
                WHERE
                  {nameof(MiningPoolEntity.Id)} = @{nameof(MiningPoolEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PersistMiningPoolCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMiningPoolCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistMiningPoolCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var poolEntity = _mapper.Map<MiningPoolEntity>(request.MiningPool);

                var isUpdate = poolEntity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

                var command = DatabaseQuery.Create(sql, poolEntity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<long>(command);

                return isUpdate ? poolEntity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.MiningPool}");

                return 0;
            }
        }
    }
}
