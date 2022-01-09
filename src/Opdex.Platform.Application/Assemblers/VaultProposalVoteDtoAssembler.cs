using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Vaults;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class VaultProposalVoteDtoAssembler : IModelAssembler<VaultProposalVote, VaultProposalVoteDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultProposalVoteDtoAssembler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<VaultProposalVoteDto> Assemble(VaultProposalVote vote)
    {
        var dto = _mapper.Map<VaultProposalVoteDto>(vote);

        var vault = await _mediator.Send(new RetrieveVaultGovernanceByIdQuery(vote.VaultId));
        var proposal = await _mediator.Send(new RetrieveVaultProposalByIdQuery(vote.ProposalId));

        dto.Vault = vault.Address;
        dto.ProposalId = proposal.PublicId;
        dto.Vote = vote.Vote.ToDecimal(TokenConstants.Cirrus.Decimals);
        dto.Balance = vote.Balance.ToDecimal(TokenConstants.Cirrus.Decimals);
        return dto;
    }
}
