using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusCreateSmartContractCommandHandler : IRequestHandler<CallCirrusCreateSmartContractCommand, Sha256>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusCreateSmartContractCommandHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public Task<Sha256> Handle(CallCirrusCreateSmartContractCommand request, CancellationToken cancellationToken)
        {
            return _smartContractsModule.CreateSmartContractAsync(request.RequestDto, cancellationToken);
        }
    }
}
