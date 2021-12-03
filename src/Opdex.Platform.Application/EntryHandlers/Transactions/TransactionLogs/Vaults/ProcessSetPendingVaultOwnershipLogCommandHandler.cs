using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults;

public class ProcessSetPendingVaultOwnershipLogCommandHandler : IRequestHandler<ProcessSetPendingVaultOwnershipLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessSetPendingVaultOwnershipLogCommandHandler> _logger;

    public ProcessSetPendingVaultOwnershipLogCommandHandler(IMediator mediator, ILogger<ProcessSetPendingVaultOwnershipLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessSetPendingVaultOwnershipLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (vault == null) return false;

            if (request.BlockHeight < vault.ModifiedBlock)
            {
                return true;
            }

            vault.SetPendingOwnership(request.Log, request.BlockHeight);

            return await _mediator.Send(new MakeVaultCommand(vault, request.BlockHeight)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(SetPendingVaultOwnershipLog)}");

            return false;
        }
    }
}