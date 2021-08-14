using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.MiningPools;
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
            return await _mediator.Send(new PersistMiningPoolCommand(request.MiningPool), cancellationToken);
        }
    }
}
