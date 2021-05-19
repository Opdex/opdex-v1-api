using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;

namespace Opdex.Platform.Application.Handlers.Markets
{
    public class MakeMarketCommandHandler : IRequestHandler<MakeMarketCommand, long>
    {
        private readonly IMediator _mediator;
        
        public MakeMarketCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<long> Handle(MakeMarketCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistMarketCommand(request.Market), CancellationToken.None);
        }
    }
}