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
    public class ProcessRedeemVaultCertificateLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessRedeemVaultCertificateLogCommand, bool>
    {
        private readonly ILogger<ProcessRedeemVaultCertificateLogCommandHandler> _logger;

        public ProcessRedeemVaultCertificateLogCommandHandler(IMediator mediator, ILogger<ProcessRedeemVaultCertificateLogCommandHandler> logger)
            : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessRedeemVaultCertificateLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                var certificatesQuery = new RetrieveVaultCertificatesByOwnerAddressQuery(request.Log.Owner);
                var certificates = await _mediator.Send(certificatesQuery, CancellationToken.None);
                
                // Todo: Maybe create a specific query for this and a unique index on (owner, vestedBlock) 
                var certificateToUpdate = certificates.Single(c => c.VestedBlock == request.Log.VestedBlock);
                
                certificateToUpdate.Redeem(request.Log, request.BlockHeight);

                var certificateCommand = new MakeVaultCertificateCommand(certificateToUpdate);
                return await _mediator.Send(certificateCommand, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(RedeemVaultCertificateLog)}");
               
                return false;
            }
        }
    }
}