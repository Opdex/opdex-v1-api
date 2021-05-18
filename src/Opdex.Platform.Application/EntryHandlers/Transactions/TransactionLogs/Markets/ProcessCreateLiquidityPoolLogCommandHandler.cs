using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets
{
    public class ProcessCreateLiquidityPoolLogCommandHandler : IRequestHandler<ProcessCreateLiquidityPoolLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessCreateLiquidityPoolLogCommandHandler> _logger;

        public ProcessCreateLiquidityPoolLogCommandHandler(IMediator mediator, ILogger<ProcessCreateLiquidityPoolLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCreateLiquidityPoolLogCommand request, CancellationToken cancellationToken)
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
                _logger.LogError(ex, $"Failure processing {nameof(CreateLiquidityPoolLog)}");
               
                return false;
            }
        }
    }
}