using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Certificates;

public class SelectVaultCertificatesByModifiedBlockQueryHandler
    : IRequestHandler<SelectVaultCertificatesByModifiedBlockQuery, IEnumerable<VaultCertificate>>
{
    private static readonly string SqlQuery =
        $@"SELECT
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
            WHERE {nameof(VaultCertificateEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultCertificatesByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultCertificate>> Handle(SelectVaultCertificatesByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

        var result = await _context.ExecuteQueryAsync<VaultCertificateEntity>(query);

        return _mapper.Map<IEnumerable<VaultCertificate>>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong modifiedBlock)
        {
            ModifiedBlock = modifiedBlock;
        }

        public ulong ModifiedBlock { get; }
    }
}