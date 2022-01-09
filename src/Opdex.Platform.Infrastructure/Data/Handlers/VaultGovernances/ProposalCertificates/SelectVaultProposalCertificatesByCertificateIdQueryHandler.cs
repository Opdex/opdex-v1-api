using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.ProposalCertificates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.ProposalCertificates;

public class SelectVaultProposalCertificatesByCertificateIdQueryHandler
    : IRequestHandler<SelectVaultProposalCertificatesByCertificateIdQuery, IEnumerable<VaultProposalCertificate>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultProposalCertificateEntity.Id)},
                {nameof(VaultProposalCertificateEntity.ProposalId)},
                {nameof(VaultProposalCertificateEntity.CertificateId)},
                {nameof(VaultProposalCertificateEntity.CreatedBlock)},
                {nameof(VaultProposalCertificateEntity.ModifiedBlock)}
            FROM vault_proposal_certificate
            WHERE {nameof(VaultProposalCertificateEntity.CertificateId)} = @{nameof(SqlParams.CertificateId)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultProposalCertificatesByCertificateIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultProposalCertificate>> Handle(SelectVaultProposalCertificatesByCertificateIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.CertiicateId), cancellationToken);

        var result = await _context.ExecuteQueryAsync<VaultProposalCertificateEntity>(query);

        return _mapper.Map<IEnumerable<VaultProposalCertificate>>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong certificateId)
        {
            CertificateId = certificateId;
        }

        public ulong CertificateId { get; }
    }
}
