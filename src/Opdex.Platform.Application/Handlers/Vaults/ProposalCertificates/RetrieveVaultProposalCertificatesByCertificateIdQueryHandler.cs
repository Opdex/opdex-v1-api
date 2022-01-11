using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.ProposalCertificates;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.ProposalCertificates;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.ProposalCertificates;

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
