using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.ProposalCertificates;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.ProposalCertificates;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.ProposalCertificates;

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
