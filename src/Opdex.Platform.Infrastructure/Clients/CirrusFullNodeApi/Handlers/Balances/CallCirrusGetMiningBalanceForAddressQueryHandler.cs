using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Balances
{
    public class CallCirrusGetMiningBalanceForAddressQueryHandler : IRequestHandler<CallCirrusGetMiningBalanceForAddressQuery, UInt256>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetMiningBalanceForAddressQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<UInt256> Handle(CallCirrusGetMiningBalanceForAddressQuery request, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                new SmartContractMethodParameter(request.Miner)
            };

            var localCallRequest = new LocalCallRequestDto(request.MiningPool, MiningPoolConstants.Methods.GetBalance, parameters, request.BlockHeight);
            var response = await _smartContractsModule.LocalCallAsync(localCallRequest, cancellationToken);
            return response.DeserializeValue<UInt256>();
        }
    }
}
