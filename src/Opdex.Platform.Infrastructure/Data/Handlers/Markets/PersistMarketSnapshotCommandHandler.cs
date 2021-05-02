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
    public class PersistMarketSnapshotCommandHandler : IRequestHandler<PersistMarketSnapshotCommand, bool>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO market_snapshot (
                {nameof(MarketSnapshotEntity.MarketId)},
                {nameof(MarketSnapshotEntity.TransactionCount)},
                {nameof(MarketSnapshotEntity.Liquidity)},
                {nameof(MarketSnapshotEntity.Volume)},
                {nameof(MarketSnapshotEntity.StakingWeight)},
                {nameof(MarketSnapshotEntity.StakingUsd)},
                {nameof(MarketSnapshotEntity.ProviderRewards)},
                {nameof(MarketSnapshotEntity.StakerRewards)},
                {nameof(MarketSnapshotEntity.SnapshotType)},
                {nameof(MarketSnapshotEntity.StartDate)},
                {nameof(MarketSnapshotEntity.EndDate)}
              ) VALUES (
                @{nameof(MarketSnapshotEntity.MarketId)},
                @{nameof(MarketSnapshotEntity.TransactionCount)},
                @{nameof(MarketSnapshotEntity.Liquidity)},
                @{nameof(MarketSnapshotEntity.Volume)},
                @{nameof(MarketSnapshotEntity.StakingWeight)},
                @{nameof(MarketSnapshotEntity.StakingUsd)},
                @{nameof(MarketSnapshotEntity.ProviderRewards)},
                @{nameof(MarketSnapshotEntity.StakerRewards)},
                @{nameof(MarketSnapshotEntity.SnapshotType)},
                @{nameof(MarketSnapshotEntity.StartDate)},
                @{nameof(MarketSnapshotEntity.EndDate)}
              );";
        
        private static readonly string UpdateSqlCommand =
            $@"UPDATE market_snapshot 
                SET 
                    {nameof(MarketSnapshotEntity.TransactionCount)} = @{nameof(MarketSnapshotEntity.TransactionCount)},
                    {nameof(MarketSnapshotEntity.Liquidity)} = @{nameof(MarketSnapshotEntity.Liquidity)},
                    {nameof(MarketSnapshotEntity.Volume)} = @{nameof(MarketSnapshotEntity.Volume)},
                    {nameof(MarketSnapshotEntity.StakingWeight)} = @{nameof(MarketSnapshotEntity.StakingWeight)},
                    {nameof(MarketSnapshotEntity.StakingUsd)} = @{nameof(MarketSnapshotEntity.StakingUsd)},
                    {nameof(MarketSnapshotEntity.ProviderRewards)} = @{nameof(MarketSnapshotEntity.ProviderRewards)},
                    {nameof(MarketSnapshotEntity.StakerRewards)} = @{nameof(MarketSnapshotEntity.StakerRewards)}
                WHERE {nameof(MarketSnapshotEntity.Id)} = @{nameof(MarketSnapshotEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistMarketSnapshotCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistMarketSnapshotCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(PersistMarketSnapshotCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<MarketSnapshotEntity>(request.Snapshot);

                var sql = entity.Id < 1 ? InsertSqlCommand : UpdateSqlCommand;
                
                var command = DatabaseQuery.Create(sql, entity, cancellationToken);
                
                return await _context.ExecuteCommandAsync(command) >= 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting market snapshot for marketId {request?.Snapshot?.MarketId} and type {request?.Snapshot?.SnapshotType}");
                
                return false;
            }
        }
    }
}