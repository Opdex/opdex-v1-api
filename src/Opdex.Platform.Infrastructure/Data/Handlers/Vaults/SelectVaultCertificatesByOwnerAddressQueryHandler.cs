using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults
{
    public class SelectVaultCertificatesByOwnerAddressQueryHandler : IRequestHandler<SelectVaultCertificatesByOwnerAddressQuery, IEnumerable<VaultCertificate>>
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
            FROM odx_vault_certificate
            WHERE {nameof(VaultCertificateEntity.Owner)} = @{nameof(SqlParams.Owner)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public SelectVaultCertificatesByOwnerAddressQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<IEnumerable<VaultCertificate>> Handle(SelectVaultCertificatesByOwnerAddressQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.OwnerAddress);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<VaultCertificateEntity>(query);

            return _mapper.Map<IEnumerable<VaultCertificate>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(string owner)
            {
                Owner = owner;
            }

            public string Owner { get; }
        }
    }
}