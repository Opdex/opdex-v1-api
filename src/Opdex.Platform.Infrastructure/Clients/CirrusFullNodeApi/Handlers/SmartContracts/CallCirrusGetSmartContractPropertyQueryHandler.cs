using MediatR;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusGetSmartContractPropertyQueryHandler : IRequestHandler<CallCirrusGetSmartContractPropertyQuery, SmartContractMethodParameter>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetSmartContractPropertyQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<SmartContractMethodParameter> Handle(CallCirrusGetSmartContractPropertyQuery request, CancellationToken cancellationToken)
        {
            var value = await _smartContractsModule.GetContractStorageAsync(request.Contract, request.PropertyStateKey, ((uint)request.PropertyType).ToString(),
                                                                            request.BlockHeight, cancellationToken);

            return new SmartContractMethodParameter(value, request.PropertyType);
        }
    }
}
