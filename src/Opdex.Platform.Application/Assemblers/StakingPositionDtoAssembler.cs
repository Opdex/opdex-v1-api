using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers
{
    public class StakingPositionDtoAssembler : IModelAssembler<AddressStaking, StakingPositionDto>
    {
        private readonly IMediator _mediator;

        public StakingPositionDtoAssembler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<StakingPositionDto> Assemble(AddressStaking source)
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByIdQuery(source.LiquidityPoolId, findOrThrow: true));
            var market = await _mediator.Send(new RetrieveMarketByIdQuery(liquidityPool.MarketId, findOrThrow: true));
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(market.StakingTokenId.Value, findOrThrow: true));

            return new StakingPositionDto
            {
                Address = source.Owner,
                Amount = source.Weight.ToDecimal(token.Decimals),
                LiquidityPool = liquidityPool.Address,
                StakingToken = token.Address
            };
        }
    }
}
