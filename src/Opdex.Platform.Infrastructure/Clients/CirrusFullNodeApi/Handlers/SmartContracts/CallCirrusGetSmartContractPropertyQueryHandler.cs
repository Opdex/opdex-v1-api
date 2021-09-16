using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusGetSmartContractPropertyQueryHandler : IRequestHandler<CallCirrusGetSmartContractPropertyQuery, string>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetSmartContractPropertyQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<string> Handle(CallCirrusGetSmartContractPropertyQuery request, CancellationToken cancellationToken)
        {
            var propertyType = ((uint)request.PropertyType).ToString();

            return await _smartContractsModule.GetContractStorageAsync(request.Contract, request.PropertyStateKey,
                                                                       propertyType, request.BlockHeight, cancellationToken);
        }
    }
}
