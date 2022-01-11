using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Certificates;

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
        var certificatesList = certificates.ToList();
        var staleCount = certificatesList.Count;

        _logger.LogDebug($"Found {staleCount} stale vault certificates.");

        int refreshFailureCount = 0;

        var certsByVault = certificatesList.GroupBy(cert => cert.VaultId);

        foreach (var vaultCerts in certsByVault)
        {
            var vault = await _mediator.Send(new RetrieveVaultByIdQuery(vaultCerts.Key, findOrThrow: false));

            if (vault == null)
            {
                refreshFailureCount += vaultCerts.Count();
                _logger.LogError($"Cannot find vault with id {vaultCerts.Key}.");
                continue;
            }

            var certChunks = vaultCerts.Chunk(10);

            foreach (var chunk in certChunks)
            {
                await Task.WhenAll(chunk.Select(async cert =>
                {
                    var summary = await _mediator.Send(new RetrieveVaultContractCertificateSummaryByOwnerQuery(vault.Address,
                                                                                                                         cert.Owner,
                                                                                                                         request.RewindHeight));

                    if (summary != null && summary.VestedBlock == cert.VestedBlock)
                    {
                        cert.Update(summary, request.RewindHeight);

                        var certRefreshed = await _mediator.Send(new MakeVaultCertificateCommand(cert)) > 0;

                        if (!certRefreshed) refreshFailureCount++;
                    }
                }));
            }
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} vault certificates.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale vault certificates.");

        return refreshFailureCount == 0;
    }
}
