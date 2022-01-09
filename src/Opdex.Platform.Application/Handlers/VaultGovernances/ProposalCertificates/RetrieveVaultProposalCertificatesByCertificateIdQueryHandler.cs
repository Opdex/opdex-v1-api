using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.ProposalCertificates;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.ProposalCertificates;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.ProposalCertificates;

public class RetrieveVaultProposalCertificatesByCertificateIdQueryHandler
    : IRequestHandler<RetrieveVaultProposalCertificatesByCertificateIdQuery, IEnumerable<VaultProposalCertificate>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalCertificatesByCertificateIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<IEnumerable<VaultProposalCertificate>> Handle(RetrieveVaultProposalCertificatesByCertificateIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultProposalCertificatesByCertificateIdQuery(request.CertiicateId), cancellationToken);
    }
}
