using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vault;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vault
{
    public class ProcessChangeVaultOwnerLogCommandHandler : IRequestHandler<ProcessChangeVaultOwnerLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessChangeVaultOwnerLogCommandHandler> _logger;

        public ProcessChangeVaultOwnerLogCommandHandler(IMediator mediator, ILogger<ProcessChangeVaultOwnerLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessChangeVaultOwnerLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var vault = await _mediator.Send(new RetrieveVaultQuery(), CancellationToken.None);
                
                vault.SetOwner(request.Log);
                
                var vaultId = await _mediator.Send(new MakeVaultCommand(vault), CancellationToken.None);

                return vaultId > 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ChangeVaultOwnerLog)}");
               
                return false;
            }
        }
    }
}