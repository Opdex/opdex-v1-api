using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults
{
    public class SelectVaultCertificatesWithFilterQueryHandler : IRequestHandler<SelectVaultCertificatesWithFilterQuery, IEnumerable<VaultCertificate>>
    {
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

        private static readonly string SqlQuery =
            @$"SELECT
                c.{nameof(VaultCertificateEntity.Id)},
                c.{nameof(VaultCertificateEntity.VaultId)},
                c.{nameof(VaultCertificateEntity.Owner)},
                c.{nameof(VaultCertificateEntity.Amount)},
                c.{nameof(VaultCertificateEntity.VestedBlock)},
                c.{nameof(VaultCertificateEntity.Redeemed)},
                c.{nameof(VaultCertificateEntity.Revoked)},
                c.{nameof(VaultCertificateEntity.CreatedBlock)},
                c.{nameof(VaultCertificateEntity.ModifiedBlock)}
            FROM vault_certificate c
            {WhereFilter}
            {OrderBy}
            {Limit};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectVaultCertificatesWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VaultCertificate>> Handle(SelectVaultCertificatesWithFilterQuery request, CancellationToken cancellationToken)
        {
            var certificateId = request.Next > 0 ? request.Next : request.Previous;

            var queryParams = new SqlParams(certificateId, request.VaultId, request.Holder);

            var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

            var results = await _context.ExecuteQueryAsync<VaultCertificateEntity>(query);

            // re-sort back into correct order
            if (request.Previous > 0)
            {
                results = request.Direction == SortDirectionType.ASC
                    ? results.OrderBy(t => t.Id)
                    : results.OrderByDescending(t => t.Id);
            }

            return _mapper.Map<IList<VaultCertificate>>(results);
        }

        private string QueryBuilder(SelectVaultCertificatesWithFilterQuery request)
        {
            var whereFilter = string.Empty;

            var sortOperator = string.Empty;

            // going forward in ascending order, use greater than
            if (request.Next > 0 && request.Direction == SortDirectionType.ASC) sortOperator = ">";

            // going forward in descending order, use less than or equal to
            if (request.Next > 0 && request.Direction == SortDirectionType.DESC) sortOperator = "<";

            // going backward in ascending order, use less than
            if (request.Previous > 0 && request.Direction == SortDirectionType.ASC) sortOperator = "<";

            // going backward in descending order, use greater than
            if (request.Previous > 0 && request.Direction == SortDirectionType.DESC) sortOperator = ">";

            if (sortOperator.HasValue())
            {
                whereFilter = $" WHERE c.{nameof(VaultCertificateEntity.Id)} {sortOperator} @{nameof(SqlParams.CertificateId)}";
            }

            var filter = $"c.`{nameof(VaultCertificateEntity.VaultId)}` = @{nameof(SqlParams.VaultId)}";
            whereFilter += whereFilter.HasValue() ? $" AND {filter}" : $" WHERE {filter}";

            if (request.Holder.HasValue())
            {
                whereFilter += $" AND c.`{nameof(VaultCertificateEntity.Owner)}` = @{nameof(SqlParams.Holder)}";
            }

            // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
            string direction;

            if (request.Previous > 0)
            {
                direction = request.Direction == SortDirectionType.DESC ? nameof(SortDirectionType.ASC) : nameof(SortDirectionType.DESC);
            }
            else
            {
                direction = Enum.GetName(typeof(SortDirectionType), request.Direction);
            }

            var orderBy = $" GROUP BY c.{nameof(VaultCertificateEntity.Id)} ORDER BY c.{nameof(VaultCertificateEntity.Id)} {direction}";

            var limit = $" LIMIT {request.Limit + 1}";

            return SqlQuery
                .Replace(WhereFilter, whereFilter)
                .Replace(OrderBy, orderBy)
                .Replace(Limit, limit);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long certificateId, long vaultId, string holder)
            {
                CertificateId = certificateId;
                VaultId = vaultId;
                Holder = holder;
            }

            public long CertificateId { get; }
            public long VaultId { get; }
            public string Holder { get; }
        }
    }
}
