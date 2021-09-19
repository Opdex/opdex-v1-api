using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults
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

                var certificates = await _mediator.Send(new RetrieveVaultCertificatesByOwnerAddressQuery(request.Log.Owner));

                // Todo: Maybe create a specific query for this and a unique index on (owner, vestedBlock)
                var certificateToUpdate = certificates.Single(c => c.VestedBlock == request.Log.VestedBlock);

                certificateToUpdate.Redeem(request.Log, request.BlockHeight);

                return await _mediator.Send(new MakeVaultCertificateCommand(certificateToUpdate));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(RedeemVaultCertificateLog)}");

                return false;
            }
        }
    }
}
