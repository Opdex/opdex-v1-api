using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Certificates
{
    public class GetVaultCertificatesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultCertificatesWithFilterQuery, VaultCertificatesDto>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetVaultCertificatesWithFilterQueryHandler(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async override Task<VaultCertificatesDto> Handle(GetVaultCertificatesWithFilterQuery request, CancellationToken cancellationToken)
        {
            var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
            var certificates = await _mediator.Send(new RetrieveVaultCertificatesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

            var certificatesResults = certificates.ToList();

            var cursorDto = BuildCursorDto(certificatesResults, request.Cursor, pointerSelector: result => result.Id);

            return new VaultCertificatesDto { Certificates = _mapper.Map<IEnumerable<VaultCertificateDto>>(certificatesResults), Cursor = cursorDto };
        }
    }
}
