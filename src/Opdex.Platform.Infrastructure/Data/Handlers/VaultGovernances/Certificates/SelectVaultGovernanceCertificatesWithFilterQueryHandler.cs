using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Certificates;

public class SelectVaultGovernanceCertificatesWithFilterQueryHandler : IRequestHandler<SelectVaultGovernanceCertificatesWithFilterQuery, IEnumerable<VaultCertificate>>
{
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        @$"SELECT
                c.{nameof(VaultCertificateEntity.Id)},
                c.{nameof(VaultCertificateEntity.VaultId)},
                c.{nameof(VaultCertificateEntity.ProposalId)},
                c.{nameof(VaultCertificateEntity.Owner)},
                c.{nameof(VaultCertificateEntity.Amount)},
                c.{nameof(VaultCertificateEntity.VestedBlock)},
                c.{nameof(VaultCertificateEntity.Redeemed)},
                c.{nameof(VaultCertificateEntity.Revoked)},
                c.{nameof(VaultCertificateEntity.CreatedBlock)},
                c.{nameof(VaultCertificateEntity.ModifiedBlock)}
            FROM vault_governance_certificate c
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string SortDirection = "{SortDirection}";

    private static readonly string PagingBackwardQuery =
        @$"SELECT * FROM ({InnerQuery}) results ORDER BY results.{nameof(VaultCertificateEntity.Id)} {SortDirection};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultGovernanceCertificatesWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultCertificate>> Handle(SelectVaultGovernanceCertificatesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.Cursor.Pointer, request.VaultId, request.Cursor.Holder);

        var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<VaultCertificateEntity>(query);

        return _mapper.Map<IEnumerable<VaultCertificate>>(results);
    }

    private string QueryBuilder(SelectVaultGovernanceCertificatesWithFilterQuery request)
    {
        var whereFilterBuilder = new StringBuilder();

        if (!request.Cursor.IsFirstRequest)
        {
            var sortOperator = request.Cursor.PagingDirection switch
            {
                // going backward in descending order, use greater than
                PagingDirection.Backward when request.Cursor.SortDirection == SortDirectionType.DESC => ">",
                _ => request.Cursor.PagingDirection switch
                {
                    // going forward in descending order, use less than or equal to
                    PagingDirection.Forward when request.Cursor.SortDirection == SortDirectionType.DESC => "<",
                    // going backward in ascending order, use less than
                    PagingDirection.Backward when request.Cursor.SortDirection == SortDirectionType.ASC => "<",
                    _ => request.Cursor.PagingDirection switch
                    {
                        // going forward in ascending order, use greater than
                        PagingDirection.Forward when request.Cursor.SortDirection == SortDirectionType.ASC => ">",
                        _ => string.Empty
                    }
                }
            };

            whereFilterBuilder.Append($" WHERE c.{nameof(VaultCertificateEntity.Id)} {sortOperator} @{nameof(SqlParams.IdPointer)}");
        }

        whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
        whereFilterBuilder.Append($" c.{nameof(VaultCertificateEntity.VaultId)} = @{nameof(SqlParams.VaultId)}");

        if (request.Cursor.Holder != Address.Empty)
        {
            whereFilterBuilder.Append($" AND c.{nameof(VaultCertificateEntity.Owner)} = @{nameof(SqlParams.Holder)}");
        }

        switch (request.Cursor.Status)
        {
            case VaultCertificateStatusFilter.Vesting:
                whereFilterBuilder.Append($" AND c.{nameof(VaultCertificateEntity.Redeemed)} = false");
                break;
            case VaultCertificateStatusFilter.Redeemed:
                whereFilterBuilder.Append($" AND c.{nameof(VaultCertificateEntity.Redeemed)} = true");
                break;
            case VaultCertificateStatusFilter.Revoked:
                whereFilterBuilder.Append($" AND c.{nameof(VaultCertificateEntity.Revoked)} = true");
                break;
        }

        // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
        string direction = request.Cursor.PagingDirection switch
        {
            PagingDirection.Backward => request.Cursor.SortDirection == SortDirectionType.DESC
                ? nameof(SortDirectionType.ASC)
                : nameof(SortDirectionType.DESC),
            _ => Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection)
        };

        var orderBy = $" GROUP BY c.{nameof(VaultCertificateEntity.Id)} ORDER BY c.{nameof(VaultCertificateEntity.Id)} {direction}";

        var limit = $" LIMIT {request.Cursor.Limit + 1}";

        var query = SqlQuery.Replace(WhereFilter, whereFilterBuilder.ToString())
            .Replace(OrderBy, orderBy)
            .Replace(Limit, limit);

        return request.Cursor.PagingDirection switch
        {
            PagingDirection.Forward => $"{query};",
            _ => PagingBackwardQuery.Replace(InnerQuery, query)
                .Replace(SortDirection, Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection))
        };
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong idPointer, ulong vaultId, Address holder)
        {
            IdPointer = idPointer;
            VaultId = vaultId;
            Holder = holder;
        }

        public ulong IdPointer { get; }
        public ulong VaultId { get; }
        public Address Holder { get; }
    }
}
