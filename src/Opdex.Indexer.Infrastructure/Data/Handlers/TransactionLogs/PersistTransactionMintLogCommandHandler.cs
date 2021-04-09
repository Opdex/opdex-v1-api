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
    public class PersistTransactionMintLogCommandHandler: IRequestHandler<PersistTransactionMintLogCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_log_mint (
                {nameof(MintLogEntity.Sender)},
                {nameof(MintLogEntity.AmountCrs)},
                {nameof(MintLogEntity.AmountSrc)},
                {nameof(MintLogEntity.CreatedDate)}
              ) VALUES (
                @{nameof(MintLogEntity.Sender)},
                @{nameof(MintLogEntity.AmountCrs)},
                @{nameof(MintLogEntity.AmountSrc)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionMintLogCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionMintLogCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<long> Handle(PersistTransactionMintLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var mintLogEntity = _mapper.Map<MintLogEntity>(request.MintLog);
            
                var command = DatabaseQuery.Create(SqlCommand, mintLogEntity, cancellationToken);
            
                var result = await _context.ExecuteScalarAsync<long>(command);
            
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist {request.MintLog}");
                return 0;
            }
        }
    }
}