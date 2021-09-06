using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;
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
            {Limit}".RemoveExcessWhitespace();

        private const string InnerQuery = "{InnerQuery}";
        private const string SortDirection = "{SortDirection}";

        private static readonly string PagingBackwardQuery =
            @$"SELECT * FROM ({InnerQuery}) r ORDER BY r.{nameof(VaultCertificateEntity.Id)} {SortDirection};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectVaultCertificatesWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VaultCertificate>> Handle(SelectVaultCertificatesWithFilterQuery request, CancellationToken cancellationToken)
        {
            var certificateId = request.Cursor.Pointer;

            var queryParams = new SqlParams(certificateId, request.VaultId, request.Cursor.Holder);

            var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

            var results = await _context.ExecuteQueryAsync<VaultCertificateEntity>(query);

            return _mapper.Map<IList<VaultCertificate>>(results);
        }

        private string QueryBuilder(SelectVaultCertificatesWithFilterQuery request)
        {
            var whereFilter = string.Empty;

            if (!request.Cursor.IsFirstRequest)
            {
                var sortOperator = string.Empty;

                // going forward in ascending order, use greater than
                if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = ">";

                // going forward in descending order, use less than or equal to
                if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = "<";

                // going backward in ascending order, use less than
                if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = "<";

                // going backward in descending order, use greater than
                if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = ">";

                whereFilter = $" WHERE c.{nameof(VaultCertificateEntity.Id)} {sortOperator} @{nameof(SqlParams.CertificateId)}";
            }

            var filter = $"c.`{nameof(VaultCertificateEntity.VaultId)}` = @{nameof(SqlParams.VaultId)}";
            whereFilter += whereFilter.HasValue() ? $" AND {filter}" : $" WHERE {filter}";

            if (request.Cursor.Holder.HasValue())
            {
                whereFilter += $" AND c.`{nameof(VaultCertificateEntity.Owner)}` = @{nameof(SqlParams.Holder)}";
            }

            // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
            string direction;

            if (request.Cursor.PagingDirection == PagingDirection.Backward)
            {
                direction = request.Cursor.SortDirection == SortDirectionType.DESC ? nameof(SortDirectionType.ASC) : nameof(SortDirectionType.DESC);
            }
            else
            {
                direction = Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection);
            }

            var orderBy = $" GROUP BY c.{nameof(VaultCertificateEntity.Id)} ORDER BY c.{nameof(VaultCertificateEntity.Id)} {direction}";

            var limit = $" LIMIT {request.Cursor.Limit + 1}";

            var query = SqlQuery.Replace(WhereFilter, whereFilter)
                                .Replace(OrderBy, orderBy)
                                .Replace(Limit, limit);

            if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";
            // re-sort back into requested order
            else return PagingBackwardQuery.Replace(InnerQuery, query)
                                           .Replace(SortDirection, Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection));
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
