using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Certificates;

public class SelectVaultCertificateByIdQueryHandler : IRequestHandler<SelectVaultCertificateByIdQuery, VaultCertificate>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultCertificateEntity.Id)},
                {nameof(VaultCertificateEntity.VaultId)},
                {nameof(VaultCertificateEntity.Owner)},
                {nameof(VaultCertificateEntity.Amount)},
                {nameof(VaultCertificateEntity.VestedBlock)},
                {nameof(VaultCertificateEntity.Redeemed)},
                {nameof(VaultCertificateEntity.Revoked)},
                {nameof(VaultCertificateEntity.CreatedBlock)},
                {nameof(VaultCertificateEntity.ModifiedBlock)}
            FROM vault_certificate
            WHERE {nameof(VaultCertificateEntity.Id)} = @{nameof(SqlParams.CertificateId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultCertificateByIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<VaultCertificate> Handle(SelectVaultCertificateByIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.CertificateId), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultCertificateEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException("Vault certificate not found.");
        }

        return result == null ? null : _mapper.Map<VaultCertificate>(result);
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
