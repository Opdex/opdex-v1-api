using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Votes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Votes;

public class SelectVaultProposalVotesWithFilterQueryHandler : IRequestHandler<SelectVaultProposalVotesWithFilterQuery, IEnumerable<VaultProposalVote>>
{
    private const string TableJoins = "{TableJoins}";
    private const string WhereFilter = "{WhereFilter}";
    private const string OrderBy = "{OrderBy}";
    private const string Limit = "{Limit}";

    private static readonly string SqlQuery =
        @$"SELECT
                v.{nameof(VaultProposalVoteEntity.Id)},
                v.{nameof(VaultProposalVoteEntity.VaultId)},
                v.{nameof(VaultProposalVoteEntity.ProposalId)},
                v.{nameof(VaultProposalVoteEntity.Voter)},
                v.{nameof(VaultProposalVoteEntity.Vote)},
                v.{nameof(VaultProposalVoteEntity.Balance)},
                v.{nameof(VaultProposalVoteEntity.InFavor)},
                v.{nameof(VaultProposalVoteEntity.CreatedBlock)},
                v.{nameof(VaultProposalVoteEntity.ModifiedBlock)}
            FROM vault_proposal_vote v
            {TableJoins}
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

        var queryParams = new SqlParams(voteId, request.VaultId, request.Cursor.ProposalId, request.Cursor.Voter);

        var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

        var results = await _context.ExecuteQueryAsync<VaultProposalVoteEntity>(query);

        return _mapper.Map<IEnumerable<VaultProposalVote>>(results);
    }

    private static string QueryBuilder(SelectVaultProposalVotesWithFilterQuery request)
    {
        var whereFilterBuilder = new StringBuilder();
        var joinsBuilder = new StringBuilder();

        var filterOnProposal = request.Cursor.ProposalId != 0;
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

            whereFilterBuilder.Append($" WHERE v.{nameof(VaultProposalVoteEntity.Id)} {sortOperator} @{nameof(SqlParams.VoteId)}");
        }

        whereFilterBuilder.Append(whereFilterBuilder.Length == 0 ? " WHERE" : " AND");
        whereFilterBuilder.Append($" v.{nameof(VaultProposalVoteEntity.VaultId)} = @{nameof(SqlParams.VaultId)}");

        if (filterOnProposal)
        {
            joinsBuilder.Append($" LEFT JOIN vault_proposal vp ON vp.{nameof(VaultProposalEntity.Id)} = v.{nameof(VaultProposalVoteEntity.ProposalId)}");
            whereFilterBuilder.Append($" AND vp.{nameof(VaultProposalEntity.PublicId)} = @{nameof(SqlParams.ProposalId)}");
        }

        if (filterOnVoter)
        {
            whereFilterBuilder.Append($" AND v.{nameof(VaultProposalVoteEntity.Voter)} = @{nameof(SqlParams.Voter)}");
        }

        if (!request.Cursor.IncludeZeroBalances)
        {
            whereFilterBuilder.Append($" AND v.{nameof(VaultProposalVoteEntity.Balance)} > 0");
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

        var orderBy = $" ORDER BY v.{nameof(VaultProposalVoteEntity.Id)} {direction}";

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
