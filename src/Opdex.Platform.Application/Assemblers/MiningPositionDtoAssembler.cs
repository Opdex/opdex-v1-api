using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class MiningPositionDtoAssembler : IModelAssembler<AddressMining, MiningPositionDto>
{
    private readonly IMediator _mediator;

    public MiningPositionDtoAssembler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<MiningPositionDto> Assemble(AddressMining source)
    {
        var miningPool = await _mediator.Send(new RetrieveMiningPoolByIdQuery(source.MiningPoolId, findOrThrow: true));
        var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByIdQuery(miningPool.LiquidityPoolId, findOrThrow: true));
        var token = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.LpTokenId, findOrThrow: true));

        return new MiningPositionDto
        {
            Address = source.Owner,
            Amount = source.Balance.ToDecimal(token.Decimals),
            MiningPool = miningPool.Address,
            MiningToken = token.Address
        };
    }
}