using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionLogs
{
    public class PersistTransactionLiquidityPoolCreatedLogCommandHandler : IRequestHandler<PersistTransactionLiquidityPoolCreatedLogCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_log_liquidity_pool_created (
                {nameof(LiquidityPoolCreatedLogEntity.Token)},
                {nameof(LiquidityPoolCreatedLogEntity.Pool)},
                {nameof(LiquidityPoolCreatedLogEntity.CreatedDate)}
              ) VALUES (
                @{nameof(LiquidityPoolCreatedLogEntity.Token)},
                @{nameof(LiquidityPoolCreatedLogEntity.Pool)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionLiquidityPoolCreatedLogCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionLiquidityPoolCreatedLogCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionLiquidityPoolCreatedLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var poolCreatedLogEntity = _mapper.Map<LiquidityPoolCreatedLogEntity>(request.LiquidityPoolCreatedLog);
            
                var command = DatabaseQuery.Create(SqlCommand, poolCreatedLogEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.LiquidityPoolCreatedLog}");
                
                return 0;
            }
        }
    }
}