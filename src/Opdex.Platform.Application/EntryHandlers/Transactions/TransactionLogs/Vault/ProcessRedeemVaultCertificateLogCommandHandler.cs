using System;
using System.Linq;
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
    public class ProcessRedeemVaultCertificateLogCommandHandler : IRequestHandler<ProcessRedeemVaultCertificateLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessRedeemVaultCertificateLogCommandHandler> _logger;

        public ProcessRedeemVaultCertificateLogCommandHandler(IMediator mediator, ILogger<ProcessRedeemVaultCertificateLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessRedeemVaultCertificateLogCommand request, CancellationToken cancellationToken)
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
                _logger.LogError(ex, $"Failure processing {nameof(RedeemVaultCertificateLog)}");
               
                return false;
            }
        }
    }
}