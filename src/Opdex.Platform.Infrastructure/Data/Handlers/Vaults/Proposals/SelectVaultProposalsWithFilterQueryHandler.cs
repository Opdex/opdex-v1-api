using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Proposals;

public class SelectVaultProposalsWithFilterQueryHandler : IRequestHandler<SelectVaultProposalsWithFilterQuery, IEnumerable<VaultProposal>>
{
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultProposalEntity.Id)},
                {nameof(VaultProposalEntity.PublicId)},
                {nameof(VaultProposalEntity.VaultId)},
                {nameof(VaultProposalEntity.Creator)},
                {nameof(VaultProposalEntity.Wallet)},
                {nameof(VaultProposalEntity.Amount)},
                {nameof(VaultProposalEntity.Description)},
                {nameof(VaultProposalEntity.ProposalTypeId)},
                {nameof(VaultProposalEntity.ProposalStatusId)},
                {nameof(VaultProposalEntity.Expiration)},
                {nameof(VaultProposalEntity.YesAmount)},
                {nameof(VaultProposalEntity.NoAmount)},
                {nameof(VaultProposalEntity.PledgeAmount)},
                {nameof(VaultProposalEntity.Approved)},
                {nameof(VaultProposalEntity.CreatedBlock)},
                {nameof(VaultProposalEntity.ModifiedBlock)}
            FROM vault_proposal
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string SortDirection = "{SortDirection}";

    private static readonly string PagingBackwardQuery =
        @$"SELECT * FROM ({InnerQuery}) results ORDER BY results.{nameof(VaultProposalEntity.Expiration)} {SortDirection}, results.{nameof(VaultProposalEntity.PublicId)} {SortDirection};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultProposalsWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultProposal>> Handle(SelectVaultProposalsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var queryParams = new SqlParams(request.Cursor.Pointer, request.Cursor.Status, request.Cursor.Type, request.VaultId);

        var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<VaultProposalEntity>(query);

        return _mapper.Map<IEnumerable<VaultProposal>>(results);
    }

    private static string QueryBuilder(SelectVaultProposalsWithFilterQuery request)
    {
        var whereFilterBuilder = new StringBuilder();

        if (!request.Cursor.IsFirstRequest)
        {
            var sortOperator = request.Cursor.PagingDirection switch
            {
                // going forward in ascending order, use greater than
                PagingDirection.Forward when request.Cursor.SortDirection == SortDirectionType.ASC => ">",
                // going backward in descending order, use greater than
                PagingDirection.Backward when request.Cursor.SortDirection == SortDirectionType.DESC => ">",
                // going forward in descending order, use less than or equal to
                PagingDirection.Forward when request.Cursor.SortDirection == SortDirectionType.DESC => "<",
                // going backward in ascending order, use less than
                PagingDirection.Backward when request.Cursor.SortDirection == SortDirectionType.ASC => "<",
                _ => throw new ArgumentException("Unrecognised paging and sorting direction.", nameof(request))
            };

            whereFilterBuilder.Append($" WHERE ({nameof(VaultProposalEntity.Expiration)}, {nameof(VaultProposalEntity.PublicId)}) {sortOperator} (@{nameof(SqlParams.ExpirationPointer)}, @{nameof(SqlParams.PublicIdPointer)})");
        }

        whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
        whereFilterBuilder.Append($" {nameof(VaultProposalEntity.VaultId)} = @{nameof(SqlParams.VaultId)}");

        if (request.Cursor.Status != default)
        {
            whereFilterBuilder.Append($" AND {nameof(VaultProposalEntity.ProposalStatusId)} = @{nameof(SqlParams.Status)}");
        }

        if (request.Cursor.Type != default)
        {
            whereFilterBuilder.Append($" AND {nameof(VaultProposalEntity.ProposalTypeId)} = @{nameof(SqlParams.Type)}");
        }

        // Set the direction, moving backwards with previous requests, the sort order must be reversed first.

        string direction = request.Cursor.PagingDirection switch
        {
            PagingDirection.Backward => request.Cursor.SortDirection == SortDirectionType.DESC
                ? nameof(SortDirectionType.ASC)
                : nameof(SortDirectionType.DESC),
            PagingDirection.Forward => Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection),
            _ => throw new ArgumentException("Unrecognised paging direction.", nameof(request))
        };

        var orderBy = $" ORDER BY {nameof(VaultProposalEntity.Expiration)} {direction}, {nameof(VaultProposalEntity.PublicId)} {direction}";

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
        public SqlParams((ulong, ulong) pointer, VaultProposalStatus status, VaultProposalType type, ulong vaultId)
        {
            ExpirationPointer = pointer.Item1;
            PublicIdPointer = pointer.Item2;
            Status = status;
            Type = type;
            VaultId = vaultId;
        }

        public ulong ExpirationPointer { get; }
        public ulong PublicIdPointer { get; }
        public VaultProposalStatus Status { get; }
        public VaultProposalType Type { get; }
        public ulong VaultId { get; }
    }
}
