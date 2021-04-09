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
    public class PersistTransactionReservesLogCommandHandler: IRequestHandler<PersistTransactionReservesLogCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_log_reserves (
                {nameof(ReservesLogEntity.ReserveCrs)},
                {nameof(ReservesLogEntity.ReserveSrc)},
                {nameof(ReservesLogEntity.CreatedDate)}
              ) VALUES (
                @{nameof(ReservesLogEntity.ReserveCrs)},
                @{nameof(ReservesLogEntity.ReserveSrc)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionReservesLogCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionReservesLogCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionReservesLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var syncLogEntity = _mapper.Map<ReservesLogEntity>(request.ReservesLog);
            
                var command = DatabaseQuery.Create(SqlCommand, syncLogEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.ReservesLog}");
                return 0;
            }
        }
    }
}