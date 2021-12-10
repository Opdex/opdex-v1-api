using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Proposals;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Proposals;

public class SelectVaultProposalsByModifiedBlockQueryHandler : IRequestHandler<SelectVaultProposalsByModifiedBlockQuery, IEnumerable<VaultProposal>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultProposalEntity.Id)},
                {nameof(VaultProposalEntity.PublicId)},
                {nameof(VaultProposalEntity.VaultGovernanceId)},
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
            WHERE {nameof(VaultProposalEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public  SelectVaultProposalsByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<VaultProposal>> Handle(SelectVaultProposalsByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

        var result = await _context.ExecuteQueryAsync<VaultProposalEntity>(query);

        return _mapper.Map<IEnumerable<VaultProposal>>(result);
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
