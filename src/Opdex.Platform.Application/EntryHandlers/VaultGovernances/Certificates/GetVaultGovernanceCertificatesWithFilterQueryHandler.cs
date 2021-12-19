using AutoMapper;
using MediatR;
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
    private readonly IMapper _mapper;

    public GetVaultGovernanceCertificatesWithFilterQueryHandler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<VaultCertificatesDto> Handle(GetVaultGovernanceCertificatesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

        var certificates = await _mediator.Send(new RetrieveVaultGovernanceCertificatesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

        var certificatesResults = certificates.ToList();

        var cursorDto = BuildCursorDto(certificatesResults, request.Cursor, pointerSelector: result => result.Id);

        var assembledResults = _mapper.Map<IEnumerable<VaultCertificateDto>>(certificatesResults);

        return new VaultCertificatesDto() { Certificates = assembledResults, Cursor = cursorDto };
    }
}
