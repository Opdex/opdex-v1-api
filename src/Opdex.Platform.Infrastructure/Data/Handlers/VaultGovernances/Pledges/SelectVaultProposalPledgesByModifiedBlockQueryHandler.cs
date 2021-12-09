using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Pledges;

public class SelectVaultProposalPledgesByModifiedBlockQueryHandler : IRequestHandler<SelectVaultProposalPledgesByModifiedBlockQuery, IEnumerable<VaultProposalPledge>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultProposalPledgeEntity.Id)},
                {nameof(VaultProposalPledgeEntity.VaultGovernanceId)},
                {nameof(VaultProposalPledgeEntity.ProposalId)},
                {nameof(VaultProposalPledgeEntity.Pledger)},
                {nameof(VaultProposalPledgeEntity.Pledge)},
                {nameof(VaultProposalPledgeEntity.Balance)},
                {nameof(VaultProposalPledgeEntity.CreatedBlock)},
                {nameof(VaultProposalPledgeEntity.ModifiedBlock)}
            FROM vault_proposal_pledge
            WHERE {nameof(VaultProposalPledgeEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public  SelectVaultProposalPledgesByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultProposalPledge>> Handle(SelectVaultProposalPledgesByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

        var result = await _context.ExecuteQueryAsync<VaultProposalPledgeEntity>(query);

        return _mapper.Map<IEnumerable<VaultProposalPledge>>(result);
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
