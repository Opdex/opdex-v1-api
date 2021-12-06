using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class VaultGovernanceDtoAssembler : IModelAssembler<VaultGovernance, VaultGovernanceDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultGovernanceDtoAssembler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<VaultGovernanceDto> Assemble(VaultGovernance vault)
    {
        var dto = _mapper.Map<VaultGovernanceDto>(vault);

        var token = await _mediator.Send(new RetrieveTokenByIdQuery(vault.TokenId));

        dto.Token = token.Address;
        dto.TokensUnassigned = vault.UnassignedSupply.ToDecimal(token.Decimals);
        dto.TokensProposed = vault.ProposedSupply.ToDecimal(token.Decimals);
        dto.TotalPledgeMinimum = vault.TotalPledgeMinimum.ToDecimal(TokenConstants.Cirrus.Decimals);
        dto.TotalVoteMinimum = vault.TotalVoteMinimum.ToDecimal(TokenConstants.Cirrus.Decimals);

        return dto;
    }
}
