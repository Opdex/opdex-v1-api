using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools
{
    public class MakeLiquidityPoolCommandHandler : IRequestHandler<MakeLiquidityPoolCommand, ulong>
    {
        private readonly IMediator _mediator;

        public MakeLiquidityPoolCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<ulong> Handle(MakeLiquidityPoolCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistLiquidityPoolCommand(request.LiquidityPool), cancellationToken);
        }
    }
}
