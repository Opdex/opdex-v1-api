using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MarketDeployers
{
    public class ProcessChangeDeployerOwnerLogCommandHandler : IRequestHandler<ProcessChangeDeployerOwnerLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessChangeDeployerOwnerLogCommandHandler> _logger;

        public ProcessChangeDeployerOwnerLogCommandHandler(IMediator mediator, ILogger<ProcessChangeDeployerOwnerLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessChangeDeployerOwnerLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the owner address of the market
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ChangeDeployerOwnerLog)}");
               
                return false;
            }
        }
    }
}