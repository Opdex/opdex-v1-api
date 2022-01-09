using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Certificates;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Certificates;

public class GetVaultCertificatesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultCertificatesWithFilterQuery, VaultCertificatesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultCertificate, VaultCertificateDto> _certificateAssembler;

    public GetVaultCertificatesWithFilterQueryHandler(IMediator mediator,
                                                                IModelAssembler<VaultCertificate, VaultCertificateDto> assembler,
                                                                ILogger<GetVaultCertificatesWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _certificateAssembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
    }

    public override async Task<VaultCertificatesDto> Handle(GetVaultCertificatesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

        var certificates = await _mediator.Send(new RetrieveVaultCertificatesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

        var certificatesResults = certificates.ToList();

        var cursorDto = BuildCursorDto(certificatesResults, request.Cursor, pointerSelector: result => result.Id);

        var certificateDtos = await Task.WhenAll(certificatesResults.Select(certificate => _certificateAssembler.Assemble(certificate)));

        return new VaultCertificatesDto { Certificates = certificateDtos, Cursor = cursorDto };
    }
}
