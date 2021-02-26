using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents
{
    public class PersistTransactionSwapEventCommandHandler: IRequestHandler<PersistTransactionSwapEventCommand, bool>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO transaction_event_swap (
                {nameof(SwapEventEntity.TransactionId)},
                {nameof(SwapEventEntity.Address)},
                {nameof(SwapEventEntity.Sender)},
                {nameof(SwapEventEntity.To)},
                {nameof(SwapEventEntity.AmountCrsIn)},
                {nameof(SwapEventEntity.AmountSrcIn)},
                {nameof(SwapEventEntity.AmountCrsOut)},
                {nameof(SwapEventEntity.AmountSrcOut)},
                {nameof(SwapEventEntity.CreatedDate)}
              ) VALUES (
                @{nameof(SwapEventEntity.TransactionId)},
                @{nameof(SwapEventEntity.Address)},
                @{nameof(SwapEventEntity.Sender)},
                @{nameof(SwapEventEntity.To)},
                @{nameof(SwapEventEntity.AmountCrsIn)},
                @{nameof(SwapEventEntity.AmountSrcIn)},
                @{nameof(SwapEventEntity.AmountCrsOut)},
                @{nameof(SwapEventEntity.AmountSrcOut)},
                UTC_TIMESTAMP()
              );";
        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistTransactionSwapEventCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistTransactionSwapEventCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<bool> Handle(PersistTransactionSwapEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var swapEventEntity = _mapper.Map<SwapEventEntity>(request.SwapEvent);

                var command = DatabaseQuery.Create(SqlCommand, swapEventEntity, cancellationToken);

                var result = await _context.ExecuteCommandAsync(command);

                return result > 0;
            }
            catch (Exception)
            {
                _logger.LogError($"Unable to persist {request.SwapEvent}");
                return false;
            }
        }
    }
}