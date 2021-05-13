using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public class ProcessVaultCertificateRedeemedLogCommandHandler : IRequestHandler<ProcessVaultCertificateRedeemedLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessVaultCertificateRedeemedLogCommandHandler> _logger;

        public ProcessVaultCertificateRedeemedLogCommandHandler(IMediator mediator, ILogger<ProcessVaultCertificateRedeemedLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessVaultCertificateRedeemedLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Mark certificate as redeemed
                var certificates = await _mediator.Send(new RetrieveVaultCertificatesByOwnerAddressQuery(request.Log.Owner), CancellationToken.None);
                var certificateToUpdate = certificates.Single(c => c.VestedBlock == request.Log.VestedBlock);
                
                certificateToUpdate.Redeem(request.Log);
                
                return await _mediator.Send(new MakeVaultCertificateCommand(certificateToUpdate), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(VaultCertificateRedeemedLog)}");
               
                return false;
            }
        }
    }
}