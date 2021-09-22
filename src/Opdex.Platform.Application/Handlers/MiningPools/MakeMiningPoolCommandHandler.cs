using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.MiningPools
{
    public class MakeMiningPoolCommandHandler : IRequestHandler<MakeMiningPoolCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeMiningPoolCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(MakeMiningPoolCommand request, CancellationToken cancellationToken)
        {
            if (request.Refresh)
            {
                var summary = await _mediator.Send(new RetrieveMiningPoolContractSummaryQuery(request.MiningPool.Address,
                                                                                              request.BlockHeight,
                                                                                              includeRewardPerBlock: request.RefreshRewardPerBlock,
                                                                                              includeRewardPerLpt: request.RefreshRewardPerLpt,
                                                                                              includeMiningPeriodEndBlock: request.RefreshMiningPeriodEndBlock));

                request.MiningPool.Update(summary);
            }

            return await _mediator.Send(new PersistMiningPoolCommand(request.MiningPool));
        }
    }
}
