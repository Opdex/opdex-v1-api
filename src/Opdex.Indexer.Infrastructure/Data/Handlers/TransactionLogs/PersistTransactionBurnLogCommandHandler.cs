using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionLogs
{
    public class PersistTransactionBurnLogCommandHandler: IRequestHandler<PersistTransactionBurnLogCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_log_burn (
                {nameof(BurnLogEntity.Sender)},
                `{nameof(BurnLogEntity.To)}`,
                {nameof(BurnLogEntity.AmountCrs)},
                {nameof(BurnLogEntity.AmountSrc)},
                {nameof(BurnLogEntity.CreatedDate)}
              ) VALUES (
                @{nameof(BurnLogEntity.Sender)},
                @{nameof(BurnLogEntity.To)},
                @{nameof(BurnLogEntity.AmountCrs)},
                @{nameof(BurnLogEntity.AmountSrc)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionBurnLogCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionBurnLogCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionBurnLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var burnLogEntity = _mapper.Map<BurnLogEntity>(request.BurnLog);
            
                var command = DatabaseQuery.Create(SqlCommand, burnLogEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.BurnLog}");
                return 0;
            }
        }
    }
}