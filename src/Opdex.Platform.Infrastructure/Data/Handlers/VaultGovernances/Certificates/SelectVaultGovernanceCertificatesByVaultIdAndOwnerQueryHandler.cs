using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Certificates;

public class SelectVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandler
    : IRequestHandler<SelectVaultGovernanceCertificatesByVaultIdAndOwnerQuery, IEnumerable<VaultCertificate>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultCertificateEntity.Id)},
                {nameof(VaultCertificateEntity.VaultId)},
                {nameof(VaultCertificateEntity.ProposalId)},
                {nameof(VaultCertificateEntity.Owner)},
                {nameof(VaultCertificateEntity.Amount)},
                {nameof(VaultCertificateEntity.Revoked)},
                {nameof(VaultCertificateEntity.Redeemed)},
                {nameof(VaultCertificateEntity.VestedBlock)},
                {nameof(VaultCertificateEntity.CreatedBlock)},
                {nameof(VaultCertificateEntity.ModifiedBlock)}
            FROM vault_governance_certificate
            WHERE {nameof(VaultCertificateEntity.VaultId)} = @{nameof(SqlParams.VaultId)}
                AND {nameof(VaultCertificateEntity.Owner)} = @{nameof(SqlParams.Owner)}".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultCertificate>> Handle(SelectVaultGovernanceCertificatesByVaultIdAndOwnerQuery request,
                                                            CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.VaultId, request.Owner), cancellationToken);

        var result = await _context.ExecuteQueryAsync<VaultCertificateEntity>(query);

        return _mapper.Map<IEnumerable<VaultCertificate>>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong vaultId, Address owner)
        {
            VaultId = vaultId;
            Owner = owner;
        }

        public ulong VaultId { get; }
        public Address Owner { get; }
    }
}
