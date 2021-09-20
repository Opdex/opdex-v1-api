using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Certificates
{
    public class CreateRewindVaultCertificatesCommandHandler : IRequestHandler<CreateRewindVaultCertificatesCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateRewindVaultCertificatesCommandHandler> _logger;

        public CreateRewindVaultCertificatesCommandHandler(IMediator mediator, ILogger<CreateRewindVaultCertificatesCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateRewindVaultCertificatesCommand request, CancellationToken cancellationToken)
        {
            var certificates = await _mediator.Send(new RetrieveVaultCertificatesByModifiedBlockQuery(request.RewindHeight));
            var staleCount = certificates.Count();

            _logger.LogDebug($"Found {staleCount} stale vault certificates.");

            int refreshFailureCount = 0;

            var certsByOwner = certificates.GroupBy(cert => cert.Owner).ToList();

            foreach (var ownersCerts in certsByOwner)
            {
                // Todo: Retrieve Certificates for owner
                var summaries = new List<VaultContractCertificateSummary>();

                foreach (var cert in ownersCerts)
                {
                    var summary = summaries.Single(summary => summary.VestedBlock == cert.VestedBlock);

                    cert.Update(summary, request.RewindHeight);

                    var certRefreshed = await _mediator.Send(new MakeVaultCertificateCommand(cert));

                    if (!certRefreshed) refreshFailureCount++;
                }
            }

            _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} vault certificates.");

            if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale vault certificates.");

            return refreshFailureCount == 0;
        }
    }
}
