using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Blocks
{
    public class CreateRewindToBlockCommandHandler : IRequestHandler<CreateRewindToBlockCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateRewindToBlockCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateRewindToBlockCommand request, CancellationToken cancellationToken)
        {
            // Todo: Consider a MaxRewind limit to protect from accidental triggers from `index/rewind`.
            // MaxRewind could potentially create a future problem if it is not limited to the endpoint and
            // a real rewind is larger than our set maximum limit.

            // Ensure the rewind block exists
            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(request.Block, false), cancellationToken);
            if (block == null) throw new InvalidDataException(nameof(block), "Unable to find a block by the provided block number.");

            var rewound = await _mediator.Send(new MakeRewindToBlockCommand(request.Block));
            if (!rewound) return false;

            // todo: refresh stale records

            return rewound;
        }
    }
}
