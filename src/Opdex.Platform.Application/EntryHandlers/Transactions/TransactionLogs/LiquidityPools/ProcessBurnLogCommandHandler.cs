using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessBurnLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessBurnLogCommand, bool>
    {
        private readonly ILogger<ProcessBurnLogCommandHandler> _logger;

        public ProcessBurnLogCommandHandler(IMediator mediator, ILogger<ProcessBurnLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessBurnLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Log.Contract, findOrThrow: true));
                var lpToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool.LpTokenId, findOrThrow: true));

                lpToken.UpdateTotalSupply(request.Log.TotalSupply, request.BlockHeight);

                var response = await _mediator.Send(new MakeTokenCommand(lpToken.Address, lpToken));

                return response > 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(BurnLog)}");

                return false;
            }
        }
    }
}
