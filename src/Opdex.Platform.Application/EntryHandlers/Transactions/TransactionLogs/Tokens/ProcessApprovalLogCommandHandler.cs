using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens
{
    public class ProcessApprovalLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessApprovalLogCommand, bool>
    {
        private readonly ILogger<ProcessApprovalLogCommandHandler> _logger;

        public ProcessApprovalLogCommandHandler(IMediator mediator, ILogger<ProcessApprovalLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessApprovalLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                // Get/Set user balances and allowances
                // Could be liquidity pool token or src token
                // Could be allowance update and/or balance update
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ApprovalLog)}");
               
                return false;
            }
        }
    }
}