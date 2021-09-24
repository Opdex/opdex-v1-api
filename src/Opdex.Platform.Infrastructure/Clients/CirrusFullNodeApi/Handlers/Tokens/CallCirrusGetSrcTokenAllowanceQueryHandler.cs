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
    public class CallCirrusGetSrcTokenAllowanceQueryHandler : IRequestHandler<CallCirrusGetSrcTokenAllowanceQuery, UInt256>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private const string MethodName = StandardTokenConstants.Methods.Allowance;

        public CallCirrusGetSrcTokenAllowanceQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<UInt256> Handle(CallCirrusGetSrcTokenAllowanceQuery request, CancellationToken cancellationToken)
        {
            var parameters = new[]
            {
                new SmartContractMethodParameter(request.Owner).Serialize(),
                new SmartContractMethodParameter(request.Spender).Serialize()
            };

            var allowanceResponse = await _smartContractsModule.LocalCallAsync(new LocalCallRequestDto(request.Token, request.Spender,
                                                                                                       MethodName, parameters), cancellationToken);

            return allowanceResponse.DeserializeValue<UInt256>();
        }
    }
}
