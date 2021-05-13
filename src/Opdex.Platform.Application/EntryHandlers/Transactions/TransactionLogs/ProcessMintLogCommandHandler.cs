using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessMintLogCommandHandler : IRequestHandler<ProcessMintLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessMintLogCommandHandler> _logger;

        public ProcessMintLogCommandHandler(IMediator mediator, ILogger<ProcessMintLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessMintLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update user liquidity pool token balances
                
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(MintLog)}");
               
                return false;
            }
        }
    }
}