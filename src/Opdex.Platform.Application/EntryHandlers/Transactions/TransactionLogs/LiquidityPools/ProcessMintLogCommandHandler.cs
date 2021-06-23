using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools
{
    public class ProcessMintLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessMintLogCommand, bool>
    {
        private readonly ILogger<ProcessMintLogCommandHandler> _logger;

        public ProcessMintLogCommandHandler(IMediator mediator, ILogger<ProcessMintLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessMintLogCommand request, CancellationToken cancellationToken)
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
                var summary = await _mediator.Send(new RetrieveCirrusLocalCallSmartContractQuery(pool.Address, "get_TotalSupply"));
                var totalSupply = summary.DeserializeValue<string>();

                lpToken.UpdateTotalSupply(totalSupply, request.BlockHeight);

                var response = await _mediator.Send(new MakeTokenCommand(lpToken.Address, lpToken));

                return response > 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(MintLog)}");

                return false;
            }
        }
    }
}