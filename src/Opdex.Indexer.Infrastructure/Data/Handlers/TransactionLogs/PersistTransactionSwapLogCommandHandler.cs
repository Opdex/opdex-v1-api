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
    public class PersistTransactionSwapLogCommandHandler: IRequestHandler<PersistTransactionSwapLogCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_log_swap (
                {nameof(SwapLogEntity.Sender)},
                `{nameof(SwapLogEntity.To)}`,
                {nameof(SwapLogEntity.AmountCrsIn)},
                {nameof(SwapLogEntity.AmountSrcIn)},
                {nameof(SwapLogEntity.AmountCrsOut)},
                {nameof(SwapLogEntity.AmountSrcOut)},
                {nameof(SwapLogEntity.CreatedDate)}
              ) VALUES (
                @{nameof(SwapLogEntity.Sender)},
                @{nameof(SwapLogEntity.To)},
                @{nameof(SwapLogEntity.AmountCrsIn)},
                @{nameof(SwapLogEntity.AmountSrcIn)},
                @{nameof(SwapLogEntity.AmountCrsOut)},
                @{nameof(SwapLogEntity.AmountSrcOut)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionSwapLogCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionSwapLogCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionSwapLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var swapLogEntity = _mapper.Map<SwapLogEntity>(request.SwapLog);

                var command = DatabaseQuery.Create(SqlCommand, swapLogEntity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<long>(command);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.SwapLog}");
                return 0;
            }
        }
    }
}