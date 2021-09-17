using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Balances
{
    public class CreateRewindAddressBalancesCommandHandler : IRequestHandler<CreateRewindAddressBalancesCommand, bool>
    {
        private readonly IMediator _mediator;

        public CreateRewindAddressBalancesCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(CreateRewindAddressBalancesCommand request, CancellationToken cancellationToken)
        {
            var balances = await _mediator.Send(new RetrieveAddressBalancesByModifiedBlockQuery(request.RewindHeight), cancellationToken);

            // Group the balances by their token
            var balancesByToken = balances.GroupBy(balance => balance.TokenId);

            foreach (var group in balancesByToken)
            {
                // Get the token for the group from the database
                var token = await _mediator.Send(new RetrieveTokenByIdQuery(group.Key));

                // Split the address balances by token into chunks of 10
                var balanceChunks = group.Chunk(10);

                foreach (var chunk in balanceChunks)
                {
                    // Each chunk runs in parallel where the address balance will be updated by rewind height
                    var tasks = chunk.Select(balance => _mediator.Send(new MakeAddressBalanceCommand(balance, token.Address, request.RewindHeight)));

                    await Task.WhenAll(tasks);
                }
            }

            return true;
        }
    }
}
