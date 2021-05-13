using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessMarketOwnerChangeLogCommandHandler : IRequestHandler<ProcessMarketOwnerChangeLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessMarketOwnerChangeLogCommandHandler> _logger;

        public ProcessMarketOwnerChangeLogCommandHandler(IMediator mediator, ILogger<ProcessMarketOwnerChangeLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessMarketOwnerChangeLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the owner address of the market
                
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(MarketOwnerChangeLog)}");
               
                return false;
            }
        }
    }
}