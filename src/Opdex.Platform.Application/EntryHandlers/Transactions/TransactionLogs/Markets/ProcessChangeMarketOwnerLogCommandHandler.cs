using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets
{
    public class ProcessChangeMarketOwnerLogCommandHandler : IRequestHandler<ProcessChangeMarketOwnerLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessChangeMarketOwnerLogCommandHandler> _logger;

        public ProcessChangeMarketOwnerLogCommandHandler(IMediator mediator, ILogger<ProcessChangeMarketOwnerLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessChangeMarketOwnerLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the owner address of the market
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ChangeMarketOwnerLog)}");
               
                return false;
            }
        }
    }
}