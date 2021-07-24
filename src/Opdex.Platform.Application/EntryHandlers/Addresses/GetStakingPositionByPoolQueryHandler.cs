using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses
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
            var liqudityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPoolAddress, findOrThrow: true), cancellationToken);
            var addressStaking = await _mediator.Send(new RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(liqudityPool.Id,
                                                                                                               request.Address,
                                                                                                               findOrThrow: true), cancellationToken);

            // TODO: retrieve staking token
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(liqudityPool.LpTokenId), cancellationToken);

            return new StakingPositionDto
            {
                Address = addressStaking.Owner,
                Amount = addressStaking.Weight.InsertDecimal(token.Decimals),
                LiquidityPool = request.LiquidityPoolAddress,
                StakingToken = token.Address
            };
        }
    }
}
