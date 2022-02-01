using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.ProposalCertificates;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults.ProposalCertificates;

public class SelectVaultProposalCertificateByProposalIdQueryHandler
    : IRequestHandler<SelectVaultProposalCertificateByProposalIdQuery, VaultProposalCertificate>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultProposalCertificateEntity.Id)},
                {nameof(VaultProposalCertificateEntity.ProposalId)},
                {nameof(VaultProposalCertificateEntity.CertificateId)}
            FROM vault_proposal_certificate
            WHERE {nameof(VaultProposalCertificateEntity.ProposalId)} = @{nameof(SqlParams.ProposalId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultProposalCertificateByProposalIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<VaultProposalCertificate> Handle(SelectVaultProposalCertificateByProposalIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.ProposalId), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultProposalCertificateEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException("Certificate not found.");
        }

        return result == null ? null : _mapper.Map<VaultProposalCertificate>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong proposalId)
        {
            ProposalId = proposalId;
        }

        public ulong ProposalId { get; }
    }
}
