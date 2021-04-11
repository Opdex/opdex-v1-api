using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;

namespace Opdex.Indexer.Infrastructure.Data.Handlers
{
    public class PersistMiningPoolCommandHandler : IRequestHandler<PersistMiningPoolCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO pool_mining (
                {nameof(MiningPoolEntity.LiquidityPoolId)},
                {nameof(MiningPoolEntity.Address)},
                {nameof(MiningPoolEntity.RewardRate)},
                {nameof(MiningPoolEntity.MiningPeriodEndBlock)},
                {nameof(MiningPoolEntity.CreatedDate)}
              ) VALUES (
                @{nameof(MiningPoolEntity.LiquidityPoolId)},
                @{nameof(MiningPoolEntity.Address)},
                @{nameof(MiningPoolEntity.RewardRate)},
                @{nameof(MiningPoolEntity.MiningPeriodEndBlock)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistMiningPoolCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistMiningPoolCommandHandler> logger)
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
            
                var command = DatabaseQuery.Create(SqlCommand, poolEntity, cancellationToken);
            
                return await _context.ExecuteScalarAsync<long>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.MiningPool}");
                
                return 0;
            }
        }
    }
}