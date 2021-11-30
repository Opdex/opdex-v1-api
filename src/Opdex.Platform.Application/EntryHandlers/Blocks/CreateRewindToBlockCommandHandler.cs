using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Permissions;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
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
        // Devnet averages 450-500 blocks per minute
        private const ulong MaxRewind = 10_800; // 48 hours - being generous at first, will take ~20 minutes to rewind

        public CreateRewindToBlockCommandHandler(IMediator mediator, ILogger<CreateRewindToBlockCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateRewindToBlockCommand request, CancellationToken cancellationToken)
        {
            // Ensure the rewind block exists
            var rewindBlock = await _mediator.Send(new RetrieveBlockByHeightQuery(request.Block, false), cancellationToken);
            if (rewindBlock == null) throw new InvalidDataException(nameof(rewindBlock), "Unable to find a block by the provided block number.");

            // Get current block and verify that the rewind does not exceed the max limit
            var currentBlock = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);
            if (currentBlock.Height - rewindBlock.Height > MaxRewind)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Block), "Rewind request exceeds maximum rewind limit.");
            }

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var rewound = await _mediator.Send(new MakeRewindToBlockCommand(request.Block), cancellationTokenSource.Token);
            if (!rewound) return false;

            _logger.LogTrace("Beginning to refresh stale records.");

            // refresh stale records
            rewound = await _mediator.Send(new CreateRewindAddressBalancesCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindMiningPositionsCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindStakingPositionsCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindDeployersCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindMiningGovernancesAndNominationsCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindVaultsCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindVaultCertificatesCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindMiningPoolsCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindMarketsCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindMarketPermissionsCommand(request.Block), CancellationToken.None) && rewound;
            rewound = await _mediator.Send(new CreateRewindSnapshotsCommand(request.Block), CancellationToken.None) && rewound; // markets/pools/tokens combined

            _logger.LogTrace("Refreshing of stale records finished.");

            return rewound;
        }
    }
}
