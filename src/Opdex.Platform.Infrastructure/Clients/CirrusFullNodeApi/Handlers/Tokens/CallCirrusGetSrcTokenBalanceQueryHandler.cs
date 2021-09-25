using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetSrcTokenBalanceQueryHandler : IRequestHandler<CallCirrusGetSrcTokenBalanceQuery, UInt256>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private const string MethodName = StandardTokenConstants.Methods.GetBalance;

        public CallCirrusGetSrcTokenBalanceQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<UInt256> Handle(CallCirrusGetSrcTokenBalanceQuery request, CancellationToken cancellationToken)
        {
            var parameters = new[] { new SmartContractMethodParameter(request.Owner).Serialize() };
            var balanceRequest = new LocalCallRequestDto(request.Token, request.Owner, MethodName, parameters, request.BlockHeight);

            var response = await _smartContractsModule.LocalCallAsync(balanceRequest, cancellationToken);

            return response.DeserializeValue<UInt256>();
        }
    }
}
