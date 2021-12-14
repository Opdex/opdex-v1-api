using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Votes;

public class SelectVaultProposalVotesWithFilterQueryHandler : IRequestHandler<SelectVaultProposalVotesWithFilterQuery, IEnumerable<VaultProposalVote>>
{
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultProposalVoteEntity.Id)},
                {nameof(VaultProposalVoteEntity.VaultGovernanceId)},
                {nameof(VaultProposalVoteEntity.ProposalId)},
                {nameof(VaultProposalVoteEntity.Voter)},
                {nameof(VaultProposalVoteEntity.Vote)},
                {nameof(VaultProposalVoteEntity.Balance)},
                {nameof(VaultProposalVoteEntity.InFavor)},
                {nameof(VaultProposalVoteEntity.CreatedBlock)},
                {nameof(VaultProposalVoteEntity.ModifiedBlock)}
            FROM vault_proposal_vote
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

    private const string InnerQuery = "{InnerQuery}";
    private const string SortDirection = "{SortDirection}";

    private static readonly string PagingBackwardQuery =
        @$"SELECT * FROM ({InnerQuery}) results ORDER BY results.{nameof(VaultProposalVoteEntity.Id)} {SortDirection};";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    public SelectVaultProposalVotesWithFilterQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<IEnumerable<VaultProposalVote>> Handle(SelectVaultProposalVotesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var voteId = request.Cursor.Pointer;

        var queryParams = new SqlParams(voteId, request.VaultId, request.ProposalId, request.Cursor.Voter);

        var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<VaultProposalVoteEntity>(query);

        return _mapper.Map<IEnumerable<VaultProposalVote>>(results);
    }

    private static string QueryBuilder(SelectVaultProposalVotesWithFilterQuery request)
    {
        var whereFilterBuilder = new StringBuilder();

        var filterOnVoter = request.Cursor.Voter != Address.Empty;

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

            whereFilterBuilder.Append($" WHERE {nameof(VaultProposalVoteEntity.Id)} {sortOperator} @{nameof(SqlParams.VoteId)}");
        }

        whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
        whereFilterBuilder.Append($" {nameof(VaultProposalVoteEntity.VaultGovernanceId)} = @{nameof(SqlParams.VaultId)}");
        whereFilterBuilder.Append($" AND {nameof(VaultProposalVoteEntity.ProposalId)} = @{nameof(SqlParams.ProposalId)}");

        if (filterOnVoter)
        {
            whereFilterBuilder.Append($" AND {nameof(VaultProposalVoteEntity.Voter)} = @{nameof(SqlParams.Voter)}");
        }

        if (!request.Cursor.IncludeZeroBalances)
        {
            whereFilterBuilder.Append($" AND {nameof(VaultProposalVoteEntity.Balance)} > 0");
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

        var orderBy = $" ORDER BY {nameof(VaultProposalVoteEntity.Id)} {direction}";

        var limit = $" LIMIT {request.Cursor.Limit + 1}";

        var query = SqlQuery.Replace(WhereFilter, whereFilterBuilder.ToString())
                            .Replace(OrderBy, orderBy)
                            .Replace(Limit, limit);

        if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";
        // re-sort back into requested order
        else return PagingBackwardQuery.Replace(InnerQuery, query)
            .Replace(SortDirection, Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection));
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong voteId, ulong vaultId, ulong proposalId, Address voter)
        {
            VoteId = voteId;
            VaultId = vaultId;
            ProposalId = proposalId;
            Voter = voter;
        }

        public ulong VoteId { get; }
        public ulong VaultId { get; }
        public ulong ProposalId { get; }
        public Address Voter { get; }
    }
}
