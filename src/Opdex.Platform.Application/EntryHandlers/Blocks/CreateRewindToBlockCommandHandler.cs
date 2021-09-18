using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
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
        private readonly ILogger<CreateRewindToBlockCommandHandler> _logger;

        public CreateRewindToBlockCommandHandler(IMediator mediator, ILogger<CreateRewindToBlockCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            // If delete fails, return immediately
            if (!rewound) return false;

            _logger.LogTrace("Beginning to refresh stale records.");

            // Refresh stale records
            rewound = await _mediator.Send(new CreateRewindAddressBalancesCommand(request.Block)) && rewound;
            rewound = await _mediator.Send(new CreateRewindMiningPositionsCommand(request.Block)) && rewound;
            rewound = await _mediator.Send(new CreateRewindStakingPositionsCommand(request.Block)) && rewound;
            rewound = await _mediator.Send(new CreateRewindDeployersCommand(request.Block)) && rewound;

            _logger.LogTrace("Refreshing of stale records finished.");

            return rewound;
        }
    }
}
