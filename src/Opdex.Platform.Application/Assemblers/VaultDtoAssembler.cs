using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Vaults;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers
{
    public class VaultDtoAssembler : IModelAssembler<Vault, VaultDto>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public VaultDtoAssembler(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<VaultDto> Assemble(Vault vault)
        {
            var vaultDto = _mapper.Map<VaultDto>(vault);

            var token = await _mediator.Send(new RetrieveTokenByIdQuery(vault.TokenId));
            var vaultBalance = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(vault.Address, token.Id));

            vaultDto.TokensLocked = vaultBalance.Balance.ToDecimal(token.Decimals);
            vaultDto.TokensUnassigned = vault.UnassignedSupply.ToDecimal(token.Decimals);
            vaultDto.LockedToken = token.Address;
            return vaultDto;
        }
    }
}
