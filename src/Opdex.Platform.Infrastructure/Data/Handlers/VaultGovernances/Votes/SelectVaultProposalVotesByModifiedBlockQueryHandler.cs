using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Votes;

public class SelectVaultProposalVotesByModifiedBlockQueryHandler : IRequestHandler<SelectVaultProposalVotesByModifiedBlockQuery, IEnumerable<VaultProposalVote>>
{
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
            WHERE {nameof(VaultProposalVoteEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public  SelectVaultProposalVotesByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultProposalVote>> Handle(SelectVaultProposalVotesByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

        var result = await _context.ExecuteQueryAsync<VaultProposalVoteEntity>(query);

        return _mapper.Map<IEnumerable<VaultProposalVote>>(result);
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
