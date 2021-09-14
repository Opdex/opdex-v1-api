using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Balances
{
    public class RewindAddressBalancesCommandHandler : IRequestHandler<RewindAddressBalancesCommand, bool>
    {
        private readonly IMediator _mediator;

        public RewindAddressBalancesCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(RewindAddressBalancesCommand request, CancellationToken cancellationToken)
        {
            var balances = await _mediator.Send(new RetrieveAddressBalancesByModifiedBlockQuery(request.RewindHeight), cancellationToken);

            // Group the balances by their token
            var balancesByToken = balances.GroupBy(balance => balance.TokenId);

            foreach (var group in balancesByToken)
            {
                // Get the token for the group from the database
                var token = await _mediator.Send(new RetrieveTokenByIdQuery(group.Key));

                // Split the address balances by token into chunks of 10
                var balanceChunks = group
                    .Select((balance, i) => new { Value = balance, Index = i })
                    .GroupBy(x => x.Index / 10)
                    .Select(g => g.Select(x => x.Value));

                foreach (var chunk in balanceChunks)
                {
                    // Each chunk runs in parallel
                    var tasks = chunk.Select(balance => _mediator.Send(new MakeAddressBalanceCommand(balance, token.Address, request.RewindHeight)));

                    await Task.WhenAll(tasks);
                }
            }

            return true;
        }
    }
}
