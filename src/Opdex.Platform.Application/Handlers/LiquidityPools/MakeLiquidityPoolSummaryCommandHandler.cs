using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools
{
    public class MakeLiquidityPoolSummaryCommandHandler : IRequestHandler<MakeLiquidityPoolSummaryCommand, ulong>
    {
        private readonly IMediator _mediator;

        public MakeLiquidityPoolSummaryCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<ulong> Handle(MakeLiquidityPoolSummaryCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistLiquidityPoolSummaryCommand(request.Summary), cancellationToken);
        }
    }
}
