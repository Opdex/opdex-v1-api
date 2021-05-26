using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vault;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vault;

namespace Opdex.Platform.Application.Handlers.vault
{
    public class RetrieveVaultCertificatesByOwnerAddressQueryHandler 
        : IRequestHandler<RetrieveVaultCertificatesByOwnerAddressQuery, IEnumerable<VaultCertificate>>
    {
        private readonly IMediator _mediator;

        public RetrieveVaultCertificatesByOwnerAddressQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<VaultCertificate>> Handle(RetrieveVaultCertificatesByOwnerAddressQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectVaultCertificatesByOwnerAddressQuery(request.OwnerAddress), cancellationToken);
        }
    }
}