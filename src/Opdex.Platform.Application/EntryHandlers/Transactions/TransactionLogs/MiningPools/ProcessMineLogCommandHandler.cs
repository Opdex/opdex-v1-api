using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.MiningPools
{
    public class ProcessMineLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessMineLogCommand, bool>
    {
        private readonly ILogger<ProcessMineLogCommandHandler> _logger;

        public ProcessMineLogCommandHandler(IMediator mediator, ILogger<ProcessMineLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessMineLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }
                
                var miningPoolQuery = new RetrieveMiningPoolByAddressQuery(request.Log.Contract, findOrThrow: true);
                var miningPool = await _mediator.Send(miningPoolQuery, CancellationToken.None);

                var addressMiningQuery = new RetrieveAddressMiningByMiningPoolIdAndOwnerQuery(miningPool.Id, request.Log.Miner, findOrThrow: false);
                var miningBalance = await _mediator.Send(addressMiningQuery, CancellationToken.None) ?? 
                                    new AddressMining(miningPool.Id, request.Log.Miner, "0", request.BlockHeight);

                var isNewer = request.BlockHeight < miningBalance.ModifiedBlock;
                if (isNewer && miningBalance.Id != 0)
                {
                    return false;
                }
                
                var queryParameters = new[] {request.Log.Miner.ToSmartContractParameter(SmartContractParameterType.Address)};
                var miningBalanceQuery = new RetrieveCirrusLocalCallSmartContractQuery(request.Log.Contract, "GetBalance", queryParameters);
                var miningBalanceResponse = await _mediator.Send(miningBalanceQuery, CancellationToken.None);
                var latestMiningBalance = miningBalanceResponse.DeserializeValue<string>();
                
                miningBalance.SetBalance(latestMiningBalance, request.BlockHeight);

                var miningBalanceCommand = new MakeAddressMiningCommand(miningBalance);
                var miningBalanceId = await _mediator.Send(miningBalanceCommand, CancellationToken.None);

                return miningBalanceId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(MineLog)}");
               
                return false;
            }
        }
    }
}