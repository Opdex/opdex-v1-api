using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class AddressBalanceDtoAssembler : IModelAssembler<AddressBalance, AddressBalanceDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public AddressBalanceDtoAssembler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<AddressBalanceDto> Assemble(AddressBalance source)
    {
        var dto = _mapper.Map<AddressBalanceDto>(source);

        var token = await _mediator.Send(new RetrieveTokenByIdQuery(source.TokenId));
        dto.Balance = source.Balance.ToDecimal(token.Decimals);
        dto.Token = token.Address;

        return dto;
    }
}