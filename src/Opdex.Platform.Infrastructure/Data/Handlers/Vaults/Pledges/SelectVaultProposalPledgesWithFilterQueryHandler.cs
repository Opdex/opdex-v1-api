using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Pledges;

public class SelectVaultProposalPledgesWithFilterQueryHandler : IRequestHandler<SelectVaultProposalPledgesWithFilterQuery, IEnumerable<VaultProposalPledge>>
{
    private const string TableJoins = "{TableJoins}";
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        @$"SELECT
                p.{nameof(VaultProposalPledgeEntity.Id)},
                p.{nameof(VaultProposalPledgeEntity.VaultId)},
                p.{nameof(VaultProposalPledgeEntity.ProposalId)},
                p.{nameof(VaultProposalPledgeEntity.Pledger)},
                p.{nameof(VaultProposalPledgeEntity.Pledge)},
                p.{nameof(VaultProposalPledgeEntity.Balance)},
                p.{nameof(VaultProposalPledgeEntity.CreatedBlock)},
                p.{nameof(VaultProposalPledgeEntity.ModifiedBlock)}
            FROM vault_proposal_pledge p
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string SortDirection = "{SortDirection}";

    private static readonly string PagingBackwardQuery =
        @$"SELECT * FROM ({InnerQuery}) results ORDER BY results.{nameof(VaultProposalPledgeEntity.Id)} {SortDirection};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultProposalPledgesWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultProposalPledge>> Handle(SelectVaultProposalPledgesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var pledgeId = request.Cursor.Pointer;

        var queryParams = new SqlParams(pledgeId, request.VaultId, request.Cursor.ProposalId, request.Cursor.Pledger);

        var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<VaultProposalPledgeEntity>(query);

        return _mapper.Map<IEnumerable<VaultProposalPledge>>(results);
    }

    private static string QueryBuilder(SelectVaultProposalPledgesWithFilterQuery request)
    {
        var whereFilterBuilder = new StringBuilder();
        var joinsBuilder = new StringBuilder();

        var filterOnProposal = request.Cursor.ProposalId != 0;
        var filterOnPledger = request.Cursor.Pledger != Address.Empty;

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

            whereFilterBuilder.Append($" WHERE {nameof(VaultProposalPledgeEntity.Id)} {sortOperator} @{nameof(SqlParams.PledgeId)}");
        }

        whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
        whereFilterBuilder.Append($" p.{nameof(VaultProposalPledgeEntity.VaultId)} = @{nameof(SqlParams.VaultId)}");

        if (filterOnProposal)
        {
            joinsBuilder.Append($" LEFT JOIN vault_proposal vp ON vp.{nameof(VaultProposalEntity.Id)} = p.{nameof(VaultProposalPledgeEntity.ProposalId)}");
            whereFilterBuilder.Append($" AND vp.{nameof(VaultProposalEntity.PublicId)} = @{nameof(SqlParams.ProposalId)}");
        }

        if (filterOnPledger)
        {
            whereFilterBuilder.Append($" AND p.{nameof(VaultProposalPledgeEntity.Pledger)} = @{nameof(SqlParams.Pledger)}");
        }

        if (!request.Cursor.IncludeZeroBalances)
        {
            whereFilterBuilder.Append($" AND p.{nameof(VaultProposalPledgeEntity.Balance)} > 0");
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

        var orderBy = $" ORDER BY p.{nameof(VaultProposalPledgeEntity.Id)} {direction}";

        var limit = $" LIMIT {request.Cursor.Limit + 1}";

        var query = SqlQuery.Replace(TableJoins, joinsBuilder.ToString())
                            .Replace(WhereFilter, whereFilterBuilder.ToString())
                            .Replace(OrderBy, orderBy)
                            .Replace(Limit, limit);

        if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";
        // re-sort back into requested order

        return PagingBackwardQuery.Replace(InnerQuery, query)
            .Replace(SortDirection, Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection));
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong pledgeId, ulong vaultId, ulong proposalId, Address pledger)
        {
            PledgeId = pledgeId;
            VaultId = vaultId;
            ProposalId = proposalId;
            Pledger = pledger;
        }

        public ulong PledgeId { get; }
        public ulong VaultId { get; }
        public ulong ProposalId { get; }
        public Address Pledger { get; }
    }
}
