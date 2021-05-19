using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vault;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vault
{
    public class ProcessCreateVaultCertificateLogCommandHandler : IRequestHandler<ProcessCreateVaultCertificateLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessCreateVaultCertificateLogCommandHandler> _logger;
        private const bool FindOrThrow = true;

        public ProcessCreateVaultCertificateLogCommandHandler(IMediator mediator, ILogger<ProcessCreateVaultCertificateLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCreateVaultCertificateLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var vault = await _mediator.Send(new RetrieveVaultQuery(FindOrThrow), CancellationToken.None);

                var vaultCertificate = new VaultCertificate(vault.Id, request.Log.Owner, request.Log.Amount, request.Log.VestedBlock, request.BlockHeight);
                
                return await _mediator.Send(new MakeVaultCertificateCommand(vaultCertificate), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CreateVaultCertificateLog)}.");
               
                return false;
            }
        }
    }
}