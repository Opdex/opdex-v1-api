using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Vaults;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class VaultDtoAssembler : IModelAssembler<VaultGovernance, VaultGovernanceDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultDtoAssembler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<VaultGovernanceDto> Assemble(VaultGovernance vault)
    {
        var dto = _mapper.Map<VaultGovernanceDto>(vault);

        var token = await _mediator.Send(new RetrieveTokenByIdQuery(vault.TokenId));

        // vault may not have a balance yet if it was just created
        var vaultTokenBalance = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(vault.Address, token.Id, findOrThrow: false));

        dto.Token = token.Address;
        dto.TokensUnassigned = vault.UnassignedSupply.ToDecimal(token.Decimals);
        dto.TokensProposed = vault.ProposedSupply.ToDecimal(token.Decimals);
        dto.TokensLocked = vaultTokenBalance is null
            ? UInt256.Zero.ToDecimal(token.Decimals)
            : vaultTokenBalance.Balance.ToDecimal(token.Decimals);
        dto.TotalPledgeMinimum = vault.TotalPledgeMinimum.ToDecimal(TokenConstants.Cirrus.Decimals);
        dto.TotalVoteMinimum = vault.TotalVoteMinimum.ToDecimal(TokenConstants.Cirrus.Decimals);

        return dto;
    }
}
