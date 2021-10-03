using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Common.Constants.SmartContracts.LiquidityPools;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningPools
{
    public class CreateMiningPoolCommandHandler : IRequestHandler<CreateMiningPoolCommand, ulong>
    {
        private readonly IMediator _mediator;

        public CreateMiningPoolCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ulong> Handle(CreateMiningPoolCommand request, CancellationToken cancellationToken)
        {
            // Todo: Should probably replace with a RetrieveStakingPoolContractSummaryQuery when we have one.
            var miningPoolAddress = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.LiquidityPoolAddress,
                                                                                                     StakingPoolConstants.StateKeys.MiningPool,
                                                                                                     SmartContractParameterType.Address,
                                                                                                     request.BlockHeight));

            var address = miningPoolAddress.Parse<Address>();

            var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(address, findOrThrow: false));

            if (miningPool != null)
            {
                return miningPool.Id;
            }

            miningPool = new MiningPool(request.LiquidityPoolId, address, request.BlockHeight);

            return await _mediator.Send(new MakeMiningPoolCommand(miningPool, request.BlockHeight));
        }
    }
}
