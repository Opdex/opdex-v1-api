using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Staking
{
    public class GetStakingPositionByPoolQueryHandler : IRequestHandler<GetStakingPositionByPoolQuery, StakingPositionDto>
    {
        private readonly IMediator _mediator;

        public GetStakingPositionByPoolQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<StakingPositionDto> Handle(GetStakingPositionByPoolQuery request, CancellationToken cancellationToken)
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPoolAddress, findOrThrow: true), cancellationToken);
            var addressStaking = await _mediator.Send(new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(liquidityPool.Id,
                                                                                                               request.Address,
                                                                                                               findOrThrow: true), cancellationToken);
            var market = await _mediator.Send(new RetrieveMarketByIdQuery(liquidityPool.MarketId, findOrThrow: true), cancellationToken);
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(market.StakingTokenId.Value, findOrThrow: true), cancellationToken);

            return new StakingPositionDto
            {
                Address = addressStaking.Owner,
                Amount = addressStaking.Weight.ToDecimal(token.Decimals),
                LiquidityPool = request.LiquidityPoolAddress,
                StakingToken = token.Address
            };
        }
    }
}
