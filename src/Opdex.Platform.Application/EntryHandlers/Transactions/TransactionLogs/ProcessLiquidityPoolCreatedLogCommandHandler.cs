using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessLiquidityPoolCreatedLogCommandHandler : IRequestHandler<ProcessLiquidityPoolCreatedLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessLiquidityPoolCreatedLogCommandHandler> _logger;

        public ProcessLiquidityPoolCreatedLogCommandHandler(IMediator mediator, ILogger<ProcessLiquidityPoolCreatedLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessLiquidityPoolCreatedLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Log.Contract), CancellationToken.None);
                var tokenId = await _mediator.Send(new MakeTokenCommand(request.Log.Token), CancellationToken.None);
                var pairId = await _mediator.Send(new MakeLiquidityPoolCommand(request.Log.Pool, tokenId, market.Id), CancellationToken.None);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(LiquidityPoolCreatedLog)}");
               
                return false;
            }
        }
    }
}