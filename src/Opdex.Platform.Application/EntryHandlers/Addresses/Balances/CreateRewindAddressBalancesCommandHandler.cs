using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Balances
{
    public class CreateRewindAddressBalancesCommandHandler : IRequestHandler<CreateRewindAddressBalancesCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateRewindAddressBalancesCommandHandler> _logger;

        public CreateRewindAddressBalancesCommandHandler(IMediator mediator, ILogger<CreateRewindAddressBalancesCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateRewindAddressBalancesCommand request, CancellationToken cancellationToken)
        {
            var balances = await _mediator.Send(new RetrieveAddressBalancesByModifiedBlockQuery(request.RewindHeight), cancellationToken);
            var staleCount = balances.Count();

            _logger.LogDebug($"Found {staleCount} stale address balances.");

            // Group the balances by their token
            var balancesByToken = balances.GroupBy(balance => balance.TokenId);

            int refreshFailureCount = 0;

            foreach (var group in balancesByToken)
            {
                // Get the token for the group from the database
                var token = await _mediator.Send(new RetrieveTokenByIdQuery(group.Key));

                // Split the address balances by token into chunks of 10
                var balanceChunks = group.Chunk(10);

                foreach (var chunk in balanceChunks)
                {
                    // Each chunk runs in parallel where the address balance will be updated by rewind height
                    var callResults = await Task.WhenAll(chunk.Select(balance => RefreshAddressBalance(balance, token.Address, request.RewindHeight)));
                    refreshFailureCount += callResults.Count(succeeded => !succeeded);
                }
            }

            _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} address balances.");

            if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale address balances.");

            return refreshFailureCount == 0;

            async Task<bool> RefreshAddressBalance(AddressBalance balance, Address token, ulong rewindHeight)
            {
                var id = await _mediator.Send(new MakeAddressBalanceCommand(balance, token, request.RewindHeight));
                return id != 0;
            }
        }
    }
}
