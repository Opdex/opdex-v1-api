using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessVaultCertificateCreatedLogCommandHandler : IRequestHandler<ProcessVaultCertificateCreatedLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessVaultCertificateCreatedLogCommandHandler> _logger;

        public ProcessVaultCertificateCreatedLogCommandHandler(IMediator mediator, ILogger<ProcessVaultCertificateCreatedLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessVaultCertificateCreatedLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var vault = await _mediator.Send(new RetrieveVaultQuery());
                
                return await _mediator.Send(new MakeVaultCertificateCommand(new VaultCertificate(vault.Id, request.Log.Owner, request.Log.Amount, request.Log.VestedBlock)), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(VaultCertificateCreatedLog)}");
               
                return false;
            }
        }
    }
}