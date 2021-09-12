using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets
{
    public class ProcessChangeMarketPermissionLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessChangeMarketPermissionLogCommand, bool>
    {
        private readonly ILogger<ProcessChangeMarketPermissionLogCommandHandler> _logger;

        public ProcessChangeMarketPermissionLogCommandHandler(IMediator mediator, ILogger<ProcessChangeMarketPermissionLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessChangeMarketPermissionLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Log.Contract), cancellationToken);
                var marketPermission = await _mediator.Send(new RetrieveMarketPermissionQuery(market.Id,
                                                                                              request.Log.Address,
                                                                                              request.Log.Permission,
                                                                                              false), cancellationToken);

                if (marketPermission is null)
                {
                    // create
                    marketPermission = new MarketPermission(market.Id,
                                                            request.Log.Address,
                                                            request.Log.Permission,
                                                            request.Log.IsAuthorized,
                                                            request.Sender,
                                                            request.BlockHeight);
                }

                if (request.Log.IsAuthorized)
                {
                    marketPermission.Authorize(request.Sender, request.BlockHeight);
                }
                else
                {
                    marketPermission.Revoke(request.Sender, request.BlockHeight);
                }

                return await _mediator.Send(new MakeMarketPermissionCommand(marketPermission), cancellationToken) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ChangeMarketPermissionLog)}");

                return false;
            }
        }
    }
}
