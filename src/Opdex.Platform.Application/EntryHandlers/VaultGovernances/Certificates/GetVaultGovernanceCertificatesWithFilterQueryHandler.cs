using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Certificates;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances.Certificates;

public class GetVaultGovernanceCertificatesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultGovernanceCertificatesWithFilterQuery, VaultCertificatesDto>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetVaultGovernanceCertificatesWithFilterQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetVaultGovernanceCertificatesWithFilterQueryHandler(IMapper mapper, IMediator mediator, ILogger<GetVaultGovernanceCertificatesWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<VaultCertificatesDto> Handle(GetVaultGovernanceCertificatesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

        var certificates = await _mediator.Send(new RetrieveVaultGovernanceCertificatesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

        var certificatesResults = certificates.ToList();

        _logger.LogTrace("Retrieved queried certificates");

        var cursorDto = BuildCursorDto(certificatesResults, request.Cursor, pointerSelector: result => result.Id);

        _logger.LogTrace("Returning {ResultCount} results", certificatesResults.Count);

        var assembledResults = _mapper.Map<IEnumerable<VaultCertificateDto>>(certificatesResults);

        _logger.LogTrace("Assembled results");

        return new VaultCertificatesDto() { Certificates = assembledResults, Cursor = cursorDto };
    }
}
