using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Certificates;

public class SelectVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandler
    : IRequestHandler<SelectVaultGovernanceCertificatesByVaultIdAndOwnerQuery, IEnumerable<VaultGovernanceCertificate>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultGovernanceCertificateEntity.Id)},
                {nameof(VaultGovernanceCertificateEntity.VaultGovernanceId)},
                {nameof(VaultGovernanceCertificateEntity.Owner)},
                {nameof(VaultGovernanceCertificateEntity.Amount)},
                {nameof(VaultGovernanceCertificateEntity.Revoked)},
                {nameof(VaultGovernanceCertificateEntity.Redeemed)},
                {nameof(VaultGovernanceCertificateEntity.VestedBlock)},
                {nameof(VaultGovernanceCertificateEntity.CreatedBlock)},
                {nameof(VaultGovernanceCertificateEntity.ModifiedBlock)}
            FROM vault_proposal_vote
            WHERE {nameof(VaultGovernanceCertificateEntity.VaultGovernanceId)} = @{nameof(SqlParams.VaultId)}
                AND {nameof(VaultGovernanceCertificateEntity.Owner)} = @{nameof(SqlParams.Owner)}".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultGovernanceCertificate>> Handle(SelectVaultGovernanceCertificatesByVaultIdAndOwnerQuery request,
                                                                      CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.VaultId, request.Owner), cancellationToken);

        var result = await _context.ExecuteQueryAsync<VaultGovernanceCertificateEntity>(query);

        return _mapper.Map<IEnumerable<VaultGovernanceCertificate>>(result);
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
