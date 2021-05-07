using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models;
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
            var market = new Market(request.Address, request.DeployerId, request.StakingTokenId, request.AuthPoolCreators, 
                request.AuthProviders, request.AuthTraders, request.Fee);
            
            return _mediator.Send(new PersistMarketCommand(market), CancellationToken.None);
        }
    }
}