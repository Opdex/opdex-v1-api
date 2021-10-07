using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
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

            // vault may not have a balance just yet if it was just created
            var vaultBalance = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(vault.Address, token.Id, findOrThrow: false));

            vaultDto.TokensLocked = vaultBalance == null
                ? UInt256.Zero.ToDecimal(token.Decimals)
                : vaultBalance.Balance.ToDecimal(token.Decimals);
            vaultDto.TokensUnassigned = vault.UnassignedSupply.ToDecimal(token.Decimals);
            vaultDto.LockedToken = token.Address;
            return vaultDto;
        }
    }
}
