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
    public class PersistLiquidityPoolSnapshotCommandHandler : IRequestHandler<PersistLiquidityPoolSnapshotCommand, bool>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO pool_liquidity_snapshot (
                {nameof(LiquidityPoolSnapshotEntity.LiquidityPoolId)},
                {nameof(LiquidityPoolSnapshotEntity.TransactionCount)},
                {nameof(LiquidityPoolSnapshotEntity.ReserveCrs)},
                {nameof(LiquidityPoolSnapshotEntity.ReserveSrc)},
                {nameof(LiquidityPoolSnapshotEntity.ReserveUsd)},
                {nameof(LiquidityPoolSnapshotEntity.VolumeCrs)},
                {nameof(LiquidityPoolSnapshotEntity.VolumeSrc)},
                {nameof(LiquidityPoolSnapshotEntity.VolumeUsd)},
                {nameof(LiquidityPoolSnapshotEntity.StakingWeight)},
                {nameof(LiquidityPoolSnapshotEntity.StakingUsd)},
                {nameof(LiquidityPoolSnapshotEntity.ProviderRewards)},
                {nameof(LiquidityPoolSnapshotEntity.StakerRewards)},
                {nameof(LiquidityPoolSnapshotEntity.SnapshotTypeId)},
                {nameof(LiquidityPoolSnapshotEntity.StartDate)},
                {nameof(LiquidityPoolSnapshotEntity.EndDate)}
              ) VALUES (
                @{nameof(LiquidityPoolSnapshotEntity.LiquidityPoolId)},
                @{nameof(LiquidityPoolSnapshotEntity.TransactionCount)},
                @{nameof(LiquidityPoolSnapshotEntity.ReserveCrs)},
                @{nameof(LiquidityPoolSnapshotEntity.ReserveSrc)},
                @{nameof(LiquidityPoolSnapshotEntity.ReserveUsd)},
                @{nameof(LiquidityPoolSnapshotEntity.VolumeCrs)},
                @{nameof(LiquidityPoolSnapshotEntity.VolumeSrc)},
                @{nameof(LiquidityPoolSnapshotEntity.VolumeUsd)},
                @{nameof(LiquidityPoolSnapshotEntity.StakingWeight)},
                @{nameof(LiquidityPoolSnapshotEntity.StakingUsd)},
                @{nameof(LiquidityPoolSnapshotEntity.ProviderRewards)},
                @{nameof(LiquidityPoolSnapshotEntity.StakerRewards)},
                @{nameof(LiquidityPoolSnapshotEntity.SnapshotTypeId)},
                @{nameof(LiquidityPoolSnapshotEntity.StartDate)},
                @{nameof(LiquidityPoolSnapshotEntity.EndDate)}
              );";
        
        private static readonly string UpdateSqlCommand =
            $@"UPDATE pool_liquidity_snapshot 
                SET 
                    {nameof(LiquidityPoolSnapshotEntity.TransactionCount)} = @{nameof(LiquidityPoolSnapshotEntity.TransactionCount)},
                    {nameof(LiquidityPoolSnapshotEntity.ReserveCrs)} = @{nameof(LiquidityPoolSnapshotEntity.ReserveCrs)},
                    {nameof(LiquidityPoolSnapshotEntity.ReserveSrc)} = @{nameof(LiquidityPoolSnapshotEntity.ReserveSrc)},
                    {nameof(LiquidityPoolSnapshotEntity.ReserveUsd)} = @{nameof(LiquidityPoolSnapshotEntity.ReserveUsd)},
                    {nameof(LiquidityPoolSnapshotEntity.VolumeCrs)} = @{nameof(LiquidityPoolSnapshotEntity.VolumeCrs)},
                    {nameof(LiquidityPoolSnapshotEntity.VolumeSrc)} = @{nameof(LiquidityPoolSnapshotEntity.VolumeSrc)},
                    {nameof(LiquidityPoolSnapshotEntity.VolumeUsd)} = @{nameof(LiquidityPoolSnapshotEntity.VolumeUsd)},
                    {nameof(LiquidityPoolSnapshotEntity.StakingWeight)} = @{nameof(LiquidityPoolSnapshotEntity.StakingWeight)},
                    {nameof(LiquidityPoolSnapshotEntity.StakingUsd)} = @{nameof(LiquidityPoolSnapshotEntity.StakingUsd)}
                    {nameof(LiquidityPoolSnapshotEntity.ProviderRewards)} = @{nameof(LiquidityPoolSnapshotEntity.ProviderRewards)}
                    {nameof(LiquidityPoolSnapshotEntity.StakerRewards)} = @{nameof(LiquidityPoolSnapshotEntity.StakerRewards)}
                WHERE {nameof(LiquidityPoolSnapshotEntity.Id)} = @{nameof(LiquidityPoolSnapshotEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistLiquidityPoolSnapshotCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistLiquidityPoolSnapshotCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(PersistLiquidityPoolSnapshotCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<LiquidityPoolSnapshotEntity>(request.Snapshot);

                var sql = entity.Id < 1 ? InsertSqlCommand : UpdateSqlCommand;
                
                var command = DatabaseQuery.Create(sql, entity, cancellationToken);
                
                return await _context.ExecuteCommandAsync(command) >= 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting liquidity pool snapshot for poolId {request?.Snapshot?.LiquidityPoolId} and type {request?.Snapshot?.SnapshotType}");
                
                return false;
            }
        }
    }
}