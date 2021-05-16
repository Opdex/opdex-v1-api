using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessPermissionsChangeLogCommandHandler : IRequestHandler<ProcessPermissionsChangeLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessPermissionsChangeLogCommandHandler> _logger;

        public ProcessPermissionsChangeLogCommandHandler(IMediator mediator, ILogger<ProcessPermissionsChangeLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessPermissionsChangeLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update the wallet permissions in the market
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(PermissionsChangeLog)}");
               
                return false;
            }
        }
    }
}