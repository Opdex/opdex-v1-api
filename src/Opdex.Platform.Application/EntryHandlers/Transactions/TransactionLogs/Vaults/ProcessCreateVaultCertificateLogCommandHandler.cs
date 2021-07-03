using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults
{
    public class ProcessCreateVaultCertificateLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessCreateVaultCertificateLogCommand, bool>
    {
        private readonly ILogger<ProcessCreateVaultCertificateLogCommandHandler> _logger;

        public ProcessCreateVaultCertificateLogCommandHandler(IMediator mediator, ILogger<ProcessCreateVaultCertificateLogCommandHandler> logger)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessCreateVaultCertificateLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                var vaultQuery = new RetrieveVaultQuery(findOrThrow: true);
                var vault = await _mediator.Send(vaultQuery, CancellationToken.None);

                var vaultCertificate = new VaultCertificate(vault.Id, request.Log.Owner, request.Log.Amount, request.Log.VestedBlock, request.BlockHeight);

                var vaultCommand = new MakeVaultCertificateCommand(vaultCertificate);
                return await _mediator.Send(vaultCommand, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(CreateVaultCertificateLog)}.");
               
                return false;
            }
        }
    }
}