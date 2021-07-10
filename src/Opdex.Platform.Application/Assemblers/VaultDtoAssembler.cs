using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.ODX;
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

        public async Task<VaultDto> Assemble(Vault source)
        {
            var vaultDto = _mapper.Map<VaultDto>(source);

            var token = await _mediator.Send(new RetrieveTokenByIdQuery(source.TokenId));
            var vaultBalance = await _mediator.Send(new RetrieveAddressBalanceByTokenIdAndOwnerQuery(token.Id, source.Address));

            // TODO: UnassignedSupply
            string unassignedSupply = "5000000000";

            vaultDto.TokensLocked = vaultBalance.Balance.InsertDecimal(TokenConstants.Cirrus.Decimals);
            vaultDto.TokensUnassigned = unassignedSupply.InsertDecimal(token.Decimals);
            vaultDto.LockedToken = token.Address;
            return vaultDto;
        }
    }
}
