using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses
{
    public class GetMiningPositionByPoolQueryHandler : IRequestHandler<GetMiningPositionByPoolQuery, MiningPositionDto>
    {
        private readonly IMediator _mediator;

        public GetMiningPositionByPoolQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<MiningPositionDto> Handle(GetMiningPositionByPoolQuery request, CancellationToken cancellationToken)
        {
            var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(request.MiningPoolAddress, findOrThrow: true), cancellationToken);
            var addressMining = await _mediator.Send(new RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(miningPool.Id,
                                                                                                          request.Address,
                                                                                                          findOrThrow: true), cancellationToken);
            var liqudityPool = await _mediator.Send(new RetrieveLiquidityPoolByIdQuery(miningPool.LiquidityPoolId, findOrThrow: true), cancellationToken);
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(liqudityPool.LpTokenId, findOrThrow: true), cancellationToken);

            return new MiningPositionDto
            {
                Address = request.Address,
                Amount = addressMining.Balance.InsertDecimal(token.Decimals),
                MiningPool = miningPool.Address,
                MiningToken = token.Address
            };
        }
    }
}
