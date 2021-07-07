using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Pools;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers
{
    public class MiningPoolDtoAssembler : IModelAssembler<MiningPool, MiningPoolDto>
    {
        private readonly IMediator _mediator;

        public MiningPoolDtoAssembler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<MiningPoolDto> Assemble(MiningPool miningPool)
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByIdQuery(miningPool.LiquidityPoolId));
            var tokensMining = await _mediator.Send(new RetrieveAddressBalanceByTokenIdAndOwnerQuery(liquidityPool.LpTokenId, miningPool.Address, false));
            var latestBlock = await _mediator.Send(new RetrieveLatestBlockQuery());

            return new MiningPoolDto
            {
                Address = miningPool.Address,
                MiningPeriodEndBlock = miningPool.MiningPeriodEndBlock,
                RewardPerBlock = miningPool.RewardPerBlock,
                RewardPerLpt = miningPool.RewardPerLpt,
                TokensMining = tokensMining?.Balance ?? "0",
                IsActive = miningPool.MiningPeriodEndBlock > latestBlock.Height
            };
        }
    }
}