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
                
                // Todo: Create IsLpt flag on the token table
                // All liquidity pools are inserted as tokens with the flag set to true
                // Symbol = LPT
                // Name = xBTC-CRS LPT

                // Todo: This is the amount they added to the pool += 
                // var balance = currentBalance + request.Log.AmountLpt;
                
                // Todo: how to handle their "position" 
                // Each log, logs how much SRC/CRS was put in the pool, after - is untracked
                // Consider not tracking their position at all and only tracking their LPT balance
                // Users would only see positions by reviewing transaction history
                // UI would show "current balance - 50 LPT" - "estimated value - $xx.xx
                
                // Update AddressBalance record
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(MintLog)}");
               
                return false;
            }
        }
    }
}