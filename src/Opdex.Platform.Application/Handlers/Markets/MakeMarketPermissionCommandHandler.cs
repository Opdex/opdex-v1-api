using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;

namespace Opdex.Platform.Application.Handlers.Markets
{
    public class MakeMarketPermissionCommandHandler : IRequestHandler<MakeMarketPermissionCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeMarketPermissionCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<long> Handle(MakeMarketPermissionCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistMarketPermissionCommand(request.MarketPermission), cancellationToken);
        }
    }
}