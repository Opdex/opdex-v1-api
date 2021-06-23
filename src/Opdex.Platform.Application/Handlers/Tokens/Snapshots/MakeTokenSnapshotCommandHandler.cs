using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens.Snapshots
{
    public class MakeTokenSnapshotCommandHandler : IRequestHandler<MakeTokenSnapshotCommand, bool>
    {
        private readonly IMediator _mediator;

        public MakeTokenSnapshotCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<bool> Handle(MakeTokenSnapshotCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistTokenSnapshotCommand(request.Snapshot), CancellationToken.None);
        }
    }
}