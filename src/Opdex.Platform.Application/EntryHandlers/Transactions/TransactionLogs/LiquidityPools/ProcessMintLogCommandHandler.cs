using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.LiquidityPools;

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
            var lpToken = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (lpToken == null) return false;

            if (request.BlockHeight < lpToken.ModifiedBlock)
            {
                return true;
            }

            lpToken.UpdateTotalSupply(request.Log.TotalSupply, request.BlockHeight);

            return await _mediator.Send(new MakeTokenCommand(lpToken, request.BlockHeight)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(MintLog)}");

            return false;
        }
    }
}