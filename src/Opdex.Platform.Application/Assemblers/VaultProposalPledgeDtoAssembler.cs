using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class VaultProposalPledgeDtoAssembler : IModelAssembler<VaultProposalPledge, VaultProposalPledgeDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultProposalPledgeDtoAssembler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<VaultProposalPledgeDto> Assemble(VaultProposalPledge pledge)
    {
        var dto = _mapper.Map<VaultProposalPledgeDto>(pledge);

        var vault = await _mediator.Send(new RetrieveVaultGovernanceByIdQuery(pledge.VaultId));
        var proposal = await _mediator.Send(new RetrieveVaultProposalByIdQuery(pledge.ProposalId));

        dto.Vault = vault.Address;
        dto.ProposalId = proposal.PublicId;
        dto.Pledge = pledge.Pledge.ToDecimal(TokenConstants.Cirrus.Decimals);
        dto.Balance = pledge.Balance.ToDecimal(TokenConstants.Cirrus.Decimals);
        return dto;
    }
}
