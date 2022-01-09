using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.ProposalCertificates;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.ProposalCertificates;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.ProposalCertificates;

public class RetrieveVaultProposalCertificateByProposalIdQueryHandler
    : IRequestHandler<RetrieveVaultProposalCertificateByProposalIdQuery, VaultProposalCertificate>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalCertificateByProposalIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<VaultProposalCertificate> Handle(RetrieveVaultProposalCertificateByProposalIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultProposalCertificateByProposalIdQuery(request.ProposalId, request.FindOrThrow), cancellationToken);
    }
}
