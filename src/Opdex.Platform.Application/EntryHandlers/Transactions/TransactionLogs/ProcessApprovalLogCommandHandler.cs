using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessApprovalLogCommandHandler : IRequestHandler<ProcessApprovalLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessApprovalLogCommandHandler> _logger;

        public ProcessApprovalLogCommandHandler(IMediator mediator, ILogger<ProcessApprovalLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessApprovalLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get/Set user balances and allowances
                // Could be liquidity pool token or src token
                // Could be allowance update and/or balance update
                
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ApprovalLog)}");
               
                return false;
            }
        }
    }
}