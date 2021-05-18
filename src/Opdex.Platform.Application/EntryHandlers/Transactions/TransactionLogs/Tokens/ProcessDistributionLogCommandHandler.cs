using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens
{
    public class ProcessDistributionLogCommandHandler : IRequestHandler<ProcessDistributionLogCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessDistributionLogCommandHandler> _logger;

        public ProcessDistributionLogCommandHandler(IMediator mediator, ILogger<ProcessDistributionLogCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessDistributionLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Todo: This throws not found exception
                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Log.Contract), CancellationToken.None);
                
                // Todo: Validate the token is ODX or set flag in DB
                if (token == null) return false;
                
                var latestDistribution = await _mediator.Send(new RetrieveLatestTokenDistributionQuery(), CancellationToken.None);
                
                if (request.Log.PeriodIndex <= latestDistribution.PeriodIndex) return false;

                var distributionBlock = request.BlockHeight;
                var nexDistributionBlock = distributionBlock + 1_971_000;
                var odxDistribution = new TokenDistribution(request.Log.VaultAmount, request.Log.MiningAmount, (int)request.Log.PeriodIndex, request.BlockHeight, 
                    nexDistributionBlock);
                
                return await _mediator.Send(new MakeTokenDistributionCommand(odxDistribution), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(DistributionLog)}");
               
                return false;
            }
        }
    }
}