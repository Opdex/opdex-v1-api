using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets
{
    public class ProcessClaimPendingMarketOwnershipLogCommandHandler : IRequestHandler<ProcessClaimPendingMarketOwnershipLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessClaimPendingMarketOwnershipLogCommandHandler> _logger;

        public ProcessClaimPendingMarketOwnershipLogCommandHandler(IMediator mediator, ILogger<ProcessClaimPendingMarketOwnershipLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessClaimPendingMarketOwnershipLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Log.Contract, findOrThrow: true));

                if (request.BlockHeight < market.ModifiedBlock)
                {
                    return true;
                }

                market.SetOwnershipClaimed(request.Log, request.BlockHeight);

                var marketId = await _mediator.Send(new MakeMarketCommand(market, request.BlockHeight));

                return marketId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ClaimPendingMarketOwnershipLog)}");

                return false;
            }
        }
    }
}
