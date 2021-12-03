using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class AddressAllowanceDtoAssembler : IModelAssembler<AddressAllowance, AddressAllowanceDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public AddressAllowanceDtoAssembler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<AddressAllowanceDto> Assemble(AddressAllowance source)
    {
        var addressAllowanceDto = _mapper.Map<AddressAllowanceDto>(source);

        var token = await _mediator.Send(new RetrieveTokenByIdQuery(source.TokenId));
        addressAllowanceDto.Allowance = source.Allowance.ToDecimal(token.Decimals);
        addressAllowanceDto.Token = token.Address;

        return addressAllowanceDto;
    }
}