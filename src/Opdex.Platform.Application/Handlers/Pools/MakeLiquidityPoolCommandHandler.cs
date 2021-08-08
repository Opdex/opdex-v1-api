using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class MakeLiquidityPoolCommandHandler : IRequestHandler<MakeLiquidityPoolCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeLiquidityPoolCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<long> Handle(MakeLiquidityPoolCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistLiquidityPoolCommand(request.LiquidityPool), cancellationToken);
        }
    }
}