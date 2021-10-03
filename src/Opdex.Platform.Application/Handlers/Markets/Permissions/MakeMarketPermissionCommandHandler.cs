using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets.Permissions
{
    public class MakeMarketPermissionCommandHandler : IRequestHandler<MakeMarketPermissionCommand, ulong>
    {
        private readonly IMediator _mediator;

        public MakeMarketPermissionCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<ulong> Handle(MakeMarketPermissionCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistMarketPermissionCommand(request.MarketPermission), cancellationToken);
        }
    }
}
