using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;

namespace Opdex.Platform.Application.Handlers.Pools
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
            return await _mediator.Send(new PersistMiningPoolCommand(request.MiningPool), cancellationToken);
        }
    }
}