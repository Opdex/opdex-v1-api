using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Certificates;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances.Certificates;

public class GetVaultGovernanceCertificatesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultGovernanceCertificatesWithFilterQuery, VaultCertificatesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultCertificate, VaultCertificateDto> _certificateAssembler;

    public GetVaultGovernanceCertificatesWithFilterQueryHandler(IMediator mediator,
                                                                IModelAssembler<VaultCertificate, VaultCertificateDto> assembler,
                                                                ILogger<GetVaultGovernanceCertificatesWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _certificateAssembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
    }

    public override async Task<VaultCertificatesDto> Handle(GetVaultGovernanceCertificatesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

        var certificates = await _mediator.Send(new RetrieveVaultGovernanceCertificatesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

        var certificatesResults = certificates.ToList();

        var cursorDto = BuildCursorDto(certificatesResults, request.Cursor, pointerSelector: result => result.Id);

        var certificateDtos = await Task.WhenAll(certificatesResults.Select(certificate => _certificateAssembler.Assemble(certificate)));

        return new VaultCertificatesDto { Certificates = certificateDtos, Cursor = cursorDto };
    }
}
